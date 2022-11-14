using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AchievementAsset{
public class GameSceneAchievements : MonoBehaviour
{
    private AchievementItem[] achievementList;
    [SerializeField]
    private int[] achievementsInfoToRetrieve;
    [SerializeField]
    private BannerCreator bannerCreator;
    [SerializeField]
    private string achievementListName = "Achievement";

    void Start()
    {
        SetUpAchievementList();
    }

    void SetUpAchievementList()
    {
        if (achievementsInfoToRetrieve != null)
        {
            if (achievementsInfoToRetrieve.Length > 0)
            {
                RetrieveSpecificAchievements();
            }
            else
            {
                RetrieveAllAchievements();
            }
        }
        else
        {
            RetrieveAllAchievements();
        }


    }

    void RetrieveAllAchievements()
    {
        int length = PlayerPrefs.GetInt(achievementListName + "Length", 0);
        if (length > 0)
        {
            Debug.Log(length);

            achievementList = new AchievementItem[length];
            for (int i = 0; i < length; i++)
            {
                RetrieveInformation(i);
                achievementList[i].SetAchievementID(i, achievementListName);
            }
        }
    }

    void RetrieveSpecificAchievements()
    {
        int length = achievementsInfoToRetrieve.Length;
        achievementList = new AchievementItem[length];
        for (int i = 0; i < length; i++)
        {
            RetrieveInformation(achievementsInfoToRetrieve[i]);
            achievementList[i].SetAchievementID(achievementsInfoToRetrieve[i], achievementListName);
        }
    }

    void RetrieveInformation(int index)
    {
        string name = PlayerPrefs.GetString(achievementListName + index + "Name", "");
        int type = PlayerPrefs.GetInt(achievementListName + index + "Type", -1);
        int check = PlayerPrefs.GetInt(achievementListName + index + "Check", -1);
        int completed = PlayerPrefs.GetInt(achievementListName + index + "Completed", 0);
        int achievementSaveProgressSetting = PlayerPrefs.GetInt("Save" + achievementListName + "Progress" + index, 0);
        achievementList[index] = new AchievementItem();
        achievementList[index].NewAchievementItemCreation(name, type, check, achievementListName);
        if (completed == 1)
        {
            achievementList[index].SetCompletionState(true);
        }
        if (achievementSaveProgressSetting == 1)
        {
            achievementList[index].SetDisplayAchievementProgressSetting(true);
        }
        SetTheCheckValues(type, index);
    }

    void SetTheCheckValues(int type, int index)
    {
        if (type == 0)
        {
            //int
            float value = PlayerPrefs.GetFloat(achievementListName + index + "ValueInt", 0);
            achievementList[index].SetNumericalValue(value);
        }
        else if (type == 1)
        {
            //float
            float value = PlayerPrefs.GetFloat(achievementListName + index + "ValueFloat", 0);
            achievementList[index].SetNumericalValue(value);
        }
        else if (type == 2)
        {
            //position
            float x = PlayerPrefs.GetFloat(achievementListName + index + "ValuePositionX", 0);
            float y = PlayerPrefs.GetFloat(achievementListName + index + "ValuePositionY", 0);
            float z = PlayerPrefs.GetFloat(achievementListName + index + "ValuePositionZ", 0);
            Vector3 position = new Vector3(x, y, z);
            float distanceFromPosition = PlayerPrefs.GetFloat(achievementListName + index + "Value", 0);
            achievementList[index].SetPositionValue(position);
            achievementList[index].SetNumericalValue(distanceFromPosition);
        }
        else if (type == 3)
        {
            //string
            string value = PlayerPrefs.GetString(achievementListName + index + "ValueString", "");
            achievementList[index].SetStringValue(value);
        }
        LoadTheImage(index);
    }

    void LoadTheImage(int index)
    {
        if (PlayerPrefs.GetString(achievementListName + index + "ImageLocation", "") != "")
        {
            Sprite achievementSprite = Resources.Load<Sprite>(PlayerPrefs.GetString(achievementListName + index + "ImageLocation"));
            achievementList[index].SetIconObject(achievementSprite);
        }
    }

    ///<summary>
    ///Call this function when the achievement to check has an integer trigger! Returns true if the achievement is completed.
    ///</summary>
    public bool UpdateAchievementInteger(int achievementIndex, int value)
    {
        if (achievementIndex >= 0 && achievementIndex < achievementList.Length)
        {
            bool result = achievementList[achievementIndex].AchievementCompleted("", value, 0, Vector3.zero);
            if (result)
            {
                CreateBanner(achievementIndex);
            }
            return result;

        }
        Debug.LogWarning("Achievement index was out of bounds!");
        return false;
    }

    ///<summary>
    ///Call this function when the achievement to check has a float trigger! Returns true if the achievement is completed.
    ///</summary>
    public bool UpdateAchievementFloat(int achievementIndex, float value)
    {
        if (achievementIndex >= 0 && achievementIndex < achievementList.Length)
        {
            bool result = achievementList[achievementIndex].AchievementCompleted("", 0, value, Vector3.zero);
            if (result)
            {
                CreateBanner(achievementIndex);
            }
            return result;
        }
        Debug.LogWarning("Achievement index was out of bounds!");
        return false;
    }

    ///<summary>
    ///Call this function when the achievement to check uses distance as a trigger! Returns true if the achievement is completed.
    ///</summary>
    public bool UpdateAchievementPosition(int achievementIndex, Vector3 position)
    {
        if (achievementIndex >= 0 && achievementIndex < achievementList.Length)
        {
            bool result = achievementList[achievementIndex].AchievementCompleted("", 0, 0, position);
            if (result)
            {
                CreateBanner(achievementIndex);
            }
            return result;
        }
        Debug.LogWarning("Achievement index was out of bounds!");
        return false;
    }

    ///<summary>
    ///Call this function when the achievement to check uses a string as a trigger! Returns true if the achievement is completed.
    ///</summary>
    public bool UpdateAchievementString(int achievementIndex, string value)
    {
        if (achievementIndex >= 0 && achievementIndex < achievementList.Length)
        {
            bool result = achievementList[achievementIndex].AchievementCompleted(value, 0, 0, Vector3.zero);
            if (result)
            {
                CreateBanner(achievementIndex);
            }
            return result;
        }
        Debug.LogWarning("Achievement index was out of bounds!");
        return false;
    }

    void CreateBanner(int achievementIndex)
    {
        if (bannerCreator != null)
        {
            string name = achievementList[achievementIndex].RetrieveAchievementName();
            Sprite icon = achievementList[achievementIndex].RetrieveAchievementIcon();
            bannerCreator.CreateNewBanner(name, icon);
        }
    }

    ///<summary>
    ///Returns the index of the achievement with a specific name.
    ///</summary>
    public int FindAchievementIndex(string nameOfAchievement)
    {
        int length = achievementList.Length;
        for (int i = 0; i < length; i++)
        {
            if (achievementList[i].RetrieveAchievementName() == nameOfAchievement)
            {
                return i;
            }
        }
        Debug.LogWarning("No achievement found with name: " + nameOfAchievement + "!");
        return -1;
    }


}
}
