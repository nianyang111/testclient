using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 斗地主房间数据
/// </summary>
public class LandlordsRoomModel
{
    private LandlordsRoomInfo curRoomInfo;

    /// <summary>
    /// 当前房间数据
    /// </summary>
    public LandlordsRoomInfo CurRoomInfo
    {
        set { curRoomInfo = value; }
        get { return curRoomInfo; }
    }

    public static JsonData GetYxbRoomConfigById(int id)
    {
        JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson("DdzRoomGradeConfig"))[id.ToString()];
        return jd;
    }

    /// <summary>
    /// 根据配置房间号得到游戏币场类型  房卡房和比赛场不经过这个方法
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static RoomType GetYxbRoomTypeByID(int id)
    {
        JsonData jd = GetYxbRoomConfigById(id);
        string roomTypeStr = jd.TryGetString("roomType");
        switch (roomTypeStr)
        {
            case "银币场":
                return RoomType.SilverCoin;
            case "金条场":
                return RoomType.GoldBar;
        }
        return RoomType.SilverCoin;
    }


    public void Clear()
    {
        curRoomInfo = null;
    }

    public class LandlordsRoomInfo
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomID { get; set; }

        /// <summary>
        /// 房间类型0免费1付费
        /// </summary>
        public RoomType RoomType { get; set; }

        /// <summary>
        /// 是否是比赛
        /// </summary>
        public bool IsMatch { get; set; }

        /// <summary>
        /// 是否抢地主
        /// </summary>
        public bool IsQdz { get; set; }

        /// <summary>
        /// 倍数
        /// </summary>
        public int Beishu { get; set; }

        /// <summary>
        /// 底注
        /// </summary>
        public int LeastStore { get; set; }

        /// <summary>
        /// 消耗房卡
        /// </summary>
        public int CostCard { get; set; }

        /// <summary>
        /// 当前场次
        /// </summary>
        public int CurPlayCount { set; get; }

        /// <summary>
        /// 最大场次
        /// </summary>
        public int MaxPlayCount { set; get; }

    }
}



/// <summary>
/// 所有玩家手牌数据层
/// </summary>
[System.Serializable]
public class LandkirdsHandCardModel : ICard
{
    List<Card> library = new List<Card>();
    public Identity identity;
    int callScore;//玩家叫分
    int multiples = 1;//玩家倍数0
    int matchScore;//比赛积分
    bool isRobot = false;//是否是机器人
    bool isZhunbei = false;
    bool isRoomer = false;//是否是房主

    public BasePlayerInfo playerInfo;
    public LandkirdsHandCardModel(string uid, Six six)
    {
        playerInfo = new BasePlayerInfo();
        playerInfo.uid = uid;
        playerInfo.six = six;
    }

    /// <summary>
    /// 是否准备
    /// </summary>
    public bool IsZhunbei
    {
        set { isZhunbei = value; }
        get { return isZhunbei; }
    }

    /// <summary>
    /// 是否是房主
    /// </summary>
    public bool IsRoomer
    {
        set { isRoomer = value; }
        get { return isRoomer; }
    }

    /// <summary>
    /// 是否托管
    /// </summary>
    public bool isTuoguan
    {
        set { isRobot = value; }
        get { return isRobot; }
    }

    /// <summary>
    /// 比赛积分
    /// </summary>
    public int MatchScore
    {
        set { matchScore = value; }
        get { return matchScore; }
    }

    /// <summary>
    /// 玩家倍数
    /// </summary>
    public int Multiples
    {
        set
        {
            if (value == 0)
                value = 1;//最低为1倍
            multiples *= value;
        }
        get { return multiples; }
    }

    /// <summary>
    /// 玩家叫分
    /// </summary>
    public int CallScore
    {
        set
        {
            callScore = value;
        }
        get { return callScore; }
    }

    /// <summary>
    /// 手牌数
    /// </summary>
    public int CardsCount
    {
        get { return library.Count; }
    }

    /// <summary>
    /// 访问身份
    /// </summary>
    public Identity AccessIdentity
    {
        set
        {
            identity = value;
        }
        get { return identity; }
    }

    /// <summary>
    /// 访问性别
    /// </summary>
    public Six Six
    {
        get { return playerInfo.six; }
    }

    /// <summary>
    /// 获取索引手牌
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Card this[int index]
    {
        get { return library[index]; }
    }

    /// <summary>
    /// 获取值的索引
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public int this[Card card]
    {
        get { return library.IndexOf(card); }
    }

    /// <summary>
    /// 添加手牌
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(Card card)
    {
        library.Add(card);
        if (card != null)//为null证明是其他玩家发的模拟牌
            card.Attribution = playerInfo.uid;
    }

    /// <summary>
    /// 出牌
    /// </summary>
    /// <returns></returns>
    public void PopCard(Card card)
    {
        if (playerInfo.uid != UserInfoModel.userInfo.userId.ToString())
        {
            library.RemoveAt(0);
        }
        else
        {
            //从手牌移除
            Card cards = library.Find(p => p.Poker.hs == card.Poker.hs && p.Poker.ds == card.Poker.ds);            
            library.Remove(cards);
        }
    }

