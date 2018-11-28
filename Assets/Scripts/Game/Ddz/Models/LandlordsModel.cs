using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandlordsModel
{
    /// <summary>
    /// instance
    /// </summary>
    static LandlordsModel instance = new LandlordsModel();
    public static LandlordsModel Instance
    {
        get { return instance; }
    }

    private LandlordsRoomModel roomModel;

    private LandlordsTipsModel tipsModel;

    private List<LandkirdsHandCardModel> handCardMode;

    private List<LandkirdsHandCardModel> callLandlordsList;

    private List<LandkirdsHandCardModel> doubelList;

    private Dictionary<string, int> popCardCountDic;

    private string curLandlordUid = string.Empty;

    private List<int> curWinerIds;

    private LandlordsResultModel resultModel;

    private bool isTuoGuan = false;

    private bool isInFight = false;

    private int timeOutAudioPopCount;

    /// <summary>是否托管 </summary> 
    public bool IsTuoGuan
    {
        set { isTuoGuan = value; }
        get { return isTuoGuan; }
    }

    /// <summary>是否正在打牌</summary>
    public bool IsInFight
    {
        set { isInFight = value; }
        get { return isInFight; }
    }

    /// <summary>斗地主主角信息层</summary>
    public LandkirdsHandCardModel MyInfo
    {
        get 
        {
            return GetHandCardMode(UserInfoModel.userInfo.userId.ToString());
        }
    }

    /// <summary>
    /// 房间所有玩家
    /// </summary>
    public List<LandkirdsHandCardModel> RoomPlayerHands
    {
        get
        {
            return handCardMode;
        }
    }

    /// <summary>
    /// 本局叫地主列表
    /// </summary>
    public List<LandkirdsHandCardModel> CallLandlordsList
    {
        get
        {
            if (callLandlordsList == null)
                callLandlordsList = new List<LandkirdsHandCardModel>();
            return callLandlordsList;
        }
    }

    /// <summary>房间分区数据层</summary>
    public LandlordsRoomModel RoomModel
    {
        get { if (roomModel == null)roomModel = new LandlordsRoomModel(); return roomModel; }
    }

    /// <summary>
    /// 提示数据层
    /// </summary>
    public LandlordsTipsModel TipsModel
    {
        get 
        {
            if (tipsModel == null)
                tipsModel = new LandlordsTipsModel();
            return tipsModel;
        }
    }

    /// <summary>当前地主</summary>
    public string CurLandlordUid
    {
        set {curLandlordUid = value; }
        get { return curLandlordUid; }
    }

    /// <summary>当前农民</summary>
    public List<string> CurFarmer
    {
        get
        {
            List<string> farmers = new List<string>();           
            if (handCardMode == null)
                return farmers;

            CharacterType cType = CharacterType.Player0;
            for (int i = 0; i < handCardMode.Count; i++)
            {
                if (handCardMode[i].AccessIdentity == Identity.Farmer)
                    farmers.Add(handCardMode[i].playerInfo.uid);
                cType++;
            }
            return farmers;
        }
    }


    /// <summary>当前赢家</summary>
    public List<int> CurWinerIds
    {
        get
        {
            if (curWinerIds == null)
                curWinerIds = new List<int>();
            return curWinerIds;
        }
    }

    /// <summary>本轮结算信息</summary>
    public LandlordsResultModel ResultModel
    {
        get
        {
            if (resultModel == null)
                resultModel = new LandlordsResultModel();
            return resultModel;
        }
    }

    /// <summary>
    /// 时间到后系统自动出牌次数
    /// </summary>
    public int TimeOutAudioPopCount
    {
        set
        {
            timeOutAudioPopCount = value;
            if (timeOutAudioPopCount >= 2)
            {
                LandlordsNet.C2G_Tuoguan(1);
                TimeOutAudioPopCount = 0;
            }
        }
        get
        {
            return timeOutAudioPopCount;
        }
    }

    /// <summary>
    ///  创建玩家
    /// </summary>
    /// <param name="character">角色类型</param>
    /// <returns></returns>
    public LandkirdsHandCardModel CreateHandCardMode(string uid, Six six)
    {
        LandkirdsHandCardModel handCard = null;
        if (handCardMode == null)
            handCardMode = new List<LandkirdsHandCardModel>();
        handCard = handCardMode.Find(p => p != null && p.playerInfo.uid == uid);
        if (handCard == null)
        {
            handCard = new LandkirdsHandCardModel(uid, six);
            int noBodyIndex = handCardMode.IndexOf(null);
            if (noBodyIndex == -1)
                handCardMode.Add(handCard);
            else
                handCardMode[noBodyIndex] = handCard;
        }
        return handCard;
    }

    /// <summary>
    /// 得到某个玩家手牌数据层
    /// </summary>
    /// <param name="uid">玩家id</param>
    /// <returns></returns>
    public LandkirdsHandCardModel GetHandCardMode(string uid)
    {
        LandkirdsHandCardModel handCard = null;
        if (handCardMode != null)
            handCard = handCardMode.Find(p => p != null && p.playerInfo.uid == uid);
        //else
          //  Debug.LogWarning("查询玩家失败,玩家id:" + uid + "  当前元素个数:" + handCardMode.Count);
        return handCard;
    }

    /// <summary>
    /// 房间玩家排序
    /// </summary>
    public void RoomPlayerSort()
    {
        if (handCardMode != null && handCardMode.Count > 1)
        {
            LandkirdsHandCardModel mainRole = handCardMode.Find(p => p != null && p.playerInfo.uid == UserInfoModel.userInfo.userId.ToString());
            int mainRoleIndex = handCardMode.IndexOf(mainRole);
            int nextIndex = mainRoleIndex == handCardMode.Count - 1 ? 0 : mainRoleIndex + 1;
            LandkirdsHandCardModel nextRole = handCardMode[nextIndex];
            handCardMode.Remove(mainRole);
            handCardMode.Remove(nextRole);
            handCardMode.Insert(0, mainRole);
            handCardMode.Insert(1, nextRole);
        }
    }

    /// <summary>
    /// 得到当前能叫地主的分值
    /// </summary>
    /// <returns></returns>
    public List<int> GetCanCallLandlordNum()
    {
        //总分值
        List<int> temp = new List<int>() { 0, 1, 2, 3 };
        //不能再叫的分值
        List<int> canNotCall = new List<int>();
        //还能叫的分值
        List<int> canCall = new List<int>();

        for (int i = 0; i < CallLandlordsList.Count; i++)
        {
            int num = CallLandlordsList[i].CallScore;
            if (num != 0)
            {
                canNotCall.Add(num);
            }
        }

        for (int i = 0; i < temp.Count; i++)
        {
            bool isCanCall = true;
            for (int j = 0; j < canNotCall.Count; j++)
            {
                if (temp[i] == canNotCall[j] && temp[i] != 0)
                {
                    isCanCall = false;
                    break;
                }
                if (CallLandlordsList.Find(p => p.CallScore > temp[i]) != null)
                {
                    isCanCall = false;
                    break;
                }
            }
            if (isCanCall)
                canCall.Add(temp[i]);
        }
        return canCall;
    }


    /// <summary>
    /// 获取指定数组的权值
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="rule"></param>
    /// <returns></returns>
    public int GetWeight(Card[] cards, CardsType rule)
    {
        int totalWeight = 0;
        if (rule == CardsType.ThreeAndOne || rule == CardsType.ThreeAndTwo || rule == CardsType.FourAndTwo)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (i < cards.Length - 2)
                {
                    if (cards[i].GetCardWeight == cards[i + 1].GetCardWeight &&
                        cards[i].GetCardWeight == cards[i + 2].GetCardWeight)
                    {
                        totalWeight += (int)cards[i].GetCardWeight;
                        totalWeight *= 3;
                        break;
                    }
                }
            }
        }
        else if (rule == CardsType.TripleStraightDaiOne || rule == CardsType.TripleStraightDaiTwo)
        {
            List<Card> cardlist = new List<Card>();
            cardlist.AddRange(cards);
            CardRules.SortFly(cardlist);
            totalWeight = (int)cardlist[0].GetCardWeight;
        }
        else
        {
            for (int i = 0; i < cards.Length; i++)
            {
                totalWeight += (int)cards[i].GetCardWeight;
            }
        }

        return totalWeight;
    }

    /// <summary>
    /// 记录玩家出牌次数
    /// </summary>
    public void AddPlayerPopCardCount(string uid)
    {
        if (popCardCountDic == null)
            popCardCountDic = new Dictionary<string, int>();

        if (!popCardCountDic.ContainsKey(uid))
            popCardCountDic.Add(uid, 0);
        popCardCountDic[uid] += 1;
    }

    /// <summary>
    /// 得到玩家出牌次数
    /// </summary>
    /// <returns></returns>
    public int GetPlayerPopCount(string uid)
    {
        int count = 0;
        popCardCountDic.TryGetValue(uid, out count);
        return count;
    }

    public void PlaySound(AudioManager.AudioSoundType audioType)
    {
        if (SetNode.read == 1)
            AudioManager.Instance.PlayTempSound(audioType, PageManager.Instance.CurrentPage.name);
    }

    /// <summary>
    /// 清理斗地主
    /// </summary>
    public void Clear()
    {
        ClearFight();
        //ChatModel.Clear();        
        handCardMode = null;
    }

    /// <summary>
    /// 清理战斗
    /// </summary>
    public void ClearFight()
    {
        CurLandlordUid = string.Empty;
        if (curWinerIds != null)
            curWinerIds.Clear();
        if (callLandlordsList != null)
            callLandlordsList.Clear();
        if (doubelList != null)
            doubelList.Clear();
        popCardCountDic = null;
        if (resultModel != null)
        {
            resultModel.Clear();
            resultModel = null;
        }
        isTuoGuan = false;
        //清理桌面缓存
        DeskCardsCache.Instance.Clear();
        TipsModel.Clear();
        TimeOutAudioPopCount = 0;
    }




















}


