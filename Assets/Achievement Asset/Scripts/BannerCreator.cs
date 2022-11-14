using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AchievementAsset{
public class BannerCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject banner, flashEffect;
    [SerializeField]
    private float bannerLifeTime = 2f;
    private enum SpawnPosition
    {
        TopLeftCorner,
        TopMiddle,
        TopRightCorner,
        LeftMiddleSide,
        Middle,
        RightMiddleSide,
        BottomLeftCorner,
        BottomMiddle,
        BottomRightCorner
    }
    private enum BannerEffect
    {
        None,
        FadeIn,
        SlideIn,
        Flashing,
    }
    [SerializeField]
    private SpawnPosition spawnPosition = SpawnPosition.TopLeftCorner;
    [SerializeField]
    private BannerEffect effect = BannerEffect.None;
    private Vector3 finalPosition;
    private Vector3 startPosition;
    [SerializeField]
    private float screenEdgeDistance = 10;
    [SerializeField]
    private Text bodyText;
    private Sprite defaultSprite;
    private IEnumerator currentBanner;
    private Stack<string> bannerQueue = new Stack<string>();
    private Stack<Sprite> bannerIconQueue = new Stack<Sprite>();

    ///<summary>
    ///To create a new banner call this function. If one already is active it creates adds the information to a queue
    ///</summary>
    public void CreateNewBanner(string title, Sprite achievementIcon)
    {

        if (currentBanner == null)
        {
            banner.SetActive(true);
            if (finalPosition == startPosition)
            {
                DeterminePositions();
                banner.transform.position = finalPosition;
            }
            if (achievementIcon != null)
            {
                if (defaultSprite == null)
                {
                    defaultSprite = banner.transform.GetChild(0).GetComponent<Image>().sprite;
                }
                banner.transform.GetChild(0).GetComponent<Image>().sprite = achievementIcon;
            }
            else if (defaultSprite != null)
            {
                banner.transform.GetChild(0).GetComponent<Image>().sprite = defaultSprite;
            }
            
            if (bodyText!=null){
                bodyText.text = title;
            } else {
                Text text = banner.transform.GetChild(3).GetComponent<Text>();
                text.text = title;
            }
            StartBannerEffect(true);
        }
        else
        {
            AddBannerToQueue(title, achievementIcon);
        }
    }

    void AddBannerToQueue(string text, Sprite achievementIcon)
    {
        bannerQueue.Push(text);
        bannerIconQueue.Push(achievementIcon);
    }

    private void DeterminePositions()
    {
        float xResolution = Screen.width;
        float yResolution = Screen.height;
        float xfinalScreenPosition = DetermineXPosition(xResolution);
        float yfinalScreenPosition = DetermineYPosition(yResolution);
        float startPositionX = 0;
        float startPositionY = 0;
        if (xfinalScreenPosition > xResolution / 2f)
        {
            startPositionX = xResolution;
        }
        else if (xfinalScreenPosition == xResolution / 2f)
        {
            startPositionX = xResolution / 2f;
        }
        if (yfinalScreenPosition > yResolution / 2f)
        {
            startPositionY = yResolution;
        }
        else if (yfinalScreenPosition == yResolution / 2f)
        {
            startPositionY = yResolution / 2f;
        }
        finalPosition = new Vector3(xfinalScreenPosition, yfinalScreenPosition, 0);
        startPosition = new Vector3(startPositionX, startPositionY, 0);

    }

    private float DetermineXPosition(float resolutionX)
    {
        float finalXPosition = resolutionX;
        if (spawnPosition == SpawnPosition.TopLeftCorner || spawnPosition == SpawnPosition.LeftMiddleSide || spawnPosition == SpawnPosition.BottomLeftCorner)
        {
            finalXPosition = (resolutionX * (screenEdgeDistance / 100f));
            return finalXPosition;
        }
        else if (spawnPosition == SpawnPosition.BottomMiddle || spawnPosition == SpawnPosition.Middle || spawnPosition == SpawnPosition.TopMiddle)
        {
            finalXPosition = (resolutionX * 0.5f);
            return finalXPosition;
        }
        else
        {
            finalXPosition = (resolutionX * ((100f - screenEdgeDistance) / 100f));
            return finalXPosition;
        }
    }

    private float DetermineYPosition(float resolutionY)
    {
        float finalYPosition = resolutionY;
        if (spawnPosition == SpawnPosition.BottomMiddle || spawnPosition == SpawnPosition.BottomRightCorner || spawnPosition == SpawnPosition.BottomLeftCorner)
        {
            finalYPosition = (resolutionY * (screenEdgeDistance / 100f));
            return finalYPosition;
        }
        else if (spawnPosition == SpawnPosition.TopMiddle || spawnPosition == SpawnPosition.TopLeftCorner || spawnPosition == SpawnPosition.TopRightCorner)
        {
            finalYPosition = (resolutionY * ((100f - screenEdgeDistance) / 100f));
            return finalYPosition;
        }
        else
        {
            finalYPosition = (resolutionY * 0.5f);

            return finalYPosition;
        }
    }

    IEnumerator StartFadeInEffect(float duration, float desiredAlphaValue, bool closing)
    {

        Image image = banner.GetComponent<Image>();
        //image.color.a = 1f-desiredAlphaValue;
        Color color = image.color;
        color = new Color(color.r, color.g, color.b, 1f - desiredAlphaValue);
        image.color = color;
        float rateOfChange = 1f / duration;

        if (desiredAlphaValue == 0)
        {
            rateOfChange *= -1;
        }
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            image.color += new Color(0, 0, 0, rateOfChange * Time.deltaTime);
            yield return new WaitForSeconds(0);
        }
        image.color = new Color(color.r, color.g, color.b, desiredAlphaValue); ;
        if (closing)
        {
            currentBanner = null;
            CheckQueue();
        }
        else
        {
            currentBanner = BannerAlive();
            StartCoroutine(currentBanner);
        }
    }


    IEnumerator StartSlideInEffect(float duration, Vector3 desiredPosition, bool closing)
    {
        float distanceX = (banner.transform.position.x - desiredPosition.x);
        float distanceY = (banner.transform.position.y - desiredPosition.y);
        float speedX = distanceX / duration;
        float speedY = distanceY / duration;
        float xDirection = -1;
        float yDirection = -1;
        Vector3 speedVector = new Vector3(xDirection * speedX, yDirection * speedY, 0);
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            banner.transform.position += speedVector * Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        if (closing)
        {
            currentBanner = null;
            CheckQueue();
        }
        else
        {
            currentBanner = BannerAlive();
            StartCoroutine(currentBanner);
        }
    }


    IEnumerator StartFlashingEffect(float duration, int numberOfFlashes, bool closing)
    {
        float durationForFlash = duration / (float)numberOfFlashes;
        bool isFlashOn = false;
        int numberOfCompletedFlashes = 0;

        while (numberOfCompletedFlashes < numberOfFlashes||isFlashOn)
        {
            if (!isFlashOn)
            {
                numberOfCompletedFlashes++;
                isFlashOn = true;
                flashEffect.SetActive(true);
            }
            else
            {
                isFlashOn = false;
                flashEffect.SetActive(false);
            }
            yield return new WaitForSeconds(durationForFlash);
        }

        if (closing)
        {
            currentBanner = null;
            CheckQueue();
        }
        else
        {
            currentBanner = BannerAlive();
            StartCoroutine(currentBanner);
        }
    }

    IEnumerator BannerAlive()
    {
        banner.transform.position = finalPosition;
        yield return new WaitForSeconds(bannerLifeTime);
        StartBannerEffect(false);
    }

    void StartBannerEffect(bool isFadeInEffect)
    {
        currentBanner = null;
        if (effect == BannerEffect.None)
        {
            if (isFadeInEffect)
            {
                currentBanner = BannerAlive();
            }
        }
        else if (effect == BannerEffect.FadeIn)
        {
            if (isFadeInEffect)
            {
                currentBanner = StartFadeInEffect(1f, 1f, false);
            }
            else
            {
                currentBanner = StartFadeInEffect(1f, 0f, true);
            }
        }
        else if (effect == BannerEffect.SlideIn)
        {
            if (isFadeInEffect)
            {
                banner.transform.position = startPosition;
                currentBanner = StartSlideInEffect(1f, finalPosition, false);
            }
            else
            {
                currentBanner = StartSlideInEffect(1f, startPosition, true);
            }
        }
        else
        {
            if (isFadeInEffect)
            {
                currentBanner = StartFlashingEffect(0.5f, 3, false);
            }
            else
            {
                currentBanner = StartFlashingEffect(0.5f, 3, true);
            }
        }
        if (currentBanner != null)
        {
            StartCoroutine(currentBanner);
        }
        else
        {
            CheckQueue();
        }
    }

    void CheckQueue()
    {
        banner.SetActive(false);
        if (bannerQueue.Count > 0)
        {
            string nextObject = bannerQueue.Pop();
            Sprite icon = bannerIconQueue.Pop();
            CreateNewBanner(nextObject, icon);
        }
    }

}
}