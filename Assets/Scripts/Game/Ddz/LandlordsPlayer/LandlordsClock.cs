using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LandlordsClock : MonoBehaviour {

    /// <summary>
    /// 总计时
    /// </summary>
    private float allTime;
    /// <summary>
    /// 提示时间
    /// </summary>
    private float tipsTime;
    /// <summary>
    /// 震动时间
    /// </summary>
    private float zhendongTime;
    /// <summary>
    /// 是否震动
    /// </summary>
    private bool isZhendong;
    /// <summary>
    /// 剩余计时
    /// </summary>
    private float remain;

    private float timer;

    public SequenceAnimation ani;
    public Text timeLb;
    private CallBack onTimeEndCall;
    private AudioSource audio;
    void Awake()
    {
        timeLb = transform.Find("value").GetComponent<Text>();
        ani = GetComponent<SequenceAnimation>();
    }

    public void Init(float allTime, float tipsTime, float zhendongTime, CallBack _onTimeEndCall = null, bool isZhendong = false)
    {
        onTimeEndCall = _onTimeEndCall;
        this.allTime = allTime;
        this.tipsTime = tipsTime;
        this.zhendongTime = zhendongTime;
        this.isZhendong = isZhendong;
        gameObject.SetActive(true);
        remain = allTime;
        CancelInvoke();
        if (ani.IsPlaying)
            ani.Stop();
        InvokeRepeating("Timer", 0, 1);
    }

    void Timer()
    {
        remain -= 1;
        timeLb.text = remain.ToString();
        if (remain == tipsTime)
        {
            TimerCallBack();
        }
        if (remain == zhendongTime && isZhendong)
        {
            if (SetNode.shock == 1)
                HandheldManager.Instance.Vibrate(zhendongTime, 1);
        }
        if (remain <= 0)
        {
            OnEnd();
        }
    }


    void TimerCallBack()
    {
        ani.Play();
        audio = AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.clock);
    }

    void OnEnd()
    {
        if (onTimeEndCall != null)
        {
            onTimeEndCall();
        }
        Clear();
    }

    void Clear()
    {
        gameObject.SetActive(false);
        if (ani != null && ani.IsPlaying)
            ani.Stop();
        CancelInvoke();
        onTimeEndCall = null;
        if (audio != null)
            audio.Stop();
        HandheldManager.Instance.Close();
    }

    void OnDisable()
    {
        Clear();
    }

    public void Close()
    {        
        Clear();  
    }
}
