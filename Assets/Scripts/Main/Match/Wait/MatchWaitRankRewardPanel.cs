using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchWaitRankRewardPanel : MonoBehaviour {

    public MatcherRankItem myRankInfo;
    public MatcherRankItem rankPrefab;
    public MatcherRewardItem rewardPrefab;
    public Transform rankContent;
    public Transform rewardContent;
    private List<MatcherRankItem> rankItemList = new List<MatcherRankItem>();
    private List<MatcherRewardItem> rewardItemList = new List<MatcherRewardItem>();

    public void Init()
    {
        UIUtils.DestroyChildren(rewardContent);
        MatcherInfo data = MatchModel.Instance.CurData;
        if (data != null)
        {
            List<RankReward> rewardList = data.rankReard;
            if (rewardList == null || rewardList.Count < 1) return;
            for (int i = 0; i < rewardList.Count; i++)
            {
                var rewardItem = Instantiate(rewardPrefab, rewardContent);
                rewardItem.Init(rewardList[i]);
                rewardItemList.Add(rewardItem);
            }
        }
    }
    void OnEnable()
    {
        string matchId = string.Empty;
        if (PageManager.Instance.CurrentPage is LandlordsPage)
            matchId = LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomID;
        else if(MaJangPage.Instance)
        {
            matchId = MaJangPage.Instance.roomNo.text;
        }
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
            rankItem.Init(ranks[i], i + 1);
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
        MatchWaitNode node = NodeManager.GetNode<MatchWaitNode>();
        if (node)
        {
            node.bottomPanel.rankRewardPanel.LoadItems(resp.matcherPlayerRanking);
        }
    }
}
