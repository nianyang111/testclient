using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Events;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
    public class TimerAction
    {
        /// <summary>
        /// 时间
        /// </summary>
        public int timer;
        /// <summary>
        /// 对应时间回调
        /// </summary>
        public UnityAction action;
    }

    public bool isAutoPlay = false;
    /// <summary>
    /// 时间总长度
    /// </summary>
    public float allLength = 10;
    /// <summary>
    /// 刷新间隔
    /// 小于0为倒计时，大于0为正计时
    /// </summary>
    public float refreshSpace = -1;
    /// <summary>
    /// 时间文本格式 
    /// 1-00:00:00, 2-00:00, 3-00
    /// </summary>
    public int timeFormat = 3;
    /// <summary>
    /// 是否只执行一次
    /// </summary>
    public bool isOnce = false;
    /// <summary>
    /// 计时器回调
    /// </summary>
    List<TimerAction> timerActions = new List<TimerAction>();
    /// <summary>
    /// 计时器结束后执行的回调函数
    /// </summary>
    public UnityAction endAction = null;
    /// <summary>
    /// 当前时间
    /// </summary>
    public float currentTime;
    Text timerText;
    bool isEnable = false;
    bool isBegin = false;
    bool isTiming = false;

    int hours = 0;
    int minutes = 0;
    int seconds = 0;

    #region ...计时器主体
    void Awake()
    {
        if (!timerText)
        {
            Text temp = GetComponent<Text>();
            if (temp)
                timerText = temp;
        }
    }

    void OnEnable()
    {
        if (isAutoPlay)
            ResetTimer(true);
        isEnable = true;
    }

    void InitTimer(float allLengthPram = 0)
    {
        if (allLengthPram != 0)
            allLength = allLengthPram;
        if (Mathf.Abs(refreshSpace) > this.allLength)
            refreshSpace = refreshSpace > 0 ? allLength : -allLength;
        currentTime = refreshSpace > 0 ? 0 : allLength;
        SetTimeText(GetTimerText(currentTime, timeFormat));
    }

    /// <summary>
    /// 计时器主体(需优化)
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerBody()
    {
        float currentWaitTime = 0;
        while (currentWaitTime < Mathf.Abs(refreshSpace))
        {
            while (!isTiming && isEnable)
                yield return new WaitForEndOfFrame();
            if (!isEnable)
                break;
            yield return new WaitForEndOfFrame();
            currentWaitTime += Time.deltaTime;
        }
        if (isEnable)
        {
            currentTime += refreshSpace;
            SetTimeText(GetTimerText(currentTime, timeFormat));
            if (refreshSpace > 0 ? currentTime < allLength : currentTime > 0)
                StartCoroutine(TimerBody());
            else
            {
                if (!isOnce)
                    ResetTimer(true);
                else
                    CloseTimer();
                if (endAction != null)
                    endAction();
            }
        }
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    /// <param name="isStart">是否立即开始计时</param>
    /// <param name="allLength">计时器长度，为0则为上一次设置的值</param>
    public void ResetTimer(bool isStartTime = false, float allLength = 0)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        StartCoroutine(ResetTimerAc(isStartTime, allLength));
    }

    IEnumerator ResetTimerAc(bool isStartTime, float allLength)
    {
        CloseTimer();
        yield return new WaitForEndOfFrame();
        if (isStartTime)
            StartTime();
        else
            InitTimer(allLength);
    }
    
    public void AddAction(int actionTime,UnityAction action)
    {
        timerActions.Add(new TimerAction() { timer = actionTime, action = action });
    }

    public void ClearAction()
    {
        timerActions.Clear();
    }

    /// <summary>
    /// 开始计时
    /// </summary>
    public void StartTime()
    {
        if (!isBegin)
        {
            if (!isEnable)
                isEnable = true;
            InitTimer();
            isBegin = true;
            isTiming = true;
            StartCoroutine(TimerBody());
        }
    }

    /// <summary>
    /// 暂停计时
    /// </summary>
    public void PauseTime()
    {
        isTiming = false;
    }

    /// <summary>
    /// 继续计时（取消暂停）
    /// </summary>
    public void ContinueTime()
    {
        isTiming = true;
    }

    /// <summary>
    /// 关闭计时器
    /// </summary>
    public void CloseTimer()
    {
        isBegin = false;
        isEnable = false;
    }

    /// <summary>
    /// 销毁计时器
    /// </summary>
    public void DestroyTimer()
    {
        CloseTimer();
        Destroy(this);
    }

    string GetTimerText(float time, int timeFormat)
    {
        for (int i = 0; i < timerActions.Count; i++)
        {
            if(timerActions[i].timer==time)
            {
                timerActions[i].action();
                timerActions.Remove(timerActions[i]);
                i--;
            }
        }

        StringBuilder sb = new StringBuilder();
        seconds = (int)time % 60;
        hours = time >= 3600 ? (int)time / 3600 : 0;
        minutes = (int)(time - hours * 3600 - seconds) / 60;
        switch (timeFormat)
        {
            case 1:
                sb.Append(GetTimeText(hours) + ":" + GetTimeText(minutes) + ":" + GetTimeText(seconds));
                break;
            case 2:
                sb.Append(GetTimeText(minutes) + ":" + GetTimeText(seconds));
                break;
            case 3:
                sb.Append(GetTimeText(seconds));
                break;
        }
        return sb.ToString();
    }

    string GetTimeText(int time)
    {
        if (time < 10)
            return "0" + time;
        else
            return time.ToString();
    }

    void SetTimeText(string time)
    {
        if (timerText)
            timerText.text = time;
    }

    void OnDisable()
    {
        CloseTimer();
    }
    #endregion

    /// <summary>
    /// 创建计时器
    /// </summary>
    /// <param name="target">计时器组件绑定的物体</param>
    /// <param name="allLength">计时器长度</param>
    /// <param name="timeFormat">计时器格式 1-00:00:00, 2-00:00, 3-00</param>
    /// <param name="refreshSpace">计时器刷新间隔</param>
    /// <param name="isOnce">计时器是否只执行一次</param>
    /// <param name="action">计时器结束时的回调</param>
    public static Timer CreateTimer(GameObject target, float allLength = 10, int timeFormat = 3, int refreshSpace = -1, bool isOnce = false, UnityAction action = null)
    {
        Timer timer = target.GetComponent<Timer>();
        if (!timer)
            timer = target.AddComponent<Timer>();
        timer.allLength = allLength;
        timer.refreshSpace = refreshSpace;
        timer.timeFormat = timeFormat;
        timer.isOnce = isOnce;
        timer.endAction = action;
        timer.currentTime = refreshSpace > 0 ? 0 : allLength;
        timer.timerText = timer.GetComponent<Text>();
        if (timer.timerText)
            timer.timerText.text = timer.GetTimerText(timer.currentTime, timeFormat);
        return timer;
    }

    public static Timer CreateTimer(GameObject target, float allLength = 10, UnityAction action = null)
    {
        Timer timer = target.GetComponent<Timer>();
        if (!timer)
            timer = target.AddComponent<Timer>();
        timer.allLength = allLength;
        timer.isOnce = true;
        timer.endAction = action;
        return timer;
    }

    public static void AddTimerAction(Timer timer,int actionTime,UnityAction action)
    {
        timer.AddAction(actionTime, action);
    }

    public static void ClearTimerAction(Timer timer)
    {
        timer.ClearAction();
    }
}
