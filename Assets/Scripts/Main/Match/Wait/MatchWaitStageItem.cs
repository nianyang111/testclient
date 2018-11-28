using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchWaitStageItem : MonoBehaviour
{
    public GameObject select, winnerLog;
    public RectTransform itemTransform;
    public Transform show;
    public Text title;
    public int staNum;
    public int index;
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(int stangeNum, string tit, bool isWin)
    {
        staNum = stangeNum;
        title.text = tit;
        winnerLog.SetActive(isWin);
        select.SetActive(false);
    }

    public void Select()
    {
        select.SetActive(true);
    }
}
