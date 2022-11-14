using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
namespace AchievementAsset{
[System.Serializable]
public class AchievementItem
{
    [SerializeField]
    private string achievementName;
    private enum AchievementTypes
    {
        integerValue = 0,
        floatValue = 1,
        positionValue = 2,
        stringValue = 3
    }

    private enum CheckTypes
    {
        greaterThan = 0,
        equalTo = 1,
        lessThan = 2
    }
    private enum DisplayProgressType
    {
        None = 0,
        LoadingBar = 1,
        TextDisplay = 2
    }

    [SerializeField, Tooltip("The string achievement type only has a check type of equal to")]
    private AchievementTypes achievementType = AchievementTypes.integerValue;
    [SerializeField]
    private CheckTypes checkType = CheckTypes.greaterThan;
    [SerializeField]
    private DisplayProgressType progressDisplay = DisplayProgressType.None;

    private int achievementID;
    [SerializeField, Tooltip("Use this for the string type of achievement")]
    private string checkValueText;
    [SerializeField]
    private Vector3 positionCheckValue;
    [SerializeField, Tooltip("For the position check use this value as the distance from the specific position selected. Use this value for the integer and float achievements as well")]
    private float distanceForPositionCheck;
    private bool achievementComplete;
    [SerializeField]
    private Sprite achievementIcon;
    private float progressMade;
    [SerializeField]
    private string achievementDescription;
    private int alphabetIndex, completedIndex, typeIndex;
    [SerializeField]
    private bool achievementProgressSavedBetweenScenes = false;
    private bool changeProgressValueActive = false;
    private bool achievementEffectCompleted = false;
    private string saveNameForAchievement;



    void OnAchievementCompleted (){
        if (!achievementEffectCompleted){

            achievementEffectCompleted = true;
        }
    }

    public void NewAchievementItemCreation(string name, int type, int checktype,string saveName)
    {
        achievementName = name;
        achievementType = (AchievementTypes)type;
        checkType = (CheckTypes)checktype;
        changeProgressValueActive = true;
        saveNameForAchievement = saveName;
        progressMade = RetrieveProgressOnAchievement();
    }

    public void SetNumericalValue(float value)
    {
        distanceForPositionCheck = value;
    }

    public void SetStringValue(string value)
    {
        checkValueText = value;
    }

    public void SetPositionValue(Vector3 position)
    {
        positionCheckValue = position;
    }

    public void SetCompletionState(bool state)
    {
        achievementComplete = state;
    }

    ///<summary>
    ///Call this function to check if the achievement has been completed.
    ///</summary>
    public bool AchievementCompleted(string stringComponent, int integerComponent, float floatComponent, Vector3 positionComponent)
    {
        if (!achievementComplete){
        bool result = false;
        if (achievementType == AchievementTypes.integerValue)
        {
            result = IntegerValueCheck(integerComponent);
        }
        else if (achievementType == AchievementTypes.floatValue)
        {
            result = FloatValueCheck(floatComponent);
        }
        else if (achievementType == AchievementTypes.positionValue)
        {
            result = PositionValueCheck(positionComponent);
        }
        else
        {
            result = StringValueCheck(stringComponent);
        }
        if (result){
            MarkAchievementAsCompleted();
            OnAchievementCompleted();
        }
        return result;
        } else {
            return false;
        }
    }

    bool IntegerValueCheck(int integerComponent)
    {
        int value = Mathf.FloorToInt(distanceForPositionCheck);
        if (checkType == CheckTypes.greaterThan && value < integerComponent)
        {
            return true;
        }
        else if (checkType == CheckTypes.equalTo && value == integerComponent)
        {
            return true;
        }
        else if (checkType == CheckTypes.lessThan && value > integerComponent)
        {
            return true;
        }
        progressMade = integerComponent;
        DetermineAchievementProgress();
        return false;
    }

