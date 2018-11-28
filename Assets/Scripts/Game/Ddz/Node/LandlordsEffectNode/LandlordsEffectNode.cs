using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandlordsEffectNode : Node
{

    /// <summary>
    /// 动画队列
    /// </summary>
    private List<AniInfo> aniList = new List<AniInfo>();
    /// <summary>
    /// 是否正在播放一个动画
    /// </summary>
    private bool isPlaying;

    GameObject aniParentObj;
    SequenceAnimation curAnis;



    public void Inits(LandlordsEventAniType aniType, float start_Time, float end_delyTime, string triggerPlayerHeadIcon = "")//特效触发者头像
    {
        //加入队列
        AniInfo info = new AniInfo(aniType, start_Time, end_delyTime, triggerPlayerHeadIcon);
        aniList.Add(info);

        if (!isPlaying)
            StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        isPlaying = true;
        yield return new WaitForSecondsRealtime(aniList[0].start_Time);

        Open();
        aniParentObj = transform.Find(aniList[0].aniType.ToString()).gameObject;
        curAnis = aniParentObj.GetComponentInChildren<SequenceAnimation>();
        aniParentObj.SetActive(true);

        LoadIcon();

        curAnis.onPlayEndCall = OnPlayEnd;
        curAnis.Rewind();
        aniList[0].PlaySound();
    }

    /// <summary>
    /// 加载头像图片
    /// </summary>
    void LoadIcon()
    {
        if (aniList[0].userIcon != "")
        {
            Image headIcon = UIUtils.FindHideChildGameObject<Image>(aniParentObj, "headIcon");
            if (headIcon)
            {
                StartCoroutine(MiscUtils.DownloadImage(aniList[0].userIcon, spr =>
                {
                    headIcon.sprite = spr;
                }));
            }
        }
    }

    /// <summary>
    /// 当播放结束
    /// </summary>
    void OnPlayEnd()
    {
        ClearCur();
        aniList.RemoveAt(0);
        if (aniList.Count > 0)
        {
            StartCoroutine(Play());
        }
        else
            gameObject.SetActive(false);
    }

    /// <summary>
    /// 清除当前
    /// </summary>
    void ClearCur()
    {
        if (curAnis != null)
        {
            //curAnis.Stop();
            curAnis = null;
        }
        if (aniParentObj != null)
        {
            aniParentObj.SetActive(false);
            aniParentObj = null;
        }       
        isPlaying = false;
    }

    public override void Close(bool isOpenLast = true)
    {
        ClearCur();
        aniList.Clear();
        base.Close(isOpenLast);
    }
}




/// <summary>
/// 动画
/// </summary>
public class AniInfo
{
    /// <summary>
    /// 动画触发者
    /// </summary>
    public string userIcon;
    /// <summary>
    /// 动画类型
    /// </summary>
    public Enum aniType;
    /// <summary>
    /// 几秒后开始播放
    /// </summary>
    public float start_Time = 0;
    /// <summary>
    /// 播放结束后延迟几秒关闭
    /// </summary>
    public float end_delyTime = 0;
    

    public AniInfo(Enum aniType, float startTime, float enddelyTime,string userIcon)
    {
        this.userIcon = userIcon;
        this.aniType = aniType;
        this.start_Time = startTime;
        this.end_delyTime = enddelyTime;
    }

    public void PlaySound()
    {
        if ((LandlordsEventAniType)aniType != LandlordsEventAniType.ChunTian || (LandlordsEventAniType)aniType != LandlordsEventAniType.FanChunTian)
        {
            if ((LandlordsEventAniType)aniType == LandlordsEventAniType.DoubleStraight)
                aniType = LandlordsEventAniType.Straight;
            AudioManager.AudioSoundType soundType = GameTool.StrToEnum<AudioManager.AudioSoundType>(aniType.ToString());
            AudioManager.Instance.PlayTempSound(soundType, PageManager.Instance.CurrentPage.name);
        }
    }
}
