using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
This class should be added in the main menu scene or where your achievements should be displayed.

*/
namespace AchievementAsset{
public class AchievementsList : MonoBehaviour
{
    [SerializeField]
    private AchievementItem[] availableAchievements;
    [SerializeField]
    public bool shouldDisplayAchievements = false;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", true)]
    private GameObject achievementPrefab;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", true)]
    private bool displayInGrid = false;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", true)]
    private Vector2 itemPadding;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", true)]
    private GameObject backgroundImage;
    private ScrollRect scrollBackground;
    private GameObject[] achievementItemList;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", false), Tooltip("The path that is need to get to the object which indicates that the achievement is not yet completed")]
    private int[] disabledIndicatorPath;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", false), Tooltip("The path that is needed to reach the parent of the name and description objects")]
    private int[] descriptionTextPath;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", true)]
    private Color disabledIndicatorColor;
    private Vector2 dimensionsOfAchievementItem;
    private float widthOfBackground, heightOfBackground;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", true)]
    private bool scrollDirectionHorizontal = false;
    private int previousSortIndex;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", true)]
    private Color alternateAchievementItemColor, normalAchievementItemColor;
    [SerializeField, ConditionalHide("shouldDisplayAchievements", true)]
    private Vector3 achievementWindowPosition;
    private bool reverseListOrder = false;
    [SerializeField]
    private string saveNameForAchievementList = "Achievement";

    void Start()
    {
        int length = availableAchievements.Length;
        for (int i = 0; i < length; i++)
        {
            availableAchievements[i].SetAchievementID(i,saveNameForAchievementList);
        }

        if (PlayerPrefs.GetInt(saveNameForAchievementList+"Length", 0) != length)
        {
            StartCoroutine(StoreAchievementInformationIncrementaly(5));
        }

        //if you want to create the achievemenets later remove this.
        StartSettingUpAchievementList();
    }

    ///<summary>
    ///Sets up the achievement list. Starts with the background then creates the achievement objects and lastly determines the sort order.
    ///</summary>
    public void StartSettingUpAchievementList()
    {
        if (shouldDisplayAchievements)
        {
            CreateBackground();
            CreateAchievementObjects();
            DetermineAchievementSortOrder();
        }
    }

    void CreateAchievementObjects()
    {
        int length = availableAchievements.Length;
        achievementItemList = new GameObject[length];
        for (int i = 0; i < length; i++)
        {
            achievementItemList[i] = NewAchievementItem(i);
            achievementItemList[i].transform.localPosition = PositionAchievements(i);
        }
    }

    GameObject NewAchievementItem(int currentAchievementIndex)
    {
        bool achievementComplete = availableAchievements[currentAchievementIndex].CheckAchievementStatus();
        string name = availableAchievements[currentAchievementIndex].RetrieveAchievementName();
        GameObject go = CreateAchievementPrefab(name);
        if (!achievementComplete)
        {
            DisableAchievement(go);
        }
        SetNameAndDescriptionText(go, name, currentAchievementIndex);
        AssignAlternateColorToAchievement(currentAchievementIndex, go);
        DisplayAchievementProgress(currentAchievementIndex, go);
        return go;
    }

    void AssignAlternateColorToAchievement(int currentAchievementIndex, GameObject go)
    {
        if (go.GetComponent<Image>() != null)
        {
            if (currentAchievementIndex % 2 != 0)
            {
                go.GetComponent<Image>().color = alternateAchievementItemColor;
            }
            else
            {
                go.GetComponent<Image>().color = normalAchievementItemColor;
            }
        }
    }

    GameObject CreateAchievementPrefab(string name)
    {
        GameObject go = Instantiate(achievementPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        go.name = name;
        go.transform.SetParent(backgroundImage.transform);
        return go;
    }

    void DisplayAchievementProgress(int currentAchievementIndex, GameObject go)
    {
        if (availableAchievements[currentAchievementIndex].DisplayAchievementProgress())
        {
            GameObject progressDisplay = go.transform.GetChild(1).gameObject;
            progressDisplay.SetActive(true);
            if (availableAchievements[currentAchievementIndex].TypeOfProgressDisplay() == 1)
            {
                progressDisplay.transform.GetChild(0).gameObject.SetActive(true);
                float value = availableAchievements[currentAchievementIndex].RetrieveProgressOnAchievement();
                value /= availableAchievements[currentAchievementIndex].RetrieveAchievementDesiredValue();
                progressDisplay.transform.GetChild(0).gameObject.GetComponent<Slider>().value = value;
            }
            else if (availableAchievements[currentAchievementIndex].TypeOfProgressDisplay() == 2)
            {
                progressDisplay.transform.GetChild(1).gameObject.SetActive(true);
                string progress = "Progress: " + availableAchievements[currentAchievementIndex].RetrieveProgressOnAchievement();
                progress += "/" + availableAchievements[currentAchievementIndex].RetrieveAchievementDesiredValue();
                progressDisplay.transform.GetChild(1).gameObject.GetComponent<Text>().text = progress;
            }
        }
    }

    void DisableAchievement(GameObject go)
    {
        if (disabledIndicatorPath == null && go.GetComponent<Image>() != null)
        {
            go.GetComponent<Image>().color = disabledIndicatorColor;
        }
        else if (disabledIndicatorPath != null)
        {
            int length = disabledIndicatorPath.Length;
            if (length > 0)
            {
                GameObject child = go;
                for (int i = 0; i < length; i++)
                {
                    child = child.transform.GetChild(disabledIndicatorPath[i]).gameObject;
                }
                child.GetComponent<Image>().color = disabledIndicatorColor;
            }
        }
    }

    void SetNameAndDescriptionText(GameObject go, string name, int index)
    {
        if (descriptionTextPath == null && go.GetComponent<Text>() != null)
        {
            go.GetComponent<Text>().text = name;
        }
        else if (disabledIndicatorPath != null)
        {
            int length = disabledIndicatorPath.Length;
            if (length > 0)
            {
                GameObject child = go;
                for (int i = 0; i < length; i++)
                {
                    child = child.transform.GetChild(disabledIndicatorPath[i]).gameObject;
                }
                string description = availableAchievements[index].RetrieveAchievementDescription();
                //child.GetComponent<Image>().color = disabledIndicatorColor;
                child.transform.GetChild(0).gameObject.GetComponent<Text>().text = name;
                child.transform.GetChild(1).gameObject.GetComponent<Text>().text = description;
                Sprite icon = availableAchievements[index].RetrieveAchievementIcon();
                if (child.GetComponent<Image>() != null)
                {
                    child.GetComponent<Image>().sprite = icon;
                }
            }
        }
    }
    Vector3 PositionAchievements(int entryIndex)
    {
        if (displayInGrid)
        {
            return GetGridPosition(entryIndex);
        }
        else
        {
            return GetListPosition(entryIndex);
        }
    }

    Vector3 GetGridPosition(int entryIndex)
    {
        int maxXPosition = Mathf.FloorToInt(widthOfBackground / (dimensionsOfAchievementItem.x + itemPadding.x));
        int maxYPosition = Mathf.FloorToInt(heightOfBackground / (dimensionsOfAchievementItem.y + 2f * itemPadding.y));
        int xPosition = 0;
        int yPosition = 0;
        if (scrollDirectionHorizontal)
        {
            yPosition = (entryIndex % maxYPosition);
            xPosition = Mathf.FloorToInt(entryIndex / maxYPosition);
        }
        else
        {
            xPosition = (entryIndex % maxXPosition);
            yPosition = Mathf.FloorToInt(entryIndex / maxXPosition);
        }
        float xVectorValue = xPosition * dimensionsOfAchievementItem.x + 2f * itemPadding.x + dimensionsOfAchievementItem.x / 2f;
        float yVectorValue = heightOfBackground - (yPosition * dimensionsOfAchievementItem.y + 2f * itemPadding.y + dimensionsOfAchievementItem.y / 2f);
        Vector3 newPosition = new Vector3(xVectorValue, yVectorValue, 0);
        return newPosition;
    }

    Vector3 GetListPosition(int entryIndex)
    {
        if (dimensionsOfAchievementItem.x > 0)
        {
            if (scrollDirectionHorizontal)
            {
                float xDistance = dimensionsOfAchievementItem.x / 2f + entryIndex * (dimensionsOfAchievementItem.x + 2f * itemPadding.x);
                float yDistance = dimensionsOfAchievementItem.y / 2f + itemPadding.y;
                return new Vector3(xDistance, yDistance, 0);
            }
            else
            {
                float xDistance = dimensionsOfAchievementItem.x / 2f + itemPadding.x;
                float yDistance = heightOfBackground - (dimensionsOfAchievementItem.y / 2f + entryIndex * (dimensionsOfAchievementItem.y + 2f * itemPadding.y));
                return new Vector3(xDistance, yDistance, 0);
            }
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    ///<summary>
    ///When the achievement list should be displayed in reverse.
    ///</summary>
    public void ReverseListOrder(bool shouldListBeReversed)
    {
        reverseListOrder = shouldListBeReversed;
        SortAchievements(previousSortIndex);
    }

    ///<summary>
    ///When the sort method has been changed call this function.
    ///</summary>
    public void SortAchievements(int sortOption)
    {
        int length = availableAchievements.Length;
        previousSortIndex = sortOption;
        if (!reverseListOrder)
        {
            for (int i = 0; i < length; i++)
            {
                int nextAchievement = GetNextAchievementIndex(i, sortOption);
                GameObject achievement = achievementItemList[nextAchievement];
                AssignAlternateColorToAchievement(i, achievement);
                achievement.transform.localPosition = PositionAchievements(i);
            }
        }
        else
        {
            int index = 0;
            for (int i = length - 1; i > -1; i--)
            {
                int nextAchievement = GetNextAchievementIndex(i, sortOption);
                GameObject achievement = achievementItemList[nextAchievement];
                AssignAlternateColorToAchievement(i, achievement);
                achievement.transform.localPosition = PositionAchievements(index);
                index++;
            }
        }

    }

    int GetNextAchievementIndex(int index, int sortOption)
    {
        if (sortOption >= 0)
        {
            int length = availableAchievements.Length;
            for (int i = 0; i < length; i++)
            {
                if (availableAchievements[i].RetrieveIndex(sortOption) == index)
                {
                    return i;
                }
            }
        }
        else
        {
            return index;
        }
        Debug.LogError("An error has occured with getting the next alphabet achievement");
        return 0;

    }


    //sort them individually loop through the list for alphabet position and then for type etc.
    void DetermineAchievementSortOrder()
    {
        int length = availableAchievements.Length;
        SortAlphabetically();
        SortByType();
        SortByCompleted();
    }

    void SortAlphabetically()
    {
        int length = availableAchievements.Length;
        List<string> names = new List<string>();
        string[] startingNamesList = new string[length];
        for (int i = 0; i < length; i++)
        {
            names.Add(availableAchievements[i].RetrieveAchievementName());
            startingNamesList[i] = names[i];
        }
        names.Sort();
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (string.Compare(names[i], startingNamesList[j]) == 0)
                {
                    availableAchievements[j].SetAlphabetIndex(i);
                }
            }
        }
    }


    void SortByType()
    {
        int length = availableAchievements.Length;
        int newIndexValue = 0;
        int maxTypeValue = availableAchievements[0].NumberOfAchievementTypes();
        for (int i = 0; i < maxTypeValue; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (i == availableAchievements[j].RetrieveAchievementType())
                {
                    availableAchievements[j].SetTypeIndex(newIndexValue);
                    newIndexValue++;
                }
            }
        }

    }