    /// <summary>
    /// 手牌排序
    /// </summary>
    public void Sort()
    {
        CardRules.SortCards(library, false);
    }

    /// <summary>
    /// 清除手牌
    /// </summary>
    public void Clear()
    {
        library.Clear();
    }

    public Card Deal()
    {
        return null;
    }
}



/// <summary>
/// 桌面扑克缓存区
/// </summary>
public class DeskCardsCache : ICard
{
    private static DeskCardsCache instance;
    private List<Card> library;
    private string uid = "-9999";
    private CardsType rule;

    /// <summary>
    /// 单例属性
    /// </summary>
    public static DeskCardsCache Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DeskCardsCache();
            }
            return instance;
        }
    }

    public CardsType Rule
    {
        set
        {
            if (LandlordsPage.Instance != null)
                LandlordsPage.Instance.cardType = value;
            rule = value;
        }
        get { return rule; }
    }

    /// <summary>
    /// 索引器
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Card this[int index]
    {
        get
        {
            return library[index];
        }
    }

    /// <summary>
    /// 获取牌库中牌的数量
    /// </summary>
    public int CardsCount
    {
        get { return library.Count; }
    }

    /// <summary>
    /// 最小权值
    /// </summary>
    public int MinWeight
    {
        get { return (int)library[0].GetCardWeight; }
    }

    /// <summary>
    /// 总权值
    /// </summary>
    public int TotalWeight
    {
        get
        {
            return LandlordsModel.Instance.GetWeight(library.ToArray(), Rule);
        }
    }

    /// <summary>
    /// 私有构造
    /// </summary>
    private DeskCardsCache()
    {
        library = new List<Card>();
        Rule = CardsType.None;
    }

    /// <summary>
    /// 发牌
    /// </summary>
    public Card Deal()
    {
        Card ret = library[library.Count - 1];
        library.Remove(ret);
        return ret;
    }

    /// <summary>
    /// 向牌库中添加牌
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(Card card)
    {
        card.Attribution = uid;
        library.Add(card);
    }

    /// <summary>
    /// 清空桌面
    /// </summary>
    public void Clear(bool isClearPopCard = true)//是否清空桌面已出牌UI?
    {
        if (isClearPopCard)
            LandlordsPage.Instance.playView.ClearDesk();
        if (library.Count != 0)
        {
            while (library.Count != 0)
            {
                Card card = library[library.Count - 1];
                library.Remove(card);
            }
            Rule = CardsType.None;
        }
    }

    /// <summary>
    /// 手牌排序
    /// </summary>
    public void Sort()
    {
        CardRules.SortCards(library, true);
    }
}

/// <summary>
/// 地主牌
/// </summary>
public class DzCard:ICard
{
     private static DzCard instance;
    private List<Card> library;

    /// <summary>
    /// 单例属性
    /// </summary>
    public static DzCard Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DzCard();
            }
            return instance;
        }
    }



    /// <summary>
    /// 索引器
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Card this[int index]
    {
        get
        {
            return library[index];
        }
    }

    /// <summary>
    /// 获取牌库中牌的数量
    /// </summary>
    public int CardsCount
    {
        get { return library.Count; }
    }

    /// <summary>
    /// 私有构造
    /// </summary>
    private DzCard()
    {
        library = new List<Card>();
    }

    /// <summary>
    /// 发牌
    /// </summary>
    public Card Deal()
    {
        Card ret = library[library.Count - 1];
        library.Remove(ret);
        return ret;
    }

    /// <summary>
    /// 向牌库中添加牌
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(Card card)
    {
        library.Add(card);
    }

    /// <summary>
    /// 清空底牌
    /// </summary>
    public void Clear()
    {
        library.Clear();
    }

    /// <summary>
    /// 手牌排序
    /// </summary>
    public void Sort()
    {
        CardRules.SortCards(library, true);
    }
}

/// <summary>
/// 牌库
/// </summary>
public class LandlordsDeck : ICard
{
    private static LandlordsDeck instance;
    private List<Card> library;
    private string uid = "-10000";

    public static LandlordsDeck Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LandlordsDeck();
            }
            return instance;
        }
    }

    /// <summary>
    /// 获取牌库中牌的数量
    /// </summary>
    public int CardsCount
    {
        get { return library.Count; }
    }

    /// <summary>
    /// 索引器
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Card this[int index]
    {
        get
        {
            return library[index];
        }
    }

    /// <summary>
    /// 私有构造
    /// </summary>
    private LandlordsDeck()
    {
        library = new List<Card>();
    }

   
    /// <summary>
    /// 洗牌
    /// </summary>
    public void Shuffle()
    {
        if (CardsCount == 54)
        {
            System.Random random = new System.Random();
            List<Card> newList = new List<Card>();
            foreach (Card item in library)
            {
                newList.Insert(random.Next(newList.Count + 1), item);
            }

            library.Clear();

            foreach (Card item in newList)
            {
                library.Add(item);
            }

            newList.Clear();
        }
    }

    /// <summary>
    /// 发牌
    /// </summary>
    public Card Deal()
    {
        Card ret = library[library.Count - 1];
        library.Remove(ret);
        return ret;
    }

    /// <summary>
    /// 向牌库中添加牌
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(Card card)
    {
        card.Attribution = uid;
        library.Add(card);
        card.IsSprite = false;
    }



    public void Sort()
    {
        CardRules.SortCards(library, true);
    }
}


/// <summary>
/// 斗地主本轮结算信息
/// </summary>
public class LandlordsResultModel
{
    private List<DdzJSPlayerInfo> resultInfos = new List<DdzJSPlayerInfo>();
    public int jdz;//叫地主倍数
    public int zd;//炸弹倍数
    public int ct;//春天倍数
    public int fct;//反春天倍数
    public int curJs;//当前局数
    public int allJs;//总局数
    /// <summary>
    /// 添加结算信息
    /// </summary>
    public void Add(DdzJSPlayerInfo info)
    {
        resultInfos.Add(info);
    }

    /// <summary>
    /// 得到本回合结算信息
    /// </summary>
    /// <returns></returns>
    public List<DdzJSPlayerInfo> GetResultInfos()
    {
        return resultInfos;
    }

    public void Clear()
    {
        jdz = 0;
        zd = 0;
        ct = 0;
        fct = 0;
        curJs = 0;
        allJs = 0;
        resultInfos.Clear();
    }
}


/// <summary>
/// 提示数据层
/// </summary>
public class LandlordsTipsModel
{
    private List<List<Card>> lastTip = new List<List<Card>>();

    public LandlordsTipsModel()
    {
        OrderController.Instance.enterCall += Clear;        
        lastTip.Clear();
    }
    
    /// <summary>
    /// 根据手牌提示
    /// </summary>
    /// <param name="isCanTips">提示是否可以出牌</param>
    /// <returns></returns>
    public List<Card> Tips(out bool isCanTips)
    {
        isCanTips = false;
        List<Card> exclude = GetCardByLastTips();
        List<Card> cards = CardRules.DelayDiscardCard(UserInfoModel.userInfo.userId.ToString(), exclude);

        if (cards.Count > 0)
        {
            if (CheckWeight(cards) && DeskCardsCache.Instance.Rule != CardsType.None)
            {//如果本次提示和上次的一样-->再找
                lastTip.Add(cards);
                bool isHaveTips = false;
                cards = Tips(out isHaveTips);
            }
            else
                lastTip.Add(cards);
            isCanTips = true;
        }
        else
        {
            if (lastTip.Count > 0)
            {//证明之前有提示牌
                lastTip.Clear();
                bool isHaveTips = false;
                cards = Tips(out isHaveTips);
                isCanTips = true; 
            }
            else
            {
                isCanTips = false;
            }
        }
        return cards;
    }

    public List<Card> Tips2(List<Card> exclude)
    {
        List<Card> cards = CardRules.DelayDiscardCard(UserInfoModel.userInfo.userId.ToString(), exclude);
        return cards;
    }

    /// <summary>
    /// 根据以往提示找到本轮已提示过的的牌 
    /// </summary>
    /// <returns></returns>
    List<Card> GetCardByLastTips()
    {
        List<Card> exclude = new List<Card>();
        for (int i = 0; i < lastTip.Count; i++)
        {
            for (int j = 0; j < lastTip[i].Count; j++)
            {
                exclude.Add(lastTip[i][j]);
            }
        }
        return exclude;
    }

    /// <summary>
    /// 检查本次提示的权值是否和上次一样
    /// </summary>
    /// <param name="curCards"></param>
    /// <returns></returns>
    bool CheckWeight(List<Card> curCards)
    {
        if (lastTip.Count > 0)
        {//检查是否和上次提示的权值重复
            Card[] lastTipsCards = lastTip[lastTip.Count - 1].ToArray();
            int lastWeight = LandlordsModel.Instance.GetWeight(lastTipsCards, DeskCardsCache.Instance.Rule);
            int curWeight = LandlordsModel.Instance.GetWeight(curCards.ToArray(), DeskCardsCache.Instance.Rule);
            return lastWeight == curWeight;
        }
        else
            return false;
    }

    void Clear(bool b)
    {
        if (OrderController.Instance.TypeUid == UserInfoModel.userInfo.userId.ToString())
        {            
            lastTip.Clear();
        }
    }

    public void Clear()
    {
        lastTip.Clear();
        OrderController.Instance.enterCall -= Clear;
    }
}

public class LandlordsSoundModel
{
    public static AudioManager.AudioSoundType PlayPlayerPopSound(string uid, CardsType cardsType,Weight weight=Weight.Three)
    {
        LandkirdsHandCardModel playerHand = LandlordsModel.Instance.GetHandCardMode(uid);
        if (playerHand != null)
        {
            string soundStr = playerHand.Six + cardsType.ToString();
            if (cardsType == CardsType.Single || cardsType == CardsType.Double)
            {
                soundStr += weight;
            }
            return GameTool.StrToEnum<AudioManager.AudioSoundType>(soundStr);
        }
        return AudioManager.AudioSoundType.None;
    }

 
}
