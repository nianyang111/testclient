using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LandlordsPage : Page
{

    #region 游戏计时
    /// <summary>叫分等待时间</summary>
    public const float wait_CallFenTime = 25;
    /// <summary>叫地主等待时间</summary>
    public const float wait_CallLandlordsTime = 25;
    /// <summary>抢地主等待时间</summary>
    public const float wait_QiangTime = 25;
    /// <summary>出牌等待时间</summary>
    public const float wait_PopTime = 25;
    /// <summary>要不起等到时间</summary>
    public const float wait_canNotPopTime = 5;

    #endregion



    public static LandlordsPage Instance;

    public LandlordsPlayView playView;
    public LandlordsComponentView componentView;
    public LandlordsResultView resultView;
    public Interaction interation;
    public GameObject cardItem;

    //[HideInInspector]
    public Sprite[] animations;
    public AssetBundle cardAssetBunle;
    public AssetBundle faceBundle;
    public CardsType cardType;

    public Image bgIcon;

    //public Text turnLb;
    
    void Awake()
    {
        Instance = this;
        cardAssetBunle = BundleManager.Instance.GetSpriteBundle("card", typeof(LandlordsPage).ToString());
        faceBundle = BundleManager.Instance.GetSpriteBundle("face");
    }


    void Update()
    {
        if (InputUtils.OnPressed())
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            if (go == null)
            {
                if (LandlordsModel.Instance.IsInFight)
                    Interaction.Instance.Chongxuan();
            }
        }

    }

    public override void Init()
    {
        base.Init();
        FloatBallManager.Instance.Hide();
        PageManager.Instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
        PageManager.Instance.canvas.worldCamera = Camera.main;        
        NodeManager.OpenNode<ChatNode>(null, null, false);
        ChatNode.Close();
        animations = BundleManager.Instance.GetSprites(faceBundle);
        NodeManager.OpenNode<NoticeNode>(null, null, false, false).Inits(Vector3.up * 412.7f, NoticeNode.NoticeSize.Short); 
    }


    public void InitRoom()
    {        
        interation.InitBtn();
        playView.Init();
        componentView.Init();
        resultView.Init();
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            bgIcon.sprite = BundleManager.Instance.GetSprite("doudizhu_bj_fangjian_bishai", LandlordsPage.Instance.GetSpriteAB());
        else
            bgIcon.sprite = BundleManager.Instance.GetSprite("doudizhu_bg_fangjian", LandlordsPage.Instance.GetSpriteAB());
    }


    private int multiples;
    private int dizhu;

    /// <summary>
    /// 全场倍数
    /// </summary>
    public int Multiples
    {
        set
        {
            multiples = value;
            componentView.UpdateUserInfoShow();
        }
        get { return multiples; }
    }

    /// <summary>
    /// 底注
    /// </summary>
    public int Dizhu
    {
        set
        {
            dizhu = value;
            componentView.UpdateUserInfoShow();
        }
        get { return dizhu; }
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="cType"></param>
    public void GameStart()
    {
        Multiples = 1;
        LandlordsClearFight();
        //所有界面的游戏开始初始化
        playView.GameStart();
        componentView.GameStart();
        interation.GameStart();        
    }



    #region 卡牌相关        

    #region 通讯
    /// <summary>
    /// 推送玩家离开
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="remain"></param>
    public void PlayerExit(string uid,bool isKick)
    {
        playView.PlayerExit(uid, isKick);
    }    

    /// <summary>
    /// 玩家准备
    /// </summary>
    /// <param name="cType"></param>
    public void PlayerZhunbei(string uid, bool isZhunbei)
    {
        LandkirdsHandCardModel cards = LandlordsModel.Instance.GetHandCardMode(uid);
        cards.IsZhunbei = isZhunbei;
        playView.PlayerZhunbei(uid, isZhunbei);
    }

    /// <summary>
    /// 玩家出牌
    /// </summary>
    /// <param name="cType"></param>
    /// <param name="selectCards"></param>
    /// <param name="selectCardsUI"></param>
    public void PopCard(string uid, List<Card> selectCards)
    {
        LandlordsBasePlayer playCard = LandlordsPage.Instance.playView.GetPlayer(uid);
        PlayCard.Instance.PopCard(uid, selectCards);
        playCard.PopCard(selectCards);
    }

    /// <summary>
    /// 玩家不出 -1要不起0不出
    /// </summary>
    /// <param name="uid"></param>
    public void NoPopCard(string uid, int type)
    {
        LandlordsBasePlayer playCard = LandlordsPage.Instance.playView.GetPlayer(uid);
        playCard.NoPopCard(type);
    }

    /// <summary>
    /// 玩家叫地主
    /// </summary>
    /// <param name="cType"></param>
    public void JiaoDizhu(string uid, bool isJiao)
    {
        if(isJiao)
            Multiples *= 3;
        LandkirdsHandCardModel cards = LandlordsModel.Instance.GetHandCardMode(uid);
        LandlordsBasePlayer playCard = LandlordsPage.Instance.playView.GetPlayer(uid);
        playCard.Jiaodizhu(isJiao);
    }

    /// <summary>
    /// 玩家抢地主
    /// </summary>
    /// <param name="cType"></param>
    public void QiangDizhu(string uid, bool isQiang)
    {
        if (isQiang)
            Multiples *= 2;
        LandkirdsHandCardModel cards = LandlordsModel.Instance.GetHandCardMode(uid);
        LandlordsBasePlayer playCard = LandlordsPage.Instance.playView.GetPlayer(uid);
        playCard.Qiangdizhu(isQiang);
    }

    /// <summary>
    /// 玩家叫分
    /// </summary>
    /// <param name="score"></param>
    public void CallLandlord(string uid, int score)
    {        
        LandkirdsHandCardModel cards = LandlordsModel.Instance.GetHandCardMode(uid);
        cards.Multiples = score;
        cards.CallScore = score;
        LandlordsModel.Instance.CallLandlordsList.Add(cards);
        LandlordsBasePlayer playCard = LandlordsPage.Instance.playView.GetPlayer(uid);
        playCard.CallLandlord(score);
    }

    /// <summary>
    /// 玩家变成地主
    /// </summary>
    public void PlayerToLandlord(string uid, bool isAddCardCountToPlayer = true)
    {
        LandkirdsHandCardModel cards = LandlordsModel.Instance.GetHandCardMode(uid);
        LandlordsModel.Instance.CurLandlordUid = uid;
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch && !LandlordsModel.Instance.RoomModel.CurRoomInfo.IsQdz)
            Multiples = cards.Multiples;

        List<Card> dipai = new List<Card>();
        //显示底牌       
        while (DzCard.Instance.CardsCount != 0)
        {
            Card card = DzCard.Instance.Deal();
            dipai.Add(card);
            if (isAddCardCountToPlayer)
                cards.AddCard(card);
        }
        UpdateDeskCardShow(dipai);
        for (int i = 0; i < dipai.Count; i++)
        {
            dipai[i].IsSprite = false;
        }
        LandlordsBasePlayer playCard = LandlordsPage.Instance.playView.GetPlayer(uid);
        if (uid == UserInfoModel.userInfo.userId.ToString())
            playCard.DealCard(faceBundle);
        if (playCard is LandlordsOtherPlayer)
            ((LandlordsOtherPlayer)playCard).CardRemainCountShow();
        //更新身份
        UpdateIndentity(uid, Identity.Landlord);
    }

    /// <summary>
    /// 玩家托管
    /// </summary>
    /// <param name="cType"></param>
    public void PlayerTuoGuan(string uid, bool isTuoguan)
    {
        if (uid == UserInfoModel.userInfo.userId.ToString())
        {
            LandlordsPage.Instance.componentView.OpenTuoGuanView(isTuoguan);
            if (OrderController.Instance.CurInterationType == InterationType.PopCard)
                Interaction.Instance.HideAllBtn();
        }

        LandkirdsHandCardModel cards = LandlordsModel.Instance.GetHandCardMode(uid);
        cards.isTuoguan = isTuoguan;
        if (!isTuoguan &&
            uid == UserInfoModel.userInfo.userId.ToString() &&
            OrderController.Instance.TypeUid == UserInfoModel.userInfo.userId.ToString() && LandlordsModel.Instance.CurWinerIds != null && LandlordsModel.Instance.CurWinerIds.Count == 0)//并且游戏没有结束
        {
            OrderController.Instance.Turn(UserInfoModel.userInfo.userId.ToString(), (int)OrderController.Instance.CurInterationType);
        }

        LandlordsBasePlayer playCard = LandlordsPage.Instance.playView.GetPlayer(uid);
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            playCard.MatchChangeState(false);
        else
            playCard.NoMatchChangeState(false);
    }

    /// <summary>
    /// 玩家聊天
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="value"></param>
    public void PlayerChat(string uid, string senderName, string value, int type)//0文字1语音2表情
    {
        LandlordsBasePlayer playCard = LandlordsPage.Instance.playView.GetPlayer(uid);
        //显示在玩家头上
        playCard.Chat(value, type);        
        ChatInfo info = new ChatInfo();
        info.chatWithName = senderName;
        info.playerBaseInfo.uid = uid;
        info.text = value;
        info.type = type;
        if (type != 2)//如果不是表情，那么加载在聊天界面
            NodeManager.GetNode<ChatNode>().chatPanel.LoadChatItem(info);
        if (type == 1)//直接播放语音
            SocialModel.Instance.PlayRecord(value);
        if (type == 0)
        {//判断是否播放常用语语音
            string changyongyuId = string.Empty;
            JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.changyongyuConfig));
            for (int i = 0; i < jd.Count; i++)
            {
                if (value == jd[i].TryGetString("chatContent"))
                {
                    changyongyuId = jd[i].TryGetString("id");
                    break;
                }
            }
            if (changyongyuId != string.Empty)
            {
                string audioName = playCard._handCard.playerInfo.six + "_" + changyongyuId;
                AudioManager.Instance.PlayTempSound((AudioManager.AudioSoundType)System.Enum.Parse(typeof(AudioManager.AudioSoundType), audioName));
            }
        }

    }
    #endregion
    /// <summary>
    /// 显示底牌
    /// </summary>
    void UpdateDeskCardShow(List<Card> dipai)
    {
        LandlordsPage.Instance.playView.DealDipai(dipai);
    }


    /// <summary>
    /// 更新身份
    /// </summary>
    /// <param name="type"></param>
    /// <param name="identity"></param>
    void UpdateIndentity(string uid, Identity identity)
    {
        LandkirdsHandCardModel cards = LandlordsModel.Instance.GetHandCardMode(uid);
        //改变属性
        cards.AccessIdentity = identity;
        //更改显示
        LandlordsBasePlayer playCard = LandlordsPage.Instance.playView.GetPlayer(uid);
        playCard.ChangeIdentity(false);
    }

    /// <summary>
    /// 加载卡牌
    /// </summary>
    public static CardUI MakeSprite(Card card, bool isSelect, Transform parent)
    {
        if (!card.IsSprite)
        {
            GameObject poker = Instantiate(LandlordsPage.Instance.cardItem, parent);
            CardUI sprite = poker.gameObject.GetComponent<CardUI>();
            poker.SetActive(true);
            sprite.Card = card;
            sprite.Select = isSelect;
            return sprite;
        }
        return null;
    }
    #endregion

     #region 事件池方法
    public void LoadComplete()
    {
        interation.SetCardMaskShow();
    }

    public void GameOver()
    {
        resultView.GameOver();
        interation.GameOver();
        componentView.GameOver();
        playView.GameOver();       
        if (LandlordsModel.Instance.RoomPlayerHands != null)
        {
            for (int i = 0; i < LandlordsModel.Instance.RoomPlayerHands.Count; i++)
            {
                if (LandlordsModel.Instance.RoomPlayerHands[i] == null)
                    continue;
                PlayerTuoGuan(LandlordsModel.Instance.RoomPlayerHands[i].playerInfo.uid, false);
                LandlordsModel.Instance.RoomPlayerHands[i].IsZhunbei = false;
                LandlordsModel.Instance.RoomPlayerHands[i].AccessIdentity = Identity.Farmer;
            }
        }
        DeskCardsCache.Instance.Clear(false);
        DzCard.Instance.Clear();
        //比赛场处理
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            InitRoom();
    }

    /// <summary>
    /// 清理战斗
    /// </summary>
    public void LandlordsClearFight()
    {
        playView.ClearUI();
        resultView.ClearUI();
        componentView.ClearUI();
        interation.NoActiveCardButton();
        DeskCardsCache.Instance.Clear();
        DzCard.Instance.Clear();
        OrderController.Instance.Clear();
        multiples = 1;
        //dizhu = 0;
        LandlordsModel.Instance.ClearFight();
    }

    public void LandlordsWarning(string uid,int remainCard,bool isShow)
    {
        playView.SetWarningShow(uid, remainCard, isShow);
    }


     #endregion

    public override void Close()
    {
        cardAssetBunle.Unload(true);
        ClearAll();
        base.Close();
    }

    public void ClearAll()
    {
        //dizhu = 0;
        multiples = 1;
        //LandlordsNet.QuiteReq();
        LandlordsModel.Instance.RoomModel.Clear();
        LandlordsModel.Instance.Clear();
        OrderController.Instance.Clear();
        LandlordsModel.Instance.IsTuoGuan = false;        
    }


    public void FinishData()
    {
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.SilverCoin)
        {
            LandlordsBasePlayer player= playView.GetPlayer(UserInfoModel.userInfo.userId.ToString());
            player.coinCountLb.text = UserInfoModel.userInfo.walletAgNum.ToString();
        }
    }
}

