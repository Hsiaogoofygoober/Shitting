using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AchievementAsset{
public class ExampleAchievementTestScript : MonoBehaviour
{
    public GameSceneAchievements achievements;
    public string achievementName;
    private int numberOfClicks = 0;
    public BannerCreator bannerCreator;
    public Sprite bannerIcon;
/* 
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AddToAchievement();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DisplayBannerExample();
        }

        
        if (Input.GetKeyDown(KeyCode.C)){
            //if you set the bool to true it will delete every player prefs entry that exists in this project. Use this only if you are certain that everything should be reset
            //it can be used to clear entries using the achievement name variable as well for single entries
            ClearPlayerPrefs(true);
        }
        
    }
 */
    void ClearPlayerPrefs(bool clearEverything)
    {
        if (clearEverything)
        {
            PlayerPrefs.DeleteAll();
        }
        else
        {
            int index = achievements.FindAchievementIndex(achievementName);
            PlayerPrefs.DeleteKey("Achievement" + index + "ValuePositionX");
            PlayerPrefs.DeleteKey("Achievement" + index + "ValuePositionY");
            PlayerPrefs.DeleteKey("Achievement" + index + "ValuePositionZ");
            PlayerPrefs.DeleteKey("Achievement" + index + "Value");
            PlayerPrefs.DeleteKey("Achievement" + index + "ValueString");
            PlayerPrefs.DeleteKey("Achievement" + index + "ValueFloat");
            PlayerPrefs.DeleteKey("Achievement" + index + "ValueInt");
            PlayerPrefs.DeleteKey("Achievement" + index + "Completed");
            PlayerPrefs.DeleteKey("Achievement" + index + "Check");
            PlayerPrefs.DeleteKey("Achievement" + index + "Type");
            PlayerPrefs.DeleteKey("Achievement" + index + "Name");
            PlayerPrefs.DeleteKey("Achievement" + index + "Progress");
            PlayerPrefs.DeleteKey("SaveAchievementProgress" + index);
            PlayerPrefs.DeleteKey("Achievement" + index + "ImageLocation");
        }
    }

    public void DisplayBannerExample()
    {
        string title = "Number of Clicks: " + numberOfClicks;
        bannerCreator.CreateNewBanner(title, bannerIcon);
    }

    public void AddToAchievement()
    {
        numberOfClicks++;
        int index = achievements.FindAchievementIndex(achievementName);
        bool completed = achievements.UpdateAchievementInteger(index, numberOfClicks);
        if (completed)
        {
            Debug.Log("AchievementCompleted");
        }
    }

}

}
