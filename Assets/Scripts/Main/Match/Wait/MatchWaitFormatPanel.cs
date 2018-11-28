using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 赛制面板
/// </summary>
public class MatchWaitFormatPanel : MonoBehaviour {
    public Text curText,nextText;

    public void SetValue(string cur,string next)
    {
        curText.text = cur;
        nextText.text = next;
    }
}
