using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchWaitTopPanel : MonoBehaviour {
    public Text title, wait;
    public Image waitIcon;

    public void FlushData(net_protocol.WaitMatcherRiseResp resp)
    {
        title.text = resp.matcherName;
        wait.text=string.Format("当前还有<color=#00FF01FF>"+resp.waitTableNum+"</color>桌正在比赛，请稍后");
        //waitIcon.sprite=BundleManager.
    }

    public void Init()
    {
        waitIcon.sprite = BundleManager.Instance.GetSprite("Match/match_word_1");
    }
}