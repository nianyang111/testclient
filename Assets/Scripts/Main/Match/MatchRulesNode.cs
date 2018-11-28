using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MatchRulesNode : Node
{
    public Text matchNameText, stageText, baseScoreText, ruleText, desText;

    public static void DisplayMatchInfo(CurrentMatcherInfoResp resp = null)
    {
        MatchRulesNode min = NodeManager.OpenNode<MatchRulesNode>(null, null, false, false);
        if (resp != null)
            min.RefreshData(resp);
        min.gameObject.SetActive(resp == null);
    }

    void RefreshData(CurrentMatcherInfoResp resp)
    {
        matchNameText.text = resp.matcherName;
        stageText.text = resp.stage;
        baseScoreText.text = resp.baseScore.ToString();
        if (PageManager.Instance.CurrentPage is LandlordsPage)
            LandlordsPage.Instance.Dizhu = LandlordsModel.Instance.RoomModel.CurRoomInfo.LeastStore = resp.baseScore;
        ruleText.text = resp.rule;
        //bisaiDesLb.text = string.Format("剩余{0}人时{1}截止,前{2}人晋级", resp.waitRiseCount, resp.rule, resp.riseCount);
        if (resp.riseCount > 0)
            desText.text = string.Format("{0}人时截止,前{1}人晋级", resp.waitRiseCount, resp.riseCount);
        else
            desText.text = string.Format("{0}局结束后，根据积分定排名。", resp.rule);
        if (PageManager.Instance.CurrentPage is LandlordsPage)
            LandlordsNet.BisaiInfoResp(resp);
    }
}
