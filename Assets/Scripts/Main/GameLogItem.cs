using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogItem : MonoBehaviour
{
    public Text titleText;
    public List<Text> logText;

    public void Init(YuePaiLog info)
    {
        titleText.text = info.curr_round + "/" + info.round + "局";
        for (int i = 0; i < info.YuePaiOther.Count; i++)
        {
            logText[i].text = (info.YuePaiOther[i].score > 0 ? "+" : "") + info.YuePaiOther[i].score;
        }
    }
}