    bool FloatValueCheck(float floatComponent)
    {
        float value = distanceForPositionCheck;
        if (checkType == CheckTypes.greaterThan && value < floatComponent)
        {
            return true;
        }
        else if (checkType == CheckTypes.equalTo && value == floatComponent)
        {
            return true;
        }
        else if (checkType == CheckTypes.lessThan && value > floatComponent)
        {
            return true;
        }
        progressMade = floatComponent;
        DetermineAchievementProgress();
        return false;
    }

    bool PositionValueCheck(Vector3 positionComponent)
    {
        float distance = (positionCheckValue - positionComponent).magnitude;
        if (checkType == CheckTypes.greaterThan && distanceForPositionCheck < distance)
        {
            return true;
        }
        else if (checkType == CheckTypes.equalTo && distanceForPositionCheck == distance)
        {
            return true;
        }
        else if (checkType == CheckTypes.lessThan && distanceForPositionCheck > distance)
        {
            return true;
        }
        progressMade = distance;
        DetermineAchievementProgress();
        return false;
    }

    bool StringValueCheck(string stringComponent)
    {
        if (checkValueText == stringComponent)
        {
            return true;
        }
        return false;
    }

    public void SetAchievementToCompleted()
    {
        if (achievementName != null)
        {
            PlayerPrefs.SetInt(achievementName, 1);
        }
        else
        {
            string name = saveNameForAchievement + achievementID;
        }
    }


    void MarkAchievementAsCompleted(){
        if (achievementName!=null){
            if (achievementName!=""){
                PlayerPrefs.SetInt(achievementName, 1);
            } else {
                PlayerPrefs.SetInt(saveNameForAchievement+achievementID, 1);
            }
        } else {
            PlayerPrefs.SetInt(saveNameForAchievement+achievementID, 1);
        }
    }

    public bool CheckAchievementStatus()
    {
        achievementComplete = false;
        if (achievementName != null&&achievementName!="")
        {

            if (PlayerPrefs.GetInt(achievementName, 0) == 1)
            {
                
                achievementComplete = true;
            } else {
                string name = saveNameForAchievement + achievementID;
            if (PlayerPrefs.GetInt(name, 0) == 1)
            {
                achievementComplete = true;
            }
            }
        }
        else
        {
            string name = saveNameForAchievement + achievementID;
            if (PlayerPrefs.GetInt(name, 0) == 1)
            {
                achievementComplete = true;
            }
        }
        if (!achievementComplete)
        {
            DetermineAchievementProgress();
        } else {
            OnAchievementCompleted();
        }
        return achievementComplete;
    }

    void DetermineAchievementProgress()
    {
        
        //check the amount of progress done on the achievement
        //display it visually somewhere
        if (achievementProgressSavedBetweenScenes&&changeProgressValueActive){
            if (achievementType == AchievementTypes.integerValue){
                PlayerPrefs.SetInt(saveNameForAchievement+achievementID+"Progress",Mathf.FloorToInt(progressMade));
            }
            else if(achievementType==AchievementTypes.floatValue||achievementType==AchievementTypes.positionValue){
                PlayerPrefs.SetFloat(saveNameForAchievement+achievementID+"Progress",progressMade);
            }
        }
    }

    public string RetrieveAchievementName()
    {
        return achievementName;
    }

    public string RetrieveAchievementDescription()
    {
        return achievementDescription;
    }

    public void SetSortValues(int positionAlphabet, int positionType, int positionCompleted)
    {
        alphabetIndex = positionAlphabet;
        typeIndex = positionType;
        completedIndex = positionCompleted;
    }

    public void SetAlphabetIndex(int positionAlphabet)
    {
        alphabetIndex = positionAlphabet;
    }

    public void SetTypeIndex(int positionType)
    {
        typeIndex = positionType;
    }

    public void SetCompletedIndex(int positionCompleted)
    {
        completedIndex = positionCompleted;
    }