    void SortByCompleted()
    {
        int length = availableAchievements.Length;
        int falseIndex = length - 1;
        int trueIndex = 0;
        for (int i = 0; i < length; i++)
        {
            if (availableAchievements[i].CheckAchievementStatus())
            {
                availableAchievements[i].SetCompletedIndex(trueIndex);
                trueIndex++;
            }
            else
            {
                availableAchievements[i].SetCompletedIndex(falseIndex);
                falseIndex--;
            }
        }
    }

    void CreateBackground()
    {
        if (shouldDisplayAchievements)
        {
            if (backgroundImage == null)
            {
                SetDimensionsForBackground(this.gameObject);
            }
            else
            {
                SetDimensionsForBackground(backgroundImage);
            }
        }
    }

    void SetDimensionsForBackground(GameObject origin)
    {
        if (origin.GetComponent<Image>() == null)
        {
            if (dimensionsOfAchievementItem == new Vector2(0, 0))
            {
                float dimensionsX = achievementPrefab.GetComponent<RectTransform>().rect.width;
                float dimensionsY = achievementPrefab.GetComponent<RectTransform>().rect.height;
                dimensionsOfAchievementItem = new Vector2(dimensionsX, dimensionsY);
            }
            float height = dimensionsOfAchievementItem.y + (itemPadding.y * 2f);
            float width = dimensionsOfAchievementItem.x + itemPadding.x * 2f;
            if (displayInGrid)
            {
                height = Screen.height;
                width = Screen.width;
            }
            else
            {
                int length = availableAchievements.Length;
                if (scrollDirectionHorizontal)
                {
                    width *= length;
                }
                else
                {
                    height *= length;

                }
            }

            heightOfBackground = height;
            widthOfBackground = width;
            backgroundImage = origin.gameObject.AddComponent<Image>().gameObject;
            backgroundImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            backgroundImage.transform.position = achievementWindowPosition;
            SetScrollDirection();
        }
        else
        {
            backgroundImage = origin.gameObject.GetComponent<Image>().gameObject;
            heightOfBackground = backgroundImage.GetComponent<RectTransform>().sizeDelta.y;
            widthOfBackground = backgroundImage.GetComponent<RectTransform>().sizeDelta.x;
            if (dimensionsOfAchievementItem == new Vector2(0, 0))
            {
                float dimensionsX = achievementPrefab.GetComponent<RectTransform>().rect.width;
                float dimensionsY = achievementPrefab.GetComponent<RectTransform>().rect.height;
                dimensionsOfAchievementItem = new Vector2(dimensionsX, dimensionsY);
            }
            SetScrollDirection();
        }
    }

