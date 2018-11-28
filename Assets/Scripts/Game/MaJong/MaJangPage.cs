using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaJangPage : Page
{
    public static MaJangPage Instance;
    public static int allMjNum = 112;
    public static bool isSendChangeSeatRequest = false;
    #region ...公共属性
    [HideInInspector]
    public GameObject btnQRCode, btnHelp, btnGameLog, btnMenu, btnChat, btnChangeTable1, btnInvitation, btnReady, btnRankList, btnMatchRule, btnVoice;
    [HideInInspector]
    public GameObject btnQuit, btnSet, btnTrusteeship, btnStore, btnWithdraw;
    [HideInInspector]
    public GameObject btnTing, btnChi, btnChiTing, btnGang, btnPeng, btnPengTing, btnDingTing, btnGuo,/* btnHu,*/ btnCancel;
    [HideInInspector]
    public GameObject resultBtnGroup, btnChangeTable2, btnShare, btnContinue, btnCloseResult;
    [HideInInspector]
    public GameObject readyGroup, gameMenu, chiSelectPanel, gangSelectPanel, tingSelectPanel, lastOutPoint, resultPanel, commonPanel, matchPanel, helpPanel, helpClose, voiceTipsObj;
    [HideInInspector]
    public GameObject trusteeshipPanel, btnCancelTrust, liuJuImage;
    [HideInInspector]
    public MaJangPlayer[] playerList;
    [HideInInspector]
    public Text roomNo, eliminateLine, rankText, surplusCount, maxMultiple, round, baseScoreText, timeLb;
    [HideInInspector]
    public int tableNoDiff, gangCount;
    [HideInInspector]
    public MaJangModel lastOutMj;
    [HideInInspector]
    public MaJangPlayer currentPlayer;
    [HideInInspector]
    public RoomType roomType;
    [HideInInspector]
    public Sprite[] currencySprites;
    [HideInInspector]
    public int playerCount = 0;
    [HideInInspector]
    public Timer timer;
    [HideInInspector]
    public Image batteryIcon;
    [HideInInspector]
    public MaJangPlayer currentOperator;//当前操作的玩家
    [HideInInspector]
    public MaJangEffect mje;
    #endregion
    //资源
    [HideInInspector]
    public AssetBundle majangBundle, faceBundle, xjxdBundle;
    [HideInInspector]
    public Sprite[] animations, xjxdAnimations;
    GameObject currentBaoMj;
    /// <summary>
    /// 当前游戏状态 0-未开始，1-游戏进行中，2-游戏已结束，但不是第一回合
    /// </summary>
    public int currentStatu;
    [HideInInspector]
    public MaJangResult majangResult;
    [HideInInspector]
    public CommonAnimation showBanker;
    int currentRound = 1;
    int maxRound;

    List<GameUser> currentUserList = new List<GameUser>();

    /// <summary>
    /// 麻将间距
    /// </summary>
    public static float mj_interval = 0.5f;
    public static float mj_size = 1f;

    public void Awake()
    {
        majangBundle = BundleManager.Instance.GetSpriteBundle("majang", typeof(MaJangPage).ToString());
        xjxdBundle = BundleManager.Instance.GetSpriteBundle("animation", typeof(MaJangPage).ToString());
        faceBundle = BundleManager.Instance.GetSpriteBundle("face");
    }

    public override void Open()
    {
        Instance = this;
        base.Open();
        currentPlayer = playerList[0];
        FloatBallManager.Instance.Hide();
        majangResult.Init();
        timer.endAction = () =>
        {
            if (!currentPlayer.isFinishAction && !roomType.Equals(RoomType.RoomCard) && currentPlayer.Equals(currentOperator))// || currentPlayer.isWaitAction)
            {
                RequestTrusteeship(true);
                ShowSelectTingPanelFinish();
            }
        };
    }
    public override void Init()
    {
        base.Init();
        PageManager.Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        NodeManager.OpenNode<ChatNode>(null, null, false);
        ChatNode.Close();
        InitButton();
        resultPanel.SetActive(true);
        animations = BundleManager.Instance.GetSprites(faceBundle);
        xjxdAnimations = BundleManager.Instance.GetSprites(xjxdBundle);

        foreach (MaJangPlayer mjp in playerList)
        {
            mjp.xjxdAnimation.SpriteFrames.AddRange(xjxdAnimations);
            mjp.xjxdAnimation.onPlayEndCall = () => { mjp.xjxdAnimation.gameObject.SetActive(false); };
        }

        switch (roomType)
        {
            case RoomType.GoldBar:
            case RoomType.SilverCoin:
                btnShare.SetActive(false);
                break;
            case RoomType.RoomCard:
                btnChangeTable2.SetActive(false);
                break;
            case RoomType.Match:
                resultBtnGroup.SetActive(false);
                break;
        }
        resultPanel.SetActive(false);
    }

    void InitButton()
    {
        #region ...右上角功能菜单
        UGUIEventListener.Get(btnQuit).onClick = delegate//退出
        {
            btnMenu.GetComponent<RadioButton>().SetValue(false);
            if (currentStatu == 0)
            {
                if (roomType.Equals(RoomType.Match))
                    TipManager.Instance.OpenTip(TipType.AlertTip, "比赛场不能退出房间");
                else
                {
                    SocketClient.Instance.AddSendMessageQueue(new C2GMessage() { msgid = MessageId.C2G_TableLeave });
                    PageManager.Instance.OpenLastPage();
                }
            }
            else
            {
                switch (roomType)
                {
                    case RoomType.RoomCard:
                        TipManager.Instance.OpenTip(TipType.ChooseTip, "牌局已经开始，解散房间后才能退出房间，确定要解散房间吗？", 0, () =>
                        { SocketClient.Instance.AddSendMessageQueue(new C2GMessage() { msgid = MessageId.C2G_Disband }); });
                        break;
                    default:
                        TipManager.Instance.OpenTip(TipType.AlertTip, "游戏已经开始，不能退出房间");
                        break;
                }
            }
        };
        UGUIEventListener.Get(btnSet).onClick = delegate { NodeManager.OpenNode<SetGameNode>(); btnMenu.GetComponent<RadioButton>().SetValue(false); };//设置
        UGUIEventListener.Get(btnTrusteeship).onClick = delegate//托管
        {
            if (GetPlayerFromUerId(UserInfoModel.userInfo.userId).statu == 2 || GetPlayerFromUerId(UserInfoModel.userInfo.userId).statu == 1)
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "听牌不能托管");
                return;
            }
            RequestTrusteeship(true);
            btnMenu.GetComponent<RadioButton>().SetValue(false);
        };
        UGUIEventListener.Get(btnCancelTrust).onClick = delegate//取消托管
        {
            RequestTrusteeship(false);
        };
        UGUIEventListener.Get(btnStore).onClick = delegate { NodeManager.OpenNode<StoreNode>(); btnMenu.GetComponent<RadioButton>().SetValue(false); };//商城
        UGUIEventListener.Get(btnWithdraw).onClick = delegate//取款
        {
            if (currentStatu == 1)
                TipManager.Instance.OpenTip(TipType.SimpleTip, "游戏正在进行中，不能取款");
            else
                NodeManager.OpenNode<SafeBoxNode>();
            btnMenu.GetComponent<RadioButton>().SetValue(false);
        };
        UGUIEventListener.Get(btnHelp).onClick = delegate { helpPanel.SetActive(!helpPanel.activeInHierarchy); };
        UGUIEventListener.Get(helpClose).onClick = delegate { helpPanel.SetActive(false); };

        #endregion

        #region ...特殊功能按钮
        UGUIEventListener.Get(btnGameLog).onClick = delegate//游戏记录
        {
            if (currentRound == 1)
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "还没有游戏记录");
                return;
            }
            if (roomType == RoomType.RoomCard)
                NodeManager.OpenNode<GameLogNode>(null, null, false).Inits(roomNo.text);
        };
        UGUIEventListener.Get(btnReady).onClick = delegate//准备
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage() { msgid = MessageId.C2G_TablePrepare });
            readyGroup.SetActive(false);
        };
        UGUIEventListener.Get(btnInvitation).onClick = delegate { NodeManager.OpenNode<InvateNode>().Inits(roomNo.text); };//邀请
        UGUIEventListener.Get(btnRankList).onClick = delegate { NodeManager.OpenNode<MatchRankRewardNode>("match").Inits(roomNo.text); };//比赛排行信息
        UGUIEventListener.Get(btnMatchRule).onClick = delegate { MatchRulesNode.DisplayMatchInfo(); };//比赛规则

        UGUIEventListener.Get(btnChangeTable1).onClick = delegate { ChangeRoom(); };//换桌
        UGUIEventListener.Get(btnChangeTable2).onClick = delegate { ChangeRoom(); };//换桌
        UGUIEventListener.Get(btnShare).onClick = delegate
        {
            string roomTypeStr = "";
            switch (roomType)
            {
                case RoomType.SilverCoin:
                    roomTypeStr = "银币场";
                    break;
                case RoomType.GoldBar:
                    roomTypeStr = "金条场";
                    break;
                default:
                    break;
            }
            Sprite icon = BundleManager.Instance.GetSprite("task/meirirenwu_pic_1");
            SDKManager.Instance.ShareWebPage(SDKManager.WechatShareScene.WXSceneSession,
                UserInfoModel.userInfo.downUrl,
                "雪瑶明水棋牌",
                "我在麻将" + roomTypeStr + "房间中打麻将,快来和我一起玩吧",
                MiscUtils.SizeTextureBilinear(icon.texture, Vector2.one * 150).EncodeToJPG());
        };//分享
        UGUIEventListener.Get(btnContinue).onClick = delegate//继续，关闭结算面板
        {
            resultPanel.SetActive(false);
            majangResult.DelResultInfo();
            if (currentRound != maxRound)
                readyGroup.SetActive(true);
        };
        UGUIEventListener.Get(btnCloseResult).onClick = delegate
        {
            resultPanel.SetActive(false);
            majangResult.DelResultInfo();
            if (currentRound != maxRound)
                readyGroup.SetActive(true);
        };//关闭结算面板
        UGUIEventListener.Get(btnChat).onClick = delegate { NodeManager.OpenNode<ChatNode>(null, null, false); };

        #endregion

        #region ...游戏中功能按钮
        UGUIEventListener.Get(btnTing).onClick = delegate { Operation(btnTing); };
        UGUIEventListener.Get(btnChiTing).onClick = delegate { Operation(btnChiTing); };
        UGUIEventListener.Get(btnPengTing).onClick = delegate { Operation(btnPengTing); };
        UGUIEventListener.Get(btnDingTing).onClick = delegate { Operation(btnDingTing); };
        UGUIEventListener.Get(btnChi).onClick = delegate { Operation(btnChi); };
        UGUIEventListener.Get(btnGang).onClick = delegate { Operation(btnGang); };
        UGUIEventListener.Get(btnPeng).onClick = delegate { Operation(btnPeng); };
        UGUIEventListener.Get(btnGuo).onClick = delegate { Operation(btnGuo); };
        UGUIEventListener.Get(btnCancel).onClick = delegate { Operation(btnCancel); };
        //UGUIEventListener.Get(btnHu).onClick = delegate { };
        #endregion

        #region 游戏中显示
        InvokeRepeating("PhoneInfoShow", 0, 60);
        StartCoroutine(Battery());
        #endregion
    }

    /// <summary>
    /// 手机电量 时间
    /// </summary>
    void PhoneInfoShow()
    {
        timeLb.text = System.DateTime.Now.Hour.ToString("D2") + ":" + System.DateTime.Now.Minute.ToString("D2");
        
    }

    IEnumerator Battery()
    {
        yield return new WaitForSecondsRealtime(1);
        int sdkBattery = SDKManager.Instance.GetBattery();
        if (sdkBattery == -1)
            StartCoroutine(Battery());
        else
            batteryIcon.fillAmount = sdkBattery / 100f;
    }

    Vector3 startClickPos;
    Vector3 endClickPos;

    /// <summary>
    /// 按下语音
    /// </summary>
    void DownYuyin()
    {
        startClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        voiceTipsObj.SetActive(true);
        GVoice.Instance.Click_GetRecFileParam();
        GVoice.Instance.Click_btnReqAuthKey();
        GVoice.Instance.Click_btnStartRecord();
    }
    /// <summary>
    /// 抬起语音
    /// </summary>
    void OnUp()
    {
        endClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (endClickPos.y > startClickPos.y && endClickPos.y - startClickPos.y > 100)
        {//取消发送
            TipManager.Instance.OpenTip(TipType.SimpleTip, "取消发送");
            voiceTipsObj.SetActive(false);
            GVoice.Instance.Click_btnStopRecord();
        }
        else
        {
            voiceTipsObj.SetActive(false);
            GVoice.Instance.Click_btnStopRecord();
            GVoice.Instance.Click_btnUploadFile(filed =>
            {
                SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
                {
                    UserTalk = new UserTalk()
                    {
                        msg = filed,
                        type = 1
                    },
                    msgid = MessageId.C2G_UserTalk
                });
                if (SetNode.chat == 0)
                    TipManager.Instance.OpenTip(TipType.SimpleTip, "您已关闭聊天功能,如要查看聊天信息请在设置里打开聊天功能");
            });
        }
    }


    bool isSendChangeTable = false;//是否已经发送换桌申请
    void ChangeRoom()
    {
        if (!isSendChangeTable)
        {
            isSendChangeTable = true;
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage() { msgid = MessageId.C2G_MjChangeTableReq });
        }
        else
            TipManager.Instance.OpenTip(TipType.SimpleTip, "换桌申请已发出，请勿频繁点击");
    }

    //完成操作后隐藏操作面板
    public void FinishAction()
    {
        foreach (Transform tf in gameMenu.transform)
            tf.gameObject.SetActive(false);
        gameMenu.SetActive(false);
    }

    //刷新倍数
    public void RefreshSurplusCount(int changeCount = -1)
    {
        MaJangScene.surplusCount += changeCount;
        MaJangPage.Instance.surplusCount.text = MaJangScene.surplusCount.ToString();
    }

    void Update()
    {
        if (InputUtils.OnPressed())
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            if (!go || (go.transform.parent.gameObject != btnMenu && go.transform.parent.parent.gameObject != btnMenu))
                btnMenu.GetComponent<RadioButton>().SetValue(false);
        }
    }

    public MaJangPlayer GetPlayerFromSeatNo(int seatNo)
    {
        if (playerCount < 4)
            seatNo *= 2;
        int index = seatNo - tableNoDiff;
        if (index < 0)
            index += 4;
        return playerList[index];
    }

    public MaJangPlayer GetPlayerFromUerId(int userId)
    {
        foreach (MaJangPlayer mjp in playerList)
            if (mjp.userId == userId)
                return mjp;
        return null;
    }

    public void RequestTrusteeship(bool isTrusteeship)
    {
        if (currentPlayer.atTrusteeship != isTrusteeship)
            if (isTrusteeship)
                SocketClient.Instance.AddSendMessageQueue(new C2GMessage() { msgid = MessageId.C2G_MjTuoGuanReq });
            else
                SocketClient.Instance.AddSendMessageQueue(new C2GMessage() { msgid = MessageId.C2G_MjCancelTuoGuanReq });
    }

    //显示所有可操作的按钮,碰 杠等
    public void EnableActionBtn(GameObject btnGo, bool hasGuo = true)
    {
        currentPlayer.isWaitAction = true;
        gameMenu.SetActive(true);
        btnGo.SetActive(true);
        btnGuo.SetActive(hasGuo);
        btnCancel.SetActive(!hasGuo);
    }

    //设置最后打出的牌
    public void SetLastOutMj(MaJangModel mjm = null)
    {
        if (mjm)
        {
            lastOutMj = mjm;
            Vector3 pos = MaJangScene.Instance.sceneCamera.WorldToScreenPoint(mjm.transform.position);
            pos.y += 20;
            lastOutPoint.transform.position = pos;
        }
        else
            lastOutPoint.transform.position = Vector2.one * 3000;
    }

    //设置玩家数量
    void SetPlayerCount(int num, bool isRoomCardType)
    {
        if (isRoomCardType)
        {
            playerList[1].gameObject.SetActive(true);
            playerList[2].gameObject.SetActive(true);
            playerList[3].gameObject.SetActive(true);
        }
        if (num < 4)
        {
            playerList[1].gameObject.SetActive(false);
            playerList[3].gameObject.SetActive(false);
        }
        playerCount = num;
    }

    //设置所有位置的座位号
    void SetAllSeatNo()
    {
        int selfSeatNO = currentPlayer.seatNo;
        int tn;
        for (int i = 1; i < playerList.Length; i++)
        {
            tn = selfSeatNO + i;
            if (tn > 3)
                tn -= 4;
            playerList[i].seatNo = tn;
        }
    }

    //刷新计时器
    public void RefreshTimer()
    {
        timer.gameObject.SetActive(true);
        timer.ResetTimer(true);
        timer.ClearAction();
        //震动
        if (currentPlayer.Equals(currentOperator))
        {
            Debug.LogWarning("我：" + currentPlayer.userId);
            Debug.LogWarning("当前操作：" + currentOperator.userId);
            timer.AddAction(5, () =>
                {
                    HandheldManager.Instance.Vibrate(5, 1);
                });
        }
    }

    public void ShowSelectTingPanel(List<MjTingNum> mjTing)
    {
        tingSelectPanel.SetActive(mjTing.Count > 0);
        Transform ShowPrefab = tingSelectPanel.transform.GetChild(0);
        for (int i = 0; i < mjTing.Count; i++)
        {
            Transform showItem = Instantiate(ShowPrefab, tingSelectPanel.transform);
            showItem.gameObject.SetActive(true);
            int mjNo = mjTing[i].mj.type * 10 + mjTing[i].mj.point;
            showItem.Find("Mj/Icon").GetComponent<Image>().sprite = BundleManager.Instance.GetSprite(mjNo.ToString(), MaJangPage.Instance.majangBundle);
            showItem.GetComponentInChildren<Text>().text = "剩余：" + mjTing[i].num + "张";
        }
    }
    public void ShowSelectTingPanelFinish()
    {
        for (int i = tingSelectPanel.transform.childCount - 1; i >= 0; i--)
            if (tingSelectPanel.transform.GetChild(i).gameObject.activeInHierarchy)
                GameObject.Destroy(tingSelectPanel.transform.GetChild(i).gameObject);
        tingSelectPanel.SetActive(false);
    }

    void OnDestroy()
    {
        Instance = null;
        majangBundle.Unload(true);
        //gamecommonBundle.Unload(true);
    }

    #region ...通信相关
    public void Ready(int seatNo, bool isReady)
    {
        GetPlayerFromSeatNo(seatNo).Ready(isReady);
    }

    public void TrusteeshipResult(bool isTrusteeship, int seatNo = -1)
    {
        if (seatNo == -1)
        {
            seatNo = currentPlayer.seatNo;
            if (playerCount == 2)
                seatNo /= 2;
        }
        GetPlayerFromSeatNo(seatNo).TrusteeshipResult(isTrusteeship);
    }

    public void LeaveByPos(int tableNo, bool isKicked)
    {
        GetPlayerFromSeatNo(tableNo).Leave(isKicked);
    }

    public void LeaveById(int userId, bool isKicked)
    {
        GetPlayerFromUerId(userId).Leave(isKicked);
    }

    public void SelectDisband(bool isDisband)
    {
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_DisbandChooise,
            DisbandChooise = new DisbandChooise() { agree = isDisband }
        });
    }

    public void JoinRoomResult(List<GameUser> userList, RoomDetail detail, bool isChangeTable)
    {
        isSendChangeTable = false;
        currentUserList.AddRange(userList);
        if (isChangeTable)
        {
            foreach (MaJangPlayer mjp in playerList) mjp.Leave(false);
            MaJangScene.Instance.moJongStackParent.gameObject.SetActive(false);
        }
        maxRound = detail.round;
        roomNo.text = detail.tableid;
        roomType = (RoomType)detail.roomType;
        bool isRoomCardType = roomType.Equals(RoomType.RoomCard);
        SetPlayerCount(detail.num, isRoomCardType);

        maxMultiple.text = detail.rate.ToString();
        baseScoreText.text = detail.baseScore.ToString();

        maxMultiple.transform.parent.parent.gameObject.SetActive(isRoomCardType);
        baseScoreText.transform.parent.gameObject.SetActive(!isRoomCardType);
        btnChangeTable1.SetActive(roomType.Equals(RoomType.GoldBar) || roomType.Equals(RoomType.SilverCoin));
        btnInvitation.SetActive(isRoomCardType);
        btnGameLog.SetActive(isRoomCardType);
        roomNo.transform.parent.gameObject.SetActive(isRoomCardType);

        if (roomType.Equals(RoomType.Match))
        {
            commonPanel.SetActive(false);
            matchPanel.SetActive(true);
            readyGroup.SetActive(false);
            btnVoice.SetActive(false);//隐藏语音
            roomNo.text = detail.matcherId;
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_CurrentMatcherInfo,
                currentMatcherInfo = new CurrentMatcherInfo() { matcherId = roomNo.text }
            });
        }
        else
        {
            if (roomType == RoomType.RoomCard)
            {
                btnVoice.SetActive(true);
                UGUIEventListener.Get(btnVoice.gameObject).onDown = delegate { DownYuyin(); };
                UGUIEventListener.Get(btnVoice.gameObject).onUp = delegate { OnUp(); };
            }
            else
            {
                btnVoice.SetActive(false);
            }
            readyGroup.SetActive(true);
            NodeManager.OpenNode<NoticeNode>(null, null, false, false).Inits(Vector3.up * 297f, NoticeNode.NoticeSize.Short);//比赛没有广播
        }

        GameUser currentUser = userList.Find((user) => { return user.uid == UserInfoModel.userInfo.userId; });
        tableNoDiff = currentUser.pos;
        if (detail.num < 4)
            tableNoDiff *= 2;
        MaJangScene.Instance.orientationParent.localEulerAngles = tableNoDiff * ConstantUtils.vecUp90;
        userList.IndexOf(currentUser);

        foreach (MaJangPlayer mjp in playerList)
            mjp.currencyIcon.sprite = currencySprites[(int)roomType - 1];

        GameUser gu;
        for (int i = 0; i < userList.Count; i++)
        {
            gu = userList[i];
            GetPlayerFromSeatNo(gu.pos).Init(gu);
        }
        SetAllSeatNo();
    }

    //其他玩家加入房间
    public void OtherJoinRoomResult(GameUser user)
    {
        currentUserList.Add(user);
        GetPlayerFromSeatNo(user.pos).Init(user);
    }

    //刷新比赛数据
    public void RefreshMatchInfo(MyMatcherRankingResp resp)
    {
        rankText.text = resp.myRank + "/" + resp.totalNum;
        eliminateLine.text = resp.dieScore.ToString();
    }

    //设置player当前状态
    public void SetPlayerStatu(UserStateChangeNotice uscn)// 0在线，1掉线，2离开
    {
        if (uscn.state == 2 || (uscn.state == 1 && currentStatu != 1 && roomType != RoomType.RoomCard))
            GetPlayerFromSeatNo(uscn.pos).Leave(false);
        else
            GetPlayerFromSeatNo(uscn.pos).ConnectChange(uscn.state == 0);
    }

    //其他人互换座位
    public void OtherPlayerChangeSeat(int seatNo1, int seatNo2)
    {
        DataExchange(GetPlayerFromSeatNo(seatNo1), GetPlayerFromSeatNo(seatNo2));
    }

    //换座位请求失败或他人拒绝换位置
    public void ChangeSeatReply(MjChangePosApplyResp mcpap)
    {
        if (mcpap.type == 0 || mcpap.type == 1)
            TipManager.Instance.OpenTip(TipType.SimpleTip, mcpap.type == 0 ? "换座位申请发送失败,请重试！" : "换座位申请发送成功");
        else if (mcpap.type == 2)
        {
            string n = GetPlayerFromUerId(mcpap.posUserId).nickName.text;
            TipManager.Instance.OpenTip(TipType.AlertTip, n + "拒绝了你的换座位请求！");
            MaJangPage.isSendChangeSeatRequest = false;
        }
    }

    //换座位请求成功或收到他人请求
    public void ChangeSeatResult(MjChangePosResp mcpp)
    {
        int targetSeatNo = mcpp.pos;
        if (mcpp.type == 1)
        {
            #region ...向他人请求换位置的回执
            MaJangPlayer mjp1 = playerList[1];
            MaJangPlayer mjp2 = playerList[2];
            MaJangPlayer mjp3 = playerList[3];
            int seatNo = currentPlayer.seatNo;
            if (playerCount == 2)
                targetSeatNo *= 2;
            switch (targetSeatNo - seatNo)
            {
                case -1://上家
                case 3:
                    currentPlayer.seatNo = mjp3.seatNo;
                    mjp3.seatNo = seatNo;

                    DataExchange(mjp1, mjp3);
                    DataExchange(mjp2, mjp3);
                    MaJangScene.Instance.orientationParent.localEulerAngles -= ConstantUtils.vecUp90;
                    break;
                case 1://下家
                case -3:
                    currentPlayer.seatNo = mjp1.seatNo;
                    mjp1.seatNo = seatNo;

                    DataExchange(mjp1, mjp2);
                    DataExchange(mjp2, mjp3);
                    MaJangScene.Instance.orientationParent.localEulerAngles += ConstantUtils.vecUp90;
                    break;
                case -2://对家
                case 2:
                    currentPlayer.seatNo = mjp2.seatNo;
                    mjp2.seatNo = seatNo;

                    DataExchange(mjp1, mjp3);
                    MaJangScene.Instance.orientationParent.localEulerAngles += ConstantUtils.vecUp180;
                    break;
            }
            TipManager.Instance.OpenTip(TipType.SimpleTip, "座位更换成功");
            MaJangPage.isSendChangeSeatRequest = false;
            tableNoDiff = currentPlayer.seatNo;
            //if (playerCount < 4)
            //    tableNoDiff *= 2;
            #endregion
        }
        else if (mcpp.type == 2)
        {
            #region ...他人请求换位置的推送
            string n = GetPlayerFromSeatNo(mcpp.pos).nickName.text;
            TipManager.Instance.OpenTip(TipType.ChooseTip, n + "想和你换座位，是否同意？", 10,
            () => { SendChangSeateReply(2, mcpp.posUserId, mcpp.pos); },
            () => { SendChangSeateReply(3, mcpp.posUserId, mcpp.pos); });
            #endregion
        }
    }

    //发送换位置请求
    void SendChangSeateReply(int type, int newUserId, int newSeatNo)
    {
        int oldSeatNo = currentPlayer.seatNo;
        if (playerCount == 2)
            oldSeatNo /= 2;
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_MjChangePosReq,
            mjChangePosReq = new MjChangePosReq()
            {
                oldPosUserId = newUserId,
                oldPos = newSeatNo,
                posUserId = currentPlayer.userId,
                pos = oldSeatNo,
                type = type
            }
        });
    }

    //两个player互换数据
    void DataExchange(MaJangPlayer player1, MaJangPlayer player2)
    {
        int seatNo = player1.seatNo;
        int userId = player1.userId;
        string name = player1.nickName.text;
        Sprite headIcon = player1.headIcon.sprite;
        string currencyNum = player1.currencyNum.text;
        bool readyIconDis = player1.readyIcon.activeSelf;
        bool vipIconDis = player1.vipIcon.activeSelf;
        bool changeSeatBtnDis = false;
        if (player1.btnChangeSeat)
            changeSeatBtnDis = player1.btnChangeSeat.activeSelf;

        player1.seatNo = player2.seatNo;
        player1.userId = player2.userId;
        player1.nickName.text = player2.nickName.text;
        player1.headIcon.sprite = player2.headIcon.sprite;
        player1.currencyNum.text = player2.currencyNum.text;
        player1.readyIcon.SetActive(player2.readyIcon.activeSelf);
        player1.vipIcon.SetActive(player2.vipIcon.activeSelf);
        if (player1.btnChangeSeat && player2.btnChangeSeat)
            player1.btnChangeSeat.SetActive(player2.btnChangeSeat.activeSelf);

        player2.seatNo = seatNo;
        player2.userId = userId;
        player2.nickName.text = name;
        player2.headIcon.sprite = headIcon;
        player2.currencyNum.text = currencyNum;
        player2.readyIcon.SetActive(readyIconDis);
        player2.vipIcon.SetActive(vipIconDis);
        if (player2.btnChangeSeat)
            player2.btnChangeSeat.SetActive(changeSeatBtnDis);
    }

    /// <summary>
    /// 开始游戏或重连游戏
    /// </summary>
    public void StartOrReConnectGame(GameStartResp gameStartResp)
    {
        currentBaoMj = null;
        foreach (MaJangPlayer mjp in playerList)
            mjp.ClearUI();
        resultPanel.SetActive(false);
        liuJuImage.SetActive(false);
        gangCount = 0;
        SetLastOutMj();
        if (gameStartResp.detail.roomType == (int)RoomType.RoomCard)
        {
            round.gameObject.SetActive(true);
            round.text = gameStartResp.round + "/" + gameStartResp.detail.round;
            currentRound = gameStartResp.round;
        }
        //设置游戏状态
        if (gameStartResp.isReconnect)
            if (gameStartResp.state != 4)
                currentStatu = 1;
            else
                if (gameStartResp.round <= 1)
                    currentStatu = 0;
                else
                    currentStatu = 2;
        else
            currentStatu = 1;

        if (gameStartResp.isReconnect)
        {
            #region ...重连
            List<GameUser> userList = new List<GameUser>();
            for (int i = 0; i < gameStartResp.user.Count; i++)
                userList.Add(gameStartResp.user[i].GameUser);
            JoinRoomResult(userList, gameStartResp.detail, false);

            foreach (GameStartUser gsu in gameStartResp.user)
            {
                MaJangPlayer mjp = GetPlayerFromSeatNo(gsu.GameUser.pos);
                if (currentStatu == 1)
                    mjp.Ready(false);
            }

            if (gameStartResp.state != 4)
            {
                GetPlayerFromSeatNo(gameStartResp.hostPos).SetBanker();
                MaJangScene.Instance.CreateMaJangStack();
                UIUtils.SetAllChildrenLayer(MaJangScene.Instance.diceParent, 0);
                LoadingNode.OpenLoadingNode(LoadingType.Common, "加载数据中...");
                StartCoroutine(MaJangScene.Instance.ThrowDice(gameStartResp.diceAngle, gameStartResp.user.Count, false, () =>
                {
                    List<Transform> smjList = MaJangScene.Instance.stackMaJong;
                    foreach (GameStartUser gsu in gameStartResp.user)
                    {
                        MaJangPlayer mjp = GetPlayerFromSeatNo(gsu.GameUser.pos);
                        if (mjp.btnChangeSeat) mjp.btnChangeSeat.SetActive(false);
                        if (gsu.ting) mjp.Ting();//gsu.tingDropMj
                        bool isCurrentPlayer = mjp.Equals(currentPlayer);
                        bool getMjRecord = false;
                        #region ...手牌
                        for (int i = 0; i < gsu.handMjCount; i++)
                        {
                            if (isCurrentPlayer)
                            {
                                Mj currMj = gsu.hand[i];
                                if (!getMjRecord && gsu.newMj != null && currMj.type == gsu.newMj.type && currMj.point == gsu.newMj.point)
                                    getMjRecord = true;
                                else
                                    mjp.GetMj(currMj);
                            }
                            else
                                mjp.GetMj(null);
                        }
                        #endregion

                        #region ...倒牌
                        foreach (MjEvent me in gsu.@event)
                        {
                            List<MaJangModel> mjmList = new List<MaJangModel>();
                            for (int i = 0; i < me.mj.Count; i++)
                            {
                                int targetIndex = 0;
                                if (i == 3 && me.mj.Count == 4)
                                {
                                    if (MaJangPage.Instance.gangCount % 2 == 0)
                                        targetIndex = smjList.Count - 2;
                                    else
                                        targetIndex = smjList.Count - 1;
                                    MaJangPage.Instance.gangCount++;
                                }

                                MaJangModel mjmTemp = smjList[targetIndex].GetComponent<MaJangModel>();
                                mjmTemp.Init(me.mj[i], mjp);
                                mjmList.Add(mjmTemp);
                                smjList.RemoveAt(targetIndex);
                            }
                            mjp.LightMj(mjmList, me.type == MjEventType.GANG_AN);
                            RefreshSurplusCount(-me.mj.Count);
                        }
                        #endregion

                        #region ...打出的牌
                        foreach (Mj mj in gsu.drop)
                        {
                            MaJangModel mjmTemp = smjList[0].GetComponent<MaJangModel>();
                            mjmTemp.Init(mj, mjp);
                            mjp.OutMj(mjmTemp, null, false);
                            smjList.RemoveAt(0);
                        }
                        RefreshSurplusCount(-gsu.drop.Count);
                        #endregion

                        #region ...听牌
                        if (gsu.ting)
                        {
                            mjp.tingIcon.SetActive(true);
                            if (isCurrentPlayer)
                            {
                                for (int i = 0; i < mjp.handMjList.Count; i++)
                                    mjp.handMjList[i].SetStatu(false);
                                mjp.statu = 2;
                            }
                        }
                        #endregion

                        #region ...行为
                        //待出
                        if (gameStartResp.state == 1 && gsu.state == 1)
                        {
                            MaJangPlayer mjp1 = GetPlayerFromSeatNo(gsu.GameUser.pos);
                            mjp1.GetMj(gsu.newMj, true);
                            if (gsu.zimo)
                                mjp1.Hu(true);
                        }
                        #endregion
                    }
                    if (gameStartResp.judge != null)
                    {
                        MaJangPlayer mjpLast = GetPlayerFromUerId(gameStartResp.judge.owner);
                        SetLastOutMj(mjpLast.outParent.GetChild(mjpLast.outParent.childCount - 1).GetComponent<MaJangModel>());
                    }
                    OpenBao(gameStartResp.changeBaoPaiNotice);
                    MaJangScene.Instance.SetOperator(GetPlayerFromSeatNo(gameStartResp.curPos).seatNo);
                    LoadingNode.CloseLoadingNode();
                    SocketClient.canHandleMessage = true;
                }));
                readyGroup.SetActive(false);
            }
            else
            {
                foreach (GameStartUser gsu in gameStartResp.user)
                {
                    MaJangPlayer mjp = GetPlayerFromSeatNo(gsu.GameUser.pos);
                    if (gsu.GameUser.prepared)
                        mjp.Ready(true);
                    if (mjp.Equals(currentPlayer))
                        readyGroup.SetActive(!gsu.GameUser.prepared);
                }
            }
            #endregion
        }
        else
        {
            #region ...正常开始
            foreach (GameStartUser gsu in gameStartResp.user)
            {
                MaJangPlayer mjp = GetPlayerFromSeatNo(gsu.GameUser.pos);
                if (mjp.btnChangeSeat)
                    mjp.btnChangeSeat.SetActive(currentStatu == 0);
            }

            if (gameStartResp.state == 1)
            {
                for (int i = 0; i < gameStartResp.user.Count; i++)
                    if (gameStartResp.user[i].GameUser.uid == UserInfoModel.userInfo.userId)
                    {
                        currentPlayer.mjList.Clear();
                        currentPlayer.mjList.AddRange(gameStartResp.user[i].hand);
                        Mj newMj = gameStartResp.user[i].newMj;
                        if (newMj != null)
                            currentPlayer.mjList.Add(newMj);
                        break;
                    }
                MaJangPlayer host = GetPlayerFromSeatNo(gameStartResp.hostPos);
                Debug.Log("庄家的位置号：" + host.seatNo + "服务器给的位置" + gameStartResp.hostPos);
                MaJangScene.Instance.Init(gameStartResp.diceAngle, host.seatNo, gameStartResp.hostPos, gameStartResp.user.Count, delegate
                {
                    #region 庄家特效
                    Vector2 local;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(PageManager.Instance.canvas.transform as RectTransform,
                        Camera.main.WorldToScreenPoint(host.bankerIcon.transform.position),
                        PageManager.Instance.canvas.worldCamera, out local))
                    {
                        showBanker.pointList.Clear();
                        showBanker.pointList.Add(Vector3.zero);
                        showBanker.pointList.Add(local);
                        showBanker.pointList.Add(local);
                        showBanker.pointList.Add(local);
                        showBanker.pointEndAction = delegate { showBanker.gameObject.SetActive(false); showBanker.transform.localPosition = Vector3.zero; showBanker.transform.localScale = Vector3.one; host.SetBanker(); };
                        showBanker.gameObject.SetActive(true);
                    }
                    #endregion
                });
            }
            #endregion
        }
        MaJangScene.Instance.moJongStackParent.gameObject.SetActive(true);
    }

    /// <summary>
    /// 开始出牌
    /// </summary>
    public void StartOutMj(UserStepOverNotice uson)
    {
        if (uson.zimo)
            currentPlayer.Hu(true);
        if (uson.mj.Count > 0)
        {
            EnableActionBtn(btnTing, false);
            currentPlayer.SetTingMj(uson.mj);
            if (GetPlayerFromUerId(UserInfoModel.userInfo.userId).atTrusteeship)
            {
                AutoOperation(btnTing);
            }

        }
        if (currentPlayer.HasSame() > 3)
        {
            EnableActionBtn(btnGang, false);
            if (GetPlayerFromUerId(UserInfoModel.userInfo.userId).atTrusteeship)
            {
                AutoOperation(btnGang);
            }
        }
        currentPlayer.isTurn = true;
    }

    /// <summary>
    /// 某玩家摸牌
    /// </summary>
    public void GetUserGetMj(UserGetMjResp ugmr)
    {
        MaJangPlayer mjp = GetPlayerFromSeatNo(ugmr.pos);
        Debug.Log("有摸牌的玩家：" + mjp.seatNo);
        mjp.GetMj(ugmr.mj, true, ugmr.fenZhang, ugmr.canHu, ugmr.isXiaoJi);
        if (mjp.Equals(currentPlayer))
        {
            mjp.isTurn = true;
            mjp.isWaitAction = true;
        }
        if (SetNode.off == 0 && SetNode.read == 1)
            AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.mjgetcard, PageManager.Instance.CurrentPage.name);
        if (mjp.Equals(currentPlayer) && ugmr.dropMj.Count > 0)
        {
            EnableActionBtn(btnTing, false);
            if (GetPlayerFromUerId(UserInfoModel.userInfo.userId).atTrusteeship)
                AutoOperation(btnTing);
            mjp.SetTingMj(ugmr.dropMj);
        }
    }

    /// <summary>
    /// 小鸡下蛋
    /// </summary>
    public void Xjxd(MjResult mjResult)
    {
        foreach (MjResultItem mri in mjResult.item)
            GetPlayerFromSeatNo(mri.pos).ChanegScore(mri.win);
    }

    /// <summary>
    /// 玩家聊天
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="value"></param>
    public void PlayerChat(UserTalkResp resp)//  UserGetMjResp ugmr, string senderName, string value, int type)//0文字1语音2表情
    {
        if (SetNode.chat == 0)
            return;
        if (resp.type == 1)
            SocialModel.Instance.PlayRecord(resp.msg);
        MaJangPlayer mjp = GetPlayerFromSeatNo((int)resp.at);
        //显示在玩家头上
        mjp.Chat(resp.msg, resp.type);
        ChatInfo info = new ChatInfo();
        info.chatWithName = resp.name;
        info.text = resp.msg;
        info.type = resp.type;
        if (resp.type != 2)//不是表情,则显示在聊天界面
            NodeManager.GetNode<ChatNode>().chatPanel.LoadChatItem(info);
        if (resp.type == 0)
        {//判断是否播放常用语语音
            string changyongyuId = string.Empty;
            JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.changyongyuConfig));
            for (int i = 0; i < jd.Count; i++)
            {
                if (resp.msg == jd[i].TryGetString("chatContent"))
                {
                    changyongyuId = jd[i].TryGetString("id");
                    break;
                }
            }
            if (changyongyuId != string.Empty)
            {
                string audioName = (mjp.sex == 0 ? "boy" : "girl") + "_" + changyongyuId;
                AudioManager.Instance.PlayTempSound((AudioManager.AudioSoundType)System.Enum.Parse(typeof(AudioManager.AudioSoundType), audioName));
            }
        }
    }

    /// <summary>
    /// 执行指定用户行为
    /// </summary>
    /// <param name="uan"></param>
    public void GetUserAction(UserActionNotice uan)
    {
        Debug.LogWarning("位置：" + uan.pos);
        Debug.LogWarning("：" + tableNoDiff);

        MaJangPlayer mjp = GetPlayerFromSeatNo(uan.pos);
        MaJangPage.Instance.RefreshTimer();
        MaJangPage.Instance.FinishAction();
        if (!uan.isReconnect)
            switch ((MjActionType)uan.type)
            {
                case MjActionType.OutMj:
                    mjp.OutMj(null, uan.mj[0]);
                    break;
                case MjActionType.Peng:
                    mjp.Peng(uan.mj);
                    break;
                case MjActionType.DGang:
                case MjActionType.AGang:
                case MjActionType.BGang:
                    mjp.Gang(uan.mj, (MjActionType)uan.type);
                    break;
                case MjActionType.Hu:
                    mjp.Hu();
                    break;
                case MjActionType.ZiMo:
                    mjp.Hu(true);
                    break;
                case MjActionType.Guo:
                    break;
                case MjActionType.Chi:
                    mjp.Chi(uan.mj);
                    break;
                case MjActionType.Ting:
                    mjp.OutMj(null, uan.mj[0]);
                    mjp.Ting();
                    break;
                case MjActionType.ChiTing:
                    mjp.ChiTing(uan.mj);
                    break;
                case MjActionType.DingTing:
                    mjp.DingTing(uan.mj);
                    break;
                case MjActionType.PengTing:
                    mjp.PengTing(uan.mj);
                    break;
            }
        if (uan.canHu)
            currentPlayer.Hu();
        else
        {
            if (uan.waitForJudge && currentPlayer.statu == 0)
                Judge(uan.pos, uan.actionType, uan.actionMj);
        }
    }

    /// <summary>
    /// 等待判定
    /// </summary>
    public void Judge(int targetSeatNo, List<int> actionType, List<Mj> mjs)
    {
        foreach (int type in actionType)
        {
            switch ((MjActionType)type)
            {
                case MjActionType.ChiTing:
                    currentPlayer.HasChi(mjs);
                    EnableActionBtn(btnChiTing);
                    break;
                case MjActionType.DingTing:
                    EnableActionBtn(btnDingTing);
                    break;
                case MjActionType.PengTing:
                    EnableActionBtn(btnPengTing);
                    break;
            }
        }
        int sameNum = currentPlayer.HasSame(MaJangPage.Instance.lastOutMj);
        if (sameNum > 2)
            EnableActionBtn(btnPeng);
        if (sameNum > 3)
            EnableActionBtn(btnGang);
        if ((playerCount == 4 && (targetSeatNo - currentPlayer.seatNo == -1 || targetSeatNo - currentPlayer.seatNo == 3))
            || (playerCount == 2 && Mathf.Abs(targetSeatNo * 2 - currentPlayer.seatNo) == 2))
        {
            int chiNum = currentPlayer.HasChi();
            if (chiNum > 0)
                EnableActionBtn(btnChi);
        }
        if (GetPlayerFromUerId(UserInfoModel.userInfo.userId).atTrusteeship)
        {
            GetPlayerFromUerId(UserInfoModel.userInfo.userId).TrusteeshipMethods();
        }
    }

    /// <summary>
    /// 托管后自动操作
    /// </summary>
    void Auto(int targetSeatNo, List<int> actionType, List<Mj> mjs)
    {
        //托管后自动操作
        //1.碰
        int sameNum = currentPlayer.HasSame(MaJangPage.Instance.lastOutMj);
        if (sameNum > 2)
        {
            AutoOperation(btnPeng);
            return;
        }

        //2.吃
        if ((playerCount == 4 && (targetSeatNo - currentPlayer.seatNo == -1 || targetSeatNo - currentPlayer.seatNo == 3))
           || (playerCount == 2 && Mathf.Abs(targetSeatNo * 2 - currentPlayer.seatNo) == 2))
        {
            int chiNum = currentPlayer.HasChi();
            if (chiNum > 0)
            {
                AutoOperation(btnChi);
                return;
            }
        }

        //3.杠
        if (sameNum > 3)
        {
            AutoOperation(btnGang);
            return;
        }

        //4.听
        bool isReturn = false;
        foreach (int type in actionType)
        {
            switch ((MjActionType)type)
            {
                case MjActionType.ChiTing:
                    AutoOperation(btnChiTing);
                    isReturn = true;
                    break;
                case MjActionType.DingTing:
                    AutoOperation(btnDingTing);
                    isReturn = true;
                    break;
                case MjActionType.PengTing:
                    AutoOperation(btnPengTing);
                    isReturn = true;
                    break;
            }
            if (isReturn)
                break;
        }
        return;
    }

    /// <summary>
    /// 游戏操作按钮事件总汇
    /// </summary>
    /// <param name="operationBtn"></param>
    void Operation(GameObject operationBtn)
    {
        currentPlayer.isWaitAction = false;
        currentPlayer.isOutNewCard = true;
        switch (operationBtn.name)
        {
            case "BtnTing":
                currentPlayer.Ting(); FinishAction();
                break;
            case "BtnChiTing":
                currentPlayer.ChiTing(); FinishAction();
                break;
            case "BtnPengTing":
                currentPlayer.PengTing(); FinishAction();
                break;
            case "BtnDingTing":
                currentPlayer.DingTing(); FinishAction();
                break;
            case "BtnChi":
                currentPlayer.Chi(); FinishAction();
                break;
            case "BtnGang":
                currentPlayer.Gang(); FinishAction();
                currentPlayer.isOutNewCard = false;
                break;
            case "BtnPeng":
                currentPlayer.Peng(); FinishAction();
                break;
            case "BtnGuo":
                currentPlayer.CreateAction(MjActionType.Guo); FinishAction();
                break;
            case "BtnCancel":
                chiSelectPanel.SetActive(false); gangSelectPanel.SetActive(false); FinishAction();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 自动操作碰杠听吃等...
    /// </summary>
    void AutoOperation(GameObject btn)
    {
        Operation(btn);
    }

    //显示宝牌、更换宝牌
    public void OpenBao(ChangeBaoPaiNotice cbpn)
    {
        if (cbpn == null)
            return;
        List<Transform> ltf = MaJangScene.Instance.stackMaJong;
        int baoIndex = 0;
        Transform bao;
        if (gangCount % 2 == cbpn.mjPos % 2)
        {
            baoIndex = ltf.Count - cbpn.mjPos + 1;
            bao = ltf[baoIndex];
            Vector3 last = ltf[baoIndex + 2].position;
            ltf[baoIndex + 2].position = ltf[baoIndex + 3].position;
            ltf[baoIndex + 3].position = last;
        }
        else
        {
            baoIndex = ltf.Count - cbpn.mjPos - 1;
            bao = ltf[baoIndex];
            if (currentBaoMj)
            {
                currentBaoMj.transform.localEulerAngles = Vector3.left * ConstantUtils.const90;
                Vector3 last = currentBaoMj.transform.position;
                currentBaoMj.transform.position = bao.position;
                bao.position = last;
            }
        }
        bao.GetComponent<MaJangModel>().Init(cbpn.mj);
        bao.localEulerAngles = Vector3.right * ConstantUtils.const90;
        currentBaoMj = bao.gameObject;
    }
    /// <summary>
    /// 游戏结算
    /// </summary>
    public void GameResult(MjResult mjResult)
    {
        if (roomType.Equals(RoomType.RoomCard))
            currentStatu = 2;
        else
            currentStatu = 0;
        StartCoroutine(GameResultAc(mjResult));
    }

    IEnumerator GameResultAc(MjResult mjResult)
    {
        RequestTrusteeship(false);
        timer.gameObject.SetActive(false);
        currentPlayer.statu = 0;
        foreach (MaJangPlayer mjp in playerList)
            mjp.statu = 0;
        int maxHuType = mjResult.maxHuType;
        yield return new WaitForSecondsRealtime(1);
        SetLastOutMj();

        bool hasResult = mjResult.mjResultRate.Count > 0;

        List<MaJangPlayer> winPlayer = new List<MaJangPlayer>();
        foreach (MjResultItem mri in mjResult.item)
        {
            #region ...加载本局结算信息
            MaJangPlayer mjp = GetPlayerFromSeatNo(mri.pos);
            if (mri.xiaoji != null && !mjp.Equals(currentPlayer))
            {
                Mj xjxdMj = mri.hand.Find((m) => { return m.type == mri.xiaoji.type && m.point == mri.xiaoji.point; });
                mri.hand.Remove(xjxdMj);
                mri.hand.Add(xjxdMj);
            }
            for (int i = 0; i < mri.hand.Count; i++)
            {
                MaJangModel mjm = mjp.handMjList[i];
                if (!mjp.Equals(currentPlayer))
                    mjm.Init(mri.hand[i], mjp);
                mjm.transform.localEulerAngles = Vector3.right * ConstantUtils.const90;
                mjm.SetStatu(true);
                UIUtils.SetAllChildrenLayer(mjm.transform, ConstantUtils.modelLayer);
            }

            //兼容逻辑 客户端如果牌比服务器多，那么删除客户端多掉的牌
            for (int i = 0; i < mjp.handMjList.Count; i++)
            {
                MaJangModel mjm = mjp.handMjList[i];
                if (mjm.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture == null)//以mainTexture标记来找哪个是多的
                {
                    mjp.handMjList.Remove(mjm);
                    i--;
                }
            }

            mjp.LightAnGang();
            mjp.handParent.localPosition -= Vector3.up * 0.35f;

            if (hasResult)
            {
                mjp.currencyNum.text = (int.Parse(mjp.currencyNum.text) + mri.score).ToString();
                mjp.mjri.gameObject.SetActive(true);
                mjp.win = mri.win;
                if (mri.win > 0)
                    winPlayer.Add(mjp);
                if (mjResult.hostIdx == mri.pos)
                    mjp.mjri.bankerIcon.gameObject.SetActive(true);
                majangResult.AddResultInfo(mjp, mri.xiaoji);
            }
            #endregion
        }
        if (hasResult)//有结算
        {
            if (currentBaoMj && currentBaoMj.GetComponent<MaJangModel>() != null && currentBaoMj.GetComponent<MaJangModel>().mjNo != 0)
            {
                majangResult.baopai.transform.parent.gameObject.SetActive(true);
                majangResult.baopai.Init(currentBaoMj.GetComponent<MaJangModel>().mjNo.ToString());
            }
            majangResult.winOrLose(mjResult.mjResultRate);
            MaJangRateMask mjrm = (MaJangRateMask)maxHuType;
            Debug.Log("最大胡牌类型：" + maxHuType + mjrm.ToString());
            for (int i = 0; i < winPlayer.Count; i++)
            {
                if (mjResult.mjResultRate[0].type != 2 || mjResult.mjResultRate[0].type == 2 && mjrm != MaJangRateMask.平胡)
                    MaJangPage.Instance.mje.PlayEffect(maxHuType);
                winPlayer[i].huAnimation(maxHuType);
                if (mjrm == MaJangRateMask.平胡)
                    winPlayer[i].PingHu(mjResult.isZiMo);
            }
            yield return new WaitForSecondsRealtime(3.5f);
            if (SetNode.off == 0 && SetNode.read == 1)
                AudioManager.Instance.PlayTempSound(currentPlayer.win > 0 ? AudioManager.AudioSoundType.mjresultwin : AudioManager.AudioSoundType.mjresultlose, PageManager.Instance.CurrentPage.name);
            if (currentRound != maxRound)
            {
                majangResult.OpenPanel();
            }
            if (roomType.Equals(RoomType.Match))
                resultBtnGroup.SetActive(false);
        }
        else//流局
        {
            liuJuImage.SetActive(true);
            if (SetNode.off == 0 && SetNode.read == 1)
                AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.mjaudioliuju, PageManager.Instance.CurrentPage.name);
            if (currentRound != maxRound)
                readyGroup.SetActive(true);
        }
        if (currentRound == maxRound)
            yield return new WaitForSecondsRealtime(3);
        SocketClient.canHandleMessage = true;
    }
    #endregion

    public void FinishData()
    {
        if (roomType == RoomType.SilverCoin )
        {
            MaJangPlayer player = GetPlayerFromUerId(UserInfoModel.userInfo.userId);
            if (player != null)
                player.currencyNum.text = UserInfoModel.userInfo.walletAgNum.ToString();
        }
        else if (roomType == RoomType.GoldBar) {
            MaJangPlayer player = GetPlayerFromUerId(UserInfoModel.userInfo.userId);
            if (player != null)
                player.currencyNum.text = UserInfoModel.userInfo.walletGoldBarNum.ToString();
        }

    }

    //public MaJangResultOthersItem item;
    //void OnGUI()
    //{
    //    if (GUILayout.Button("   "))
    //    {
    //        item.Create();
    //        item.gameObject.SetActive(true);

    //        SetTimeout.add(3.5f, () =>
    //            {
    //                majangResult.gameObject.SetActive(true);
    //                majangResult.myResultBtn.isOn = true;
    //                majangResult.OpenPanel();
    //            });

    //    }
    //}

    int A = 5;

    int B = 10;  //A+C=B
    void Test()
    {
        int temp = 0;
        for (int i = 0; i < 3; i++)
        {
            temp += Random.Range(-10, 10);
        }
        if (temp + A != B)
            Test();
        else
        {//走到这表示满足 A+C=B

        }
    }

}

public enum MjActionType
{
    // 0出牌,1碰，2点杠，3暗杠，4巴杠，5胡牌，6自摸，7放弃，8吃，飞， 9替用 ，10听 11吃听 12丁听 13碰听
    OutMj = 0,
    Peng,
    DGang,
    AGang,
    BGang,
    Hu,
    ZiMo,
    Guo,
    Chi,
    TiYong,
    Ting,
    ChiTing,
    DingTing,
    PengTing
}