    public int RetrieveIndex(int sortOption)
    {
        if (sortOption == 0)
        {
            return alphabetIndex;
        }
        else if (sortOption == 1)
        {
            return completedIndex;
        }
        else
        {
            return typeIndex;
        }
    }
    public int RetrieveAchievementType()
    {
        return (int)achievementType;
    }

    public int NumberOfAchievementTypes()
    {
        var values = System.Enum.GetValues(typeof(AchievementTypes));
        return values.Length;
    }

    public Sprite RetrieveAchievementIcon()
    {
        return achievementIcon;
    }

    ///<summary>
    ///Call this function when the achievement information should be saved to player prefs
    ///</summary>
    public void SaveAchievementItemInformation()
    {
        int index = achievementID;
        PlayerPrefs.SetString(saveNameForAchievement + index + "Name", achievementName);
        PlayerPrefs.SetInt(saveNameForAchievement + index + "Type", (int)achievementType);
        PlayerPrefs.SetInt(saveNameForAchievement + index + "Check", (int)checkType);
        if (achievementComplete)
        {
            PlayerPrefs.SetInt(saveNameForAchievement + index + "Completed", 1);
        }
        else
        {
            PlayerPrefs.SetInt(saveNameForAchievement + index + "Completed", 0);
        }
        if (achievementType == AchievementTypes.integerValue)
        {
            PlayerPrefs.SetFloat(saveNameForAchievement + index + "ValueInt", distanceForPositionCheck);
        }
        else if (achievementType == AchievementTypes.floatValue)
        {
            PlayerPrefs.SetFloat(saveNameForAchievement + index + "ValueFloat", distanceForPositionCheck);
        }
        else if (achievementType == AchievementTypes.positionValue)
        {
            PlayerPrefs.SetFloat(saveNameForAchievement + index + "ValuePositionX", positionCheckValue.x);
            PlayerPrefs.SetFloat(saveNameForAchievement + index + "ValuePositionY", positionCheckValue.y);
            PlayerPrefs.SetFloat(saveNameForAchievement + index + "ValuePositionZ", positionCheckValue.z);
            PlayerPrefs.SetFloat(saveNameForAchievement + index + "Value", distanceForPositionCheck);
        }
        else if (achievementType == AchievementTypes.stringValue)
        {
            PlayerPrefs.SetString(saveNameForAchievement + index + "ValueString", checkValueText);
        }
        PlayerPrefs.SetString(saveNameForAchievement+index+"ImageLocation",AssetDatabase.GetAssetPath(achievementIcon));
        if (achievementProgressSavedBetweenScenes){
        PlayerPrefs.SetInt("Save"+saveNameForAchievement+"Progress"+index,1);
        } else {
            PlayerPrefs.SetInt("Save"+saveNameForAchievement+"Progress"+index,0);
        }
    }

    public void SetAchievementID (int id,string saveName){
        achievementID = id;
        saveNameForAchievement = saveName;
        CheckAchievementStatus();
    }

    public void SetIconObject (Sprite sprite){
        achievementIcon = sprite;
    }

    ///<summary>
    ///Returns the value of the achievement that was used at the last achievement complete check.
    ///</summary>
    public float RetrieveProgressOnAchievement (){
        if (achievementProgressSavedBetweenScenes){
            if (achievementType == AchievementTypes.integerValue){
            return PlayerPrefs.GetInt(saveNameForAchievement+achievementID+"Progress",-1);
            } else if (achievementType == AchievementTypes.floatValue||achievementType==AchievementTypes.positionValue){
                return PlayerPrefs.GetFloat(saveNameForAchievement+achievementID+"Progress",-1);
            }
        }
        return -1;
    }

    public bool DisplayAchievementProgress (){
        return achievementProgressSavedBetweenScenes;
    }
    public int  TypeOfProgressDisplay (){
        return (int)progressDisplay;
    }

    public float RetrieveAchievementDesiredValue (){
        return distanceForPositionCheck;
    }

    public void SetDisplayAchievementProgressSetting (bool b){
        achievementProgressSavedBetweenScenes = b;
    }

}
}