    void SetScrollDirection()
    {
        if (this.gameObject.GetComponent<ScrollRect>() != null)
        {
            ScrollRect scrollRect = this.gameObject.GetComponent<ScrollRect>();
            if (scrollDirectionHorizontal)
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
            }
            else
            {
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
            }
        }
        if (backgroundImage.gameObject.GetComponent<ScrollRect>() != null)
        {
            ScrollRect scrollRect = backgroundImage.gameObject.GetComponent<ScrollRect>();
            if (scrollDirectionHorizontal)
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
            }
            else
            {
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
            }
        }
    }

    void StoreAchievementItemInformation()
    {
        int length = availableAchievements.Length;
        PlayerPrefs.SetInt(saveNameForAchievementList+"Length", length);
        for (int i = 0; i < length; i++)
        {
            availableAchievements[i].SaveAchievementItemInformation();
        }
    }

    IEnumerator StoreAchievementInformationIncrementaly(int increments)
    {
        int length = availableAchievements.Length;
        PlayerPrefs.SetInt(saveNameForAchievementList+"Length", length);
        for (int i = 0; i < length; i++)
        {
            availableAchievements[i].SaveAchievementItemInformation();
            if (i % increments == 0)
            {
                yield return new WaitForSeconds(0);
            }
        }
    }

}
}

