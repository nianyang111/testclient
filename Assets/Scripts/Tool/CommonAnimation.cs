using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CommonAnimation : MonoBehaviour
{
    public bool point, angle, scale, color, alpha, size;
    public UnityAction pointEndAction, angleEndAction, scaleEndAction, colorEndAction, alphaEndAction, sizeEndAction;//结束后的执行的函数
    public List<Vector3> pointList = new List<Vector3>(), angleList = new List<Vector3>(), scaleList = new List<Vector3>();
    public List<Color> colorList = new List<Color>();
    public List<float> alphaList = new List<float>();
    public List<Vector2> sizeList = new List<Vector2>();
    public float pointDelayTime, angleDelayTime, scaleDelayTime, colorDelayTime, alphaDelayTime, sizeDelayTime;
    public float time = 1;
    public Space space = Space.Self;
    public bool isLoop, isPingPong, isAutoPlay, isDisappear, isPause, isStop;
    public DisappearType disType;
    static WaitForSecondsRealtime wfsrPause;
    static WaitForEndOfFrame wfef;

    void Start()
    {
        if (wfsrPause == null)
            wfsrPause = new WaitForSecondsRealtime(0.1f);
        if (wfef == null)
            wfef = new WaitForEndOfFrame();
    }

    public void Play()
    {
        Stop();
        if (pointList != null && (pointList.Count > 2 || (pointList.Count == 2 && pointList[0] != pointList[1])))
            StartCoroutine(PlayAnimation(1, pointDelayTime, time, space));

        if (angleList != null && (angleList.Count > 2 || (angleList.Count == 2 && angleList[0] != angleList[1])))
            StartCoroutine(PlayAnimation(2, angleDelayTime, time));

        if (scaleList != null && (scaleList.Count > 2 || (scaleList.Count == 2 && scaleList[0] != scaleList[1])))
            StartCoroutine(PlayAnimation(3, scaleDelayTime, time));

        if (colorList != null && (colorList.Count > 2 || (colorList.Count == 2 && colorList[0] != colorList[1])))
            StartCoroutine(PlayAnimation(4, colorDelayTime, time));

        if (alphaList != null && (alphaList.Count > 2 || (alphaList.Count == 2 && alphaList[0] != alphaList[1])))
            StartCoroutine(PlayAnimation(5, alphaDelayTime, time));

        if (sizeList != null && (sizeList.Count > 2 || (sizeList.Count == 2 && sizeList[0] != sizeList[1])))
        {
            RectTransform rtf = GetComponent<RectTransform>();
            if (!rtf)
                rtf = gameObject.AddComponent<RectTransform>();
            StartCoroutine(PlayAnimation(6, sizeDelayTime, time));
        }
    }

    IEnumerator PlayAnimation(int type, float delayTime, float time, Space space = Space.Self, int startIndex = 0, float progress = 0)
    {
        if (delayTime != 0)
            yield return new WaitForSecondsRealtime(delayTime);
        Init(type);
        float speed = 0, spaceTime = 0.01f;
        bool hadCanvas, isFoward = true;
        int count = 0;
        CanvasGroup cg = GetComponent<CanvasGroup>();
        hadCanvas = cg;
        #region ....根据类型，计算变化速度
        switch (type)
        {
            case 1:
                speed = (pointList.Count - 1) * spaceTime / time;
                break;
            case 2:
                speed = (angleList.Count - 1) * spaceTime / time;
                break;
            case 3:
                speed = (scaleList.Count - 1) * spaceTime / time;
                break;
            case 4:
                speed = (colorList.Count - 1) * spaceTime / time;
                break;
            case 5:
                speed = (alphaList.Count - 1) * spaceTime / time;
                break;
            case 6:
                speed = (sizeList.Count - 1) * spaceTime / time;
                break;
        }
        #endregion
        speed *= 2;
        while (isFoward ? progress < 1 : progress > 0)
        {
            if (isStop)
            {
                Init(type);
                break;
            }
            yield return wfef;
            if (isPause)
            {
                yield return wfsrPause;
                continue;
            }

            if (isFoward)
            {
                progress += speed;
                if (progress > 1)
                    progress = 1;
            }
            else
            {
                progress -= speed;
                if (progress < 0)
                    progress = 0;
            }
            #region ...根据类型，执行不同的渐变方式
            switch (type)
            {
                case 1:
                    #region ...坐标变化
                    if (count == 0)
                        count = pointList.Count;
                    if (space == Space.Self)
                        transform.localPosition = Vector3.Lerp(pointList[startIndex], pointList[startIndex + 1], progress);
                    else
                        transform.position = Vector3.Lerp(pointList[startIndex], pointList[startIndex + 1], progress);
                    break;
                    #endregion
                case 2:
                    #region ...角度变化
                    if (count == 0)
                        count = angleList.Count;
                    transform.localEulerAngles = Vector3.Lerp(angleList[startIndex], angleList[startIndex + 1], progress);
                    break;
                    #endregion
                case 3:
                    #region ...比例变化
                    if (count == 0)
                        count = scaleList.Count;
                    transform.localScale = Vector3.Lerp(scaleList[startIndex], scaleList[startIndex + 1], progress);
                    break;
                    #endregion
                case 4:
                    #region ...颜色变化
                    if (count == 0)
                        count = colorList.Count;
                    if (GetComponent<Image>())
                        GetComponent<Image>().color = Color.Lerp(colorList[startIndex], colorList[startIndex + 1], progress);
                    else if (GetComponent<Text>())
                        GetComponent<Text>().color = Color.Lerp(colorList[startIndex], colorList[startIndex + 1], progress);
                    break;
                    #endregion
                case 5:
                    #region ...透明度变化
                    if (count == 0)
                        count = alphaList.Count;
                    if (!hadCanvas && !cg)
                    {
                        cg = gameObject.AddComponent<CanvasGroup>();
                        cg.interactable = false;
                        cg.blocksRaycasts = false;
                    }
                    cg.alpha = Mathf.Lerp(alphaList[startIndex], alphaList[startIndex + 1], progress);
                    break;
                    #endregion
                case 6:
                    #region ...尺寸变化
                    if (count == 0)
                        count = sizeList.Count;
                    GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(sizeList[startIndex], sizeList[startIndex + 1], progress);
                    break;
                    #endregion
            }
            #endregion

            if (isFoward ? progress >= 1 : progress <= 0)
            {
                progress = isFoward ? 0 : 1;
                if (isFoward ? startIndex + 2 < count : startIndex > 1)
                    startIndex += isFoward ? 1 : -1;
                else
                    if (isLoop)
                    {
                        if (isPingPong)
                        {
                            isFoward = !isFoward;
                            startIndex = isFoward ? 0 : count - 2;
                            progress = isFoward ? 0 : 1;
                        }
                        else
                            startIndex = 0;
                    }
                    else
                    {
                        progress = 1;
                        #region ...回调
                        switch (type)
                        {
                            case 1:
                                if (pointEndAction != null)
                                    pointEndAction();
                                break;
                            case 2:
                                if (angleEndAction != null)
                                    angleEndAction();
                                break;
                            case 3:
                                if (scaleEndAction != null)
                                    scaleEndAction();
                                break;
                            case 4:
                                if (colorEndAction != null)
                                    colorEndAction();
                                break;
                            case 5:
                                if (!hadCanvas)
                                    Destroy(cg);
                                if (alphaEndAction != null)
                                    alphaEndAction();
                                break;
                            case 6:
                                if (sizeEndAction != null)
                                    sizeEndAction();
                                break;
                        }
                        #endregion

                        if (isDisappear)
                            if (disType == DisappearType.Destroy)
                                Destroy(gameObject);
                            else
                                gameObject.SetActive(false);
                    }
            }
        }
    }

    void Init(int type)
    {
        switch (type)
        {
            case 1:
                #region ...坐标变化
                if (space == Space.Self)
                    transform.localPosition = pointList[0];
                else
                    transform.position = pointList[0];
                break;
                #endregion
            case 2:
                #region ...角度变化
                transform.localEulerAngles = angleList[0];
                break;
                #endregion
            case 3:
                #region ...尺寸变化
                transform.localScale = scaleList[0];
                break;
                #endregion
            case 4:
                #region ...颜色变化
                if (GetComponent<Image>())
                    GetComponent<Image>().color = colorList[0];
                else if (GetComponent<Text>())
                    GetComponent<Text>().color = colorList[0];
                break;
                #endregion
            case 5:
                #region ...透明度变化
                CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
                if (!cg)
                {
                    cg = gameObject.AddComponent<CanvasGroup>();
                    cg.interactable = false;
                    cg.blocksRaycasts = false;
                }
                cg.alpha = alphaList[0];
                break;
                #endregion
        }
        isStop = false;
        isPause = false;
    }

    void OnEnable()
    {
        if (isAutoPlay)
            Play();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Continue()
    {
        isPause = false;
    }

    public void Stop()
    {
        isStop = true;
    }

    public void Clear()
    {
        pointEndAction = angleEndAction = scaleEndAction = colorEndAction = alphaEndAction = sizeEndAction = null;
        pointList.Clear();
        angleList.Clear();
        scaleList.Clear();
        colorList.Clear();
        alphaList.Clear();
        sizeList.Clear();
        pointDelayTime = angleDelayTime = scaleDelayTime = colorDelayTime = alphaDelayTime = sizeDelayTime = 0;
        time = 1;
        isLoop = isAutoPlay = isDisappear = isPause = isStop = false;
        disType = DisappearType.Destroy;
    }

    /// <summary>
    /// 消失方式
    /// </summary>
    public enum DisappearType
    {
        Destroy,
        Disable
    }
}