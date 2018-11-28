using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchRankRewardNode : Node {
    public GameObject rankBtn,rewardBtn;
    public GameObject rankPanel,rewardPanel;
    //玩家自己显示
    public MatcherRankItem myRankInfo;
    public MatcherRankItem rankPrefab;
    public MatcherRewardItem rewardPrefab;
    public Transform rankContent;
    public Transform rewardContent;
    private List<MatcherRankItem> rankItemList = new List<MatcherRankItem>();
    private List<MatcherRewardItem> rewardItemList = new List<MatcherRewardItem>();
    public string matchId;
    public override void Init()
    {
        base.Init();
        UIUtils.DestroyChildren(rewardContent);
        var data = MatchModel.Instance.CurData;
        if (data != null)
        {
            var rewardList = data.rankReard;
            if (rewardList == null || rewardList.Count < 1) return;
            for (int i = 0; i < rewardList.Count; i++)
            {
                var rewardItem = Instantiate(rewardPrefab, rewardContent);
                rewardItem.Init(rewardList[i]);
                rewardItemList.Add(rewardItem);
            }
        }
    }


    public void Inits(string matchId)
    {
        this.matchId = matchId;
        //请求比赛排行
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            matcherRank = new MatcherRank()
            {
                matcherId = matchId
            },
            msgid = MessageId.C2G_MatcherRank
        });
    }



    void LoadItems(List<MatcherPlayerRanking> ranks)
    {
        UIUtils.DestroyChildren(rankContent);
        for (int i = 0; i < ranks.Count; i++)
        {
            var rankItem = Instantiate(rankPrefab, rankContent);
            rankItem.Init(ranks[i],i+1);
            rankItemList.Add(rankItem);
        }
        ShowMyInfo();
    }

    void ShowMyInfo()
    {
        MatcherRankItem item = rankItemList.Find(p => p.data.userName == UserInfoModel.userInfo.nickName);
        if (item)
        {
            myRankInfo.Init(item.data, item.num);
        }
    }

    public static void QueryRankFinish(MatcherPlayerRankingResp resp)
    {
        MatchRankRewardNode node = NodeManager.GetNode<MatchRankRewardNode>();
        if (node)
        {
            node.LoadItems(resp.matcherPlayerRanking);
        }
    }



}
