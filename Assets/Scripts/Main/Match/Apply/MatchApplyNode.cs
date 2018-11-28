using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchApplyNode : Node
{
    public Text titleText;
    public GameObject rulesPanel, rulesCloseBtn;
    [HideInInspector]
    public Text matchRule, finals, tip;
    public MatchApplyQuitPanel quitPanel;
    public MatchApplyLeftPanel leftPanel;
    public MatchApplyRightPanel rightPanel;
    MatcherInfo _data;

    public override void Init()
    {
        base.Init();
        leftPanel.Init(this);
        rightPanel.Init(this);
        rulesPanel.SetActive(false);
        UGUIEventListener.Get(rulesCloseBtn).onClick = (g) => { rulesPanel.SetActive(false); };
    }

    public override void Open()
    {
        base.Open();
        _data = MatchModel.Instance.CurData;
        titleText.text = _data.name;
        leftPanel.Open(_data.rankReard);
        rightPanel.Open();
        quitPanel.gameObject.SetActive(false);
        SetRewardStr();
    }

    private void SetRewardStr()
    {
        List<RankReward> rankReward = MatchModel.Instance.CurData.rankReard;
        string rankStr = "";
        if (rankReward.Count > 3)
        {
            rankStr = rankReward[rankReward.Count - 1].rank;
            int index = rankStr.IndexOf("-");
            rankStr = rankStr.Substring(index + 1);
        }
        else
        {
            int num = rankReward.Count;
            rankStr = num.ToString();
        }
        //比赛类型斗地主1  麻将2
        matchRule.text = string.Format(" 【基本规则】\n比赛采用通用" +
            (_data.type == 1 ? "斗地主" : "明水麻将") + "的游戏规则。\n" +
            "最低满" + _data.minUser + "人开赛，积分排名前" + rankStr +
            "名的玩家可以获得对应排名奖励。");
        if (_data.type == 1)
            finals.text = string.Format("【预赛】\n玩家分数低于淘汰分即被淘汰出局，截止到30人，积分排名前24名的玩家晋级。\n【决赛】\n定局为积分排名淘汰，24进9，9进3，3人争夺冠军，总共3轮9局，根据积分决出冠亚季军。");
        else
            finals.text = string.Format("【预赛】\n玩家分数低于淘汰分即被淘汰出局，截止到24人，积分排名前16名的玩家晋级。\n【决赛】\n定局为积分排名淘汰，16进8，8进4，4人争夺冠军，总共3轮9局，根据积分决出冠亚季军。");

    }

    /// <summary>退出比赛 </summary>
    public void OpenQuitPanel(string matchId)
    {
        quitPanel.Init(matchId);
    }
    /// <summary> 参加成功 并且进入准备界面</summary>
    public static void JoinMatcherFinish(net_protocol.JoinMatcherResp resp)
    {
        if (resp.result == 1)
        {
            SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
            {
                msgid = net_protocol.MessageId.C2G_PrepareMatcher,
                prepareMatcher = new net_protocol.PrepareMatcher
                {
                    matcherId = MatchModel.Instance.CurData.matchId,
                    userId = UserInfoModel.userInfo.userId
                }
            });
        }
        else if (resp.result == 2)
        {
            MatchModel.Instance.CurData = null;
            TipManager.Instance.OpenTip(TipType.SimpleTip, "比赛人数已满，请选择其他比赛");
        }
    }
    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
        leftPanel.Close();
    }
}
