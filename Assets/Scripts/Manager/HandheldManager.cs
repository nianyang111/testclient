using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandheldManager : MonoBehaviour
{

    #region instance
    static HandheldManager _instance;
    public static HandheldManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject();
                _instance = go.AddComponent<HandheldManager>();
                go.name = _instance.GetType().ToString();
            }
            return _instance;
        }
    }

    void OnDestory()
    {
        _instance = null;
    }
    #endregion

    float interval = 0;
    float timer = 0;

    /// <summary>
    /// 震动
    /// </summary>
    /// <param name="timer">震动总时间</param>
    /// <param name="interval">震动间隔/频率</param>
    public void Vibrate(float timer, float interval)
    {
        this.timer = timer;
        this.interval = interval;
        StartCoroutine(vibrator());
    }

    /// <summary>
    /// 关闭震动
    /// </summary>
    public void Close()
    {
        timer = 0;
        interval = 0;
    }

    IEnumerator vibrator()
    {
        while (timer > 0)
        {
            Handheld.Vibrate();
            yield return new WaitForSecondsRealtime(interval);
            timer -= interval;
        }
    }
}
