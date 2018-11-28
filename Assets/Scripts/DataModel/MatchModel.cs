using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchModel
{
    private static MatchModel instance;
    public static MatchModel Instance
    {
        get
        {
            if (instance == null)
                instance = new MatchModel();
            return instance;
        }
        set { instance = value; }
    }

    /// <summary> 列表数据 </summary>
    public List<MatcherInfo> matcherInfoList = new List<MatcherInfo>();

    /// <summary> 战绩统计 </summary>
    public MatcherCount matcherCount = new MatcherCount();
    /// <summary>比赛奖励 </summary>
    public List<MatcherRewardlog> rewardList = new List<MatcherRewardlog>();
    /// <summary>历史战绩 </summary>
    public List<MatchHistoryRecordData> histoyList = new List<MatchHistoryRecordData>();
    /// <summary>好友列表 </summary>
    public List<MatchFriendRankData> friendList = new List<MatchFriendRankData>();

    private MatcherInfo curData;

    public MatcherInfo CurData
    {
        get { return curData; }
        set { curData = value; }
    }

    public Sprite rewardIcon;
    /// <summary>请求比赛列表</summary>
    public void LoadMatcherFinish(LoadMatcherResp resp)
    {
        matcherInfoList.Clear();
        matcherInfoList = resp.matcherInfo;
        if (PageManager.Instance.CurrentPage is MatchPage)
            PageManager.Instance.GetPage<MatchPage>().CreateItem();
    }
    /// <summary>战绩统计 </summary>
    public void MatcherCountFinish(MatcherCountResp resp)
    {
        MatcherCount count = new MatcherCount();
        count.userId = resp.userId;
        count.successNum = resp.champion;
        count.finalistNum = resp.finals;
        count.promotionNum = resp.promotion;
        count.latestMatcher = resp.latestMatcher;
        if (count.userId == UserInfoModel.userInfo.userId)
        {
            matcherCount = count;
            if (NodeManager.GetNode<MatchRecordNode>())
                NodeManager.GetNode<MatchRecordNode>().myRecordPanel.FlushData();
            var finishItem = friendList.Find(p => p.userId == count.userId);
            if (finishItem != null)
                finishItem.matcherCount = count;
            if (NodeManager.GetNode<MatchRecordNode>())
                NodeManager.GetNode<MatchRecordNode>().friendRankPanel.FlushData();
        }
        else
        {
            var finishItem = friendList.Find(p => p.userId == count.userId);
            if (finishItem != null)
                finishItem.matcherCount = count;
            if (NodeManager.GetNode<MatchRecordNode>())
                NodeManager.GetNode<MatchRecordNode>().friendRankPanel.FlushData();
        }
    }
    /// <summary> 战绩奖励</summary>
    public void UserMatcherRewardFinish(UserMatcherRewardResp resp)
    {
        rewardList.Clear();
        rewardList = resp.matcherRewardlog;
        if (NodeManager.GetNode<MatchRecordNode>())
            NodeManager.GetNode<MatchRecordNode>().myRecordPanel.CreateItem();
    }
    /// <summary>历史战绩 </summary>
    public void MatcherHistoryFinish(MatcherHistoryResp resp)
    {
        histoyList.Clear();
        var histoysList = resp.matcherHistory;
        for (int i = 0; i < histoysList.Count; i++)
        {
            MatchHistoryRecordData data = new MatchHistoryRecordData();
            data.type = histoysList[i].matcherName;
            data.rank = histoysList[i].rank;
            data.eliminate = histoysList[i].dieNum;
            data.date = histoysList[i].matcherTime;
            histoyList.Add(data);
        }
        if (NodeManager.GetNode<MatchRecordNode>())
            NodeManager.GetNode<MatchRecordNode>().historyRecordPaenl.CreateItem();
    }
    /// <summary>好友排行 </summary>
    public void MatcherFriendRankFinish(UserMatcherFriendRankResp resp)
    {
        friendList.Clear();
        var firendRankList = resp.userMatcherFriendRank;
        for (int i = 0; i < firendRankList.Count; i++)
        {
            MatchFriendRankData data = new MatchFriendRankData();
            data.rankId = i + 1;
            data.userName = firendRankList[i].userName;
            data.masterScore = firendRankList[i].masterScore;
            data.masterLevel = firendRankList[i].masterLevel;
            data.userId = firendRankList[i].userId;
            friendList.Add(data);
        }
        if (NodeManager.GetNode<MatchRecordNode>())
            NodeManager.GetNode<MatchRecordNode>().friendRankPanel.CreateItem();
    }
    /// <summary>加入游戏</summary>
    public void JoinMatch(int cost,int costType,string matchId,string matchName,Node node = null)
    {
        //花费 -1免费 大于0
        if (cost <=0)
        {
            SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
            {
                msgid = net_protocol.MessageId.C2G_JoinMatcher,
                joinMatcher = new net_protocol.JoinMatcher()
                {
                    matcherId = matchId,
                    gold = 0,
                    silver = 0,
                }
            });

        }
        else if (cost > 0)
        {
            if (costType == 1)//1银币 2金币
            {
                if (cost <= UserInfoModel.userInfo.walletAgNum)
                {
                    SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
                    {
                        msgid = net_protocol.MessageId.C2G_JoinMatcher,
                        joinMatcher = new net_protocol.JoinMatcher()
                        {
                            matcherId = matchId,
                            gold = 0,
                            silver = cost
                        }
                    });
                }
                else
                {
                    TipManager.Instance.OpenTip(TipType.ChooseTip,
                    string.Format("报名" + matchName + "需要报名费" + cost + "银币,需获得更多的银币才能继续报名"),
                    0, delegate { NodeManager.OpenNode<StoreNode>().agBtn.isOn = true; }, delegate { if (node) node.Close(); });
                }
            }
            if (costType == 2)
            {
                if (cost <= UserInfoModel.userInfo.walletGoldBarNum)
                {
                    SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
                    {
                        msgid = net_protocol.MessageId.C2G_JoinMatcher,
                        joinMatcher = new net_protocol.JoinMatcher()
                        {
                            matcherId = matchId,
                            gold = cost,
                            silver = 0
                        }
                    });
                }
                else
                {
                    TipManager.Instance.OpenTip(TipType.ChooseTip,
                    string.Format("报名" + matchName + "需要报名费" + cost + "金条,需获得更多的银币才能继续报名"),
                    0, delegate { NodeManager.OpenNode<StoreNode>(); }, delegate { if (node)node.Close(); });
                }
            }
        }
    }
    /// <summary>进入准备</summary>
    public void PrepareMatcherFinish(PrepareMatcherResp resp)
    {
        if (resp.result == 1)
        {
            CurData = resp.matcherInfo; ;
            NodeManager.OpenNode<MatchReadyNode>("match");
            TipManager.Instance.OpenTip(TipType.SimpleTip, "报名成功");
        }
    }
    /// <summary>得到大师分等级配置</summary>
    public MatchMasterScoer GetLvJsonData(int lv)
    {
        List<MatchMasterScoer> masterScoer = new List<MatchMasterScoer>();
        LitJson.JsonData json = LitJson.JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.masterScoreConfig));
        for (int i = 0; i < json.Count; i++)
        {
            MatchMasterScoer data = JsonMapper.ToObject<MatchMasterScoer>(JsonMapper.ToJson(json[i]));
            masterScoer.Add(data);
        }
        return masterScoer.Find(p => p.masterLevel == lv);
    }
}
public class MatcherCount
{
    public int userId;
    /// <summary>入围次数 </summary>
    public int finalistNum;
    /// <summary>晋级次数 </summary>
    public int promotionNum;
    /// <summary>夺冠次数 </summary>
    public int successNum;
    /// <summary>最近一次比赛 </summary>
    public string latestMatcher;
}
public class MatchMasterScoer
{
    public int firstScore;
    public int id;
    public int masterLevel;
    public int promotionScore;
    public int secondScore;
    public int thirdScore;
    public int upExp;
}