using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchReadyNode : Node
{
    public Text title, distance, applyNum;
    public Text matchRule, finals;
    public Button closeBtn;
    public GameObject AnimPanel;
    public MatchInvitePanel invitePanel;
    public MatchRankShowPanel rankPanel;
    public MatchReadyExitPanel exitPanel;
    public MatchChatPanel chatPanel;
    MatcherInfo _data;
    public override void Init()
    {
        base.Init();
        invitePanel.Init();
        exitPanel.Init();
        rankPanel.Init();
        chatPanel.Init(this);
        UGUIEventListener.Get(closeBtn.gameObject).onClick = delegate { exitPanel.gameObject.SetActive(true); };
    }

    public override void Open()
    {
        base.Open();
        rankPanel.Open();
        exitPanel.Open();
        FloatBallManager.Instance.Hide();
        _data = MatchModel.Instance.CurData;
        title.text = _data.name;
        applyNum.text = _data.joinUser + "/" + _data.minUser;
        StartCoroutine(UpMyTime());
        chatPanel.SendMessage();
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
    IEnumerator UpMyTime()
    {
        while (_data.distance > 0)
        {
            distance.text = MatchPage.GetTimerText(_data.distance, 1);
            _data.distance--;
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    public MatcherInfo GetData()
    {
        return _data;
    }
    /// <summary> 加入人数</summary>
    public static void JoinCountFlush(net_protocol.MatcherJoinCount resp)
    {
        var node = NodeManager.GetNode<MatchReadyNode>();
        if (node)
        {
            MatchModel.Instance.CurData.joinUser = resp.joinNum;
            node.applyNum.text = string.Format(resp.joinNum + "/" + node._data.maxUser);
        }
    }
    /// <summary> 聊天 </summary>
    public static void MatcherChat(net_protocol.MatcherChatResp resp)
    {
        var node = NodeManager.GetNode<MatchReadyNode>();
        if (node)
            node.chatPanel.AnalysisMessage(resp);
    }
    /// <summary>好友列表 </summary>
    public static void InviteFriend(net_protocol.MatcherFriendResp resp)
    {
        var node = NodeManager.GetNode<MatchReadyNode>();
        if (node)
            node.invitePanel.CreateItem(resp);
    }
    /// <summary>开始比赛信息 </summary>
    public static void StartMatch()
    {
        var node = NodeManager.GetNode<MatchReadyNode>();
        if (node)
            node.AnimPanel.SetActive(true);
    }
    /// <summary> 退出游戏</summary>
    public void QuitMatcher()
    {
       MatchModel.Instance.CurData = null;
       this.Close();
    }
    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
        SetNode.FloatBall();
        chatPanel.Close();
        rankPanel.Close();
        invitePanel.Close();
    }
}
