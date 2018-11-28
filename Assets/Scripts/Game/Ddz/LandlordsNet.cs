using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandlordsNet
{

    static CallBack onMathchSuccess;
    static CallBack _startMathch;
    static CallBack<string> isInRoomCall;
    static CallBack _cancleMathCall;
    /// <summary>
    /// 请求进入房间
    /// </summary>
    /// <param name="_isInRoomCall"></param>
    public static void C2G_IsInRoomReq(CallBack<string> _isInRoomCall)
    {
        isInRoomCall = _isInRoomCall;
        //SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        //{            
        //    msgid = MessageId.C2G_DdzPreEnterRoomReq
        //});
    }
    //public static EnterDdzRoomResp _resp;
    /// <summary>
    /// 有人进入房间应答
    /// </summary>
    /// <param name="resp"></param>
    public static void G2C_EnterRoom(EnterDdzRoomResp resp)
    {
        //_resp = resp;
        if (LandlordsModel.Instance.IsInFight)
            return;
        //房间信息
        if (resp.yuePaiInfo != null)//房卡房
        {
            LandlordsModel.Instance.RoomModel.CurRoomInfo = new LandlordsRoomModel.LandlordsRoomInfo()
            {
                RoomID = resp.yuePaiInfo.rno.ToString(),
                RoomType = RoomType.RoomCard,
                Beishu = resp.yuePaiInfo.fs,
                LeastStore = resp.yuePaiInfo.ante,
                CostCard = resp.yuePaiInfo.ks,
                CurPlayCount = resp.yuePaiInfo.currJs,
                MaxPlayCount = resp.yuePaiInfo.js
            };
            LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch = false;
        }
        else if (resp.matchInfo != null)//比赛房
        {
            LandlordsModel.Instance.RoomModel.CurRoomInfo = new LandlordsRoomModel.LandlordsRoomInfo()
            {
                RoomID = resp.matchInfo.matcherid.ToString(),//比赛ID
                IsQdz = resp.matchInfo.isQdz
            };
            switch (resp.matchInfo.type)
            {
                case 1:// 比赛银币
                    LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType = RoomType.SilverCoin;
                    break;
                case 2://比赛金币
                    LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType = RoomType.GoldBar;
                    break;
                case 3://比赛积分
                    LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType = RoomType.Match;
                    break;
            }
            LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch = true;
        }
        else//游戏币房
        {
            LandlordsModel.Instance.RoomModel.CurRoomInfo = new LandlordsRoomModel.LandlordsRoomInfo()
            {
                RoomID = resp.roomId.ToString(),
                RoomType = LandlordsRoomModel.GetYxbRoomTypeByID(resp.roomId),
                LeastStore = int.Parse(LandlordsRoomModel.GetYxbRoomConfigById(resp.roomId)["ante"].ToString())
            };
            LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch = false;
        }
        bool isRoomCard= LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard;
        //3个玩家信息
        if (LandlordsModel.Instance.RoomPlayerHands != null)
            LandlordsModel.Instance.RoomPlayerHands.Clear();
        for (int i = 0; i < resp.playerBaseInfo.Count; i++)
        {
            PlayerBaseInfo playerInfo = resp.playerBaseInfo[i]; Debug.LogWarning("有玩家进入：" + playerInfo.nickname);
            string uid = playerInfo.userId.ToString();

            LandkirdsHandCardModel hand = LandlordsModel.Instance.CreateHandCardMode(uid, playerInfo.gender == 0 ? Six.boy : Six.girl);
            hand.playerInfo.userNickname = playerInfo.nickname;
            hand.playerInfo.icon = playerInfo.photo;
            if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.SilverCoin)
                hand.playerInfo.money = playerInfo.silver;
            else if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.GoldBar)
                hand.playerInfo.money = playerInfo.gold;
            hand.playerInfo.lv = playerInfo.level;            
            hand.playerInfo.vip = playerInfo.vip;
            hand.playerInfo.win = playerInfo.won;
            hand.playerInfo.lose = playerInfo.lost;
            hand.playerInfo.ratio = (float)playerInfo.rate;
            hand.playerInfo.pos = playerInfo.location;
            hand.playerInfo.exp = playerInfo.exp;
            //hand.playerInfo.isFriend = playerInfo.isFriend;
            hand.MatchScore = playerInfo.currJiFen;
            if (i == 0 && isRoomCard)
                hand.IsRoomer = true;
            if (!hand.IsRoomer)
                hand.IsZhunbei = playerInfo.isPrepared;
        }
        if (resp.playerBaseInfo[resp.playerBaseInfo.Count - 1].userId == UserInfoModel.userInfo.userId)//如果当前进来的是自己
        {            
            LandlordsPage page = LandlordsPage.Instance;
            if (page == null)
                page = PageManager.Instance.OpenPage<LandlordsPage>();
            if (!LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch && resp.playerBaseInfo[0].userId == UserInfoModel.userInfo.userId && LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)//如果自己是房主并且是房卡房
            {
                LandlordsModel.Instance.MyInfo.IsRoomer = true;
                NodeManager.OpenNode<InvateNode>().Inits(LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomID);
            }
            page.InitRoom();
        }
        else
        {
            LandlordsModel.Instance.RoomPlayerSort();
            LandlordsPage.Instance.playView.InitPlayerPrefab();
        }
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            BisaiInfoReq();
    }

    /// <summary>
    /// 请求退出房间
    /// </summary>
    public static void QuiteReq()
    {
        MessageId msgId = MessageId.C2G_DdzLeaveCRTable;
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
        {
            msgId = MessageId.C2G_DdzLeaveCRTable;
        }
        else
        {
            msgId = MessageId.C2G_DdzLeaveRoomReq;
        }
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = msgId
            });
    }

    /// <summary>
    /// 请求该房间所有结算信息
    /// </summary>
    public static void ReqCurRoomAllJiesuanInfo()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_DdzQueryYuePaiResultReq
        });
    }

    /// <summary>
    /// 临时清除房间
    /// </summary>
    public static void LinshiRemoveTable()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid=MessageId.C2G_RemoveCRTable
            });
    }

    /// <summary>
    /// 准备请求0取消1准备
    /// </summary>
    /// <param name="type"></param>
    public static void ZhunbeiReq()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_DdzPrepareReq
            });
    }

    /// <summary>
    /// 开局请求
    /// </summary>
    /// <param name="type"></param>
    public static void StartReq()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_DdzCRTableStart
        });
    } 
   
    /// <summary>
    /// 换桌请求
    /// </summary>
    public static void C2G_ChangeTabelReq()
    {       
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_DdzReplaceTableReq
        });        
    }



    /// <summary>
    /// 叫分请求 0不叫
    /// </summary>
    public static void C2G_PlayerCallReq(int score)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            ddzJiaoFenReq = new DdzJiaoFenReq()
            {
                score = score
            },
            msgid = MessageId.C2G_DdzJiaoFenReq
        });
    }

    /// <summary>
    /// 抢地主请求 0不1抢
    /// </summary>
    public static void C2G_PlayerQiangDzReq(int qdz)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            ddzQdzReq=new DdzQdzReq()
            {
                qdz=qdz
            },
            msgid = MessageId.C2G_DdzQdzReq
        });
    }


    static CallBack popCall;
    static CallBack nopopCall;
    /// <summary>
    /// 出牌请求
    /// </summary>
    /// <param name="type">-1:要不起;0:不出;1;出牌</param>
    public static void C2G_PopReq(int type, List<Card> popCards, CallBack _popCall, CallBack _nopopCall)
    {
        popCall = _popCall;
        nopopCall = _nopopCall;
        DdzChuPaiReq ddzChuPaiReqs = new DdzChuPaiReq();
        ddzChuPaiReqs.act = type;
        if (type == 1)
        {
            for (int i = 0; i < popCards.Count; i++)
            {
                ddzChuPaiReqs.poker.Add(popCards[i].Poker);
            }
        }
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            ddzChuPaiReq = ddzChuPaiReqs,
            msgid = MessageId.C2G_DdzChuPaiReq
        });
    }

    /// <summary>
    /// 托管请求 0取消 1托管
    /// </summary>
    public static void C2G_Tuoguan(int state)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            ddzTuoguanReq = new DdzTuoguanReq()
            {
                tg = state
            },
            msgid = MessageId.C2G_DdzTuoguanReq
        });
    }


    /// <summary>
    /// 投票
    /// </summary>
    /// <param name="isAgree"></param>
    public static void C2G_Vote(bool isAgree)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                ddzVoteReq = new DdzVoteReq()
                {
                    yn = isAgree
                },
                msgid = MessageId.C2G_DdzVoteReq
            });
    }

    /// <summary>
    /// 踢人
    /// </summary>
    /// <param name="isAgree"></param>
    public static void C2G_KickRep(int userId)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            ddzKickPlayerReq = new DdzKickPlayerReq()
            {
                userId = userId
            },
            msgid = MessageId.C2G_DdzKickPlayerReq
        });
    }

    /// <summary>
    /// 请求是否可以离开
    /// </summary>
    /// <param name="isAgree"></param>
    public static void C2G_IsCanLeaveRep()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_DdzCanLeaveReq
        });
    }

    /// <summary>
    /// 返回我是否可以离开  离开类型1.直接离开2.需要发起解散
    /// </summary>
    public static void G2C_IsCanLeaveResp(DdzCanLeaveResp resp)
    {
        if (resp.canLeave)
        {
            QuiteReq();
            PageManager.Instance.OpenPage<MainPage>();
        }
        else
        {
            C2G_JieSanReq();
        }
    }

    /// <summary>
    /// 请求解散房间
    /// </summary>
    /// <param name="isAgree"></param>
    public static void C2G_JieSanReq()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_DdzJieSanReq
        });
    }

    /// <summary>
    /// 断线重连请求
    /// </summary>
    public static void C2G_ReqConnect()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_DdzReconnectReq
            });
    }



    /// <summary>
    /// 断线重连
    /// </summary>
    public static IEnumerator G2C_Reconnect(DdzReconnectResp recontent)
    {
        //临时 true->正在打牌  false->正在准备阶段
        bool isFighting = recontent.stage != 0 && recontent.stage != 4;
        LandlordsModel.Instance.IsInFight = isFighting;
        OrderController.Instance.Clear();
        //房间信息
        if (recontent.yuePaiInfo != null)//房卡房
        {
            LandlordsModel.Instance.RoomModel.CurRoomInfo = new LandlordsRoomModel.LandlordsRoomInfo()
                {
                    RoomID = recontent.yuePaiInfo.rno.ToString(),
                    RoomType = RoomType.RoomCard,
                    Beishu = recontent.yuePaiInfo.fs,
                    LeastStore = recontent.yuePaiInfo.ante,
                    CostCard = recontent.yuePaiInfo.ks,
                    CurPlayCount = recontent.yuePaiInfo.currJs,
                    MaxPlayCount = recontent.yuePaiInfo.js
                };
            LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch = false;
        }
        else if (recontent.matchInfo != null)//比赛房
        {
           LandlordsModel.Instance.RoomModel.CurRoomInfo = new LandlordsRoomModel.LandlordsRoomInfo()
            {
                RoomID = recontent.matchInfo.matcherid.ToString(),//比赛ID
                IsQdz = recontent.matchInfo.isQdz
            };
           switch (recontent.matchInfo.type)
            {
                case 1:// 比赛银币
                    LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType = RoomType.SilverCoin;
                    break;
                case 2://比赛金币
                    LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType = RoomType.GoldBar;
                    break;
                case 3://比赛积分

                    break;
                default:
                    break;
            }
            LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch = true;
        }
        else//游戏币房
        {
            LandlordsModel.Instance.RoomModel.CurRoomInfo = new LandlordsRoomModel.LandlordsRoomInfo()
            {
                RoomID = recontent.roomId.ToString(),
                RoomType = LandlordsRoomModel.GetYxbRoomTypeByID(recontent.roomId),
                LeastStore = int.Parse(LandlordsRoomModel.GetYxbRoomConfigById(recontent.roomId)["ante"].ToString())
            };
            LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch = false;
        }


        //3个玩家信息
        if (LandlordsModel.Instance.RoomPlayerHands != null)
            LandlordsModel.Instance.RoomPlayerHands.Clear();
        bool isRoomCard = LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard;
        for (int i = 0; i < recontent.ddzPlayerInfo.Count; i++)
        {
            DdzPlayerInfo playerInfo = recontent.ddzPlayerInfo[i];
            string uid = playerInfo.playerBaseInfo.userId.ToString();

            LandkirdsHandCardModel hand = LandlordsModel.Instance.CreateHandCardMode(uid, playerInfo.playerBaseInfo.gender == 0 ? Six.boy : Six.girl);
            hand.playerInfo.userNickname = playerInfo.playerBaseInfo.nickname;
            hand.playerInfo.icon = playerInfo.playerBaseInfo.photo;
            if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.SilverCoin)
                hand.playerInfo.money = playerInfo.playerBaseInfo.silver;
            else if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.GoldBar)
                hand.playerInfo.money = playerInfo.playerBaseInfo.gold;
            hand.playerInfo.lv = playerInfo.playerBaseInfo.level;
            hand.playerInfo.vip = playerInfo.playerBaseInfo.vip;
            hand.playerInfo.win = playerInfo.playerBaseInfo.won;
            hand.playerInfo.lose = playerInfo.playerBaseInfo.lost;
            hand.playerInfo.ratio = (float)playerInfo.playerBaseInfo.rate;
            hand.playerInfo.pos = playerInfo.playerBaseInfo.location;
            hand.playerInfo.exp = playerInfo.playerBaseInfo.exp;
            //hand.playerInfo.isFriend = playerInfo.playerBaseInfo;
            hand.MatchScore = playerInfo.playerBaseInfo.currJiFen;
            if (i == 0 && isRoomCard)
                hand.IsRoomer = true;
            if (!hand.IsRoomer)
                hand.IsZhunbei = playerInfo.playerBaseInfo.isPrepared;

            hand.isTuoguan = playerInfo.tg == 1;
        }

        if (isFighting)
        {
            FightReconect(recontent);
        }
        else
        {
            NormalReconect();
        }
        yield return new WaitForSecondsRealtime(2);
        UserInfoModel.userInfo.inDzz = false;
    }

    /// <summary>
    /// 战斗重连
    /// </summary>
    /// <param name="recontent"></param>
    private static void FightReconect(DdzReconnectResp recontent)
    {
        //3个玩家牌 非主角只发送牌数量
        List<LandkirdsHandCardModel> hands = LandlordsModel.Instance.RoomPlayerHands;
        for (int i = 0; i < hands.Count; i++)
        {
            hands[i].Clear();
            DdzPlayerInfo playerInfo = recontent.ddzPlayerInfo[i];
            if (playerInfo.playerBaseInfo.userId == UserInfoModel.userInfo.userId)
            {
                for (int j = 0; j < playerInfo.poker.Count; j++)
                {
                    Card card = new Card(playerInfo.poker[j], UserInfoModel.userInfo.userId.ToString());
                    hands[i].AddCard(card);
                }
            }
            else
            {
                for (int j = 0; j < playerInfo.zs; j++)//这里数量根据服务器来
                {
                    hands[i].AddCard(null);
                }
            }

        }

        LandlordsModel.Instance.RoomPlayerSort();
        
        
        if (PageManager.Instance.GetPage<LandlordsPage>() == null)
            PageManager.Instance.OpenPage<LandlordsPage>();

        LandlordsPage.Instance.InitRoom();
        LandlordsModel.Instance.RoomPlayerSort();        
        
        LandlordsPage.Instance.GameStart();//这里会清除3张底牌
        LandlordsPage.Instance.Multiples = recontent.qbs;
        LandlordsPage.Instance.Dizhu = LandlordsModel.Instance.RoomModel.CurRoomInfo.LeastStore;
        LandlordsPage.Instance.playView.InitPayerLibrary();

        //发底牌
        DzCard.Instance.Clear();
        for (int i = 0; i < recontent.dzPoker.Count; i++)
        {
            DzCard.Instance.AddCard(new Card(recontent.dzPoker[i], "-9999"));
        }
        //玩家变地主
        if (recontent.dzPoker.Count == 3)
        {
            LandlordsPage.Instance.PlayerToLandlord(recontent.ddzPlayerInfo[0].playerBaseInfo.userId.ToString(), false);
        }
        ////1发牌2叫分（抢地主）3出牌5结算
        //if (recontent.stage != 1 && recontent.stage != 5)
        //    OrderController.Instance.CurInterationType = (InterationType)recontent.stage;

        OrderController.Instance.Turn(recontent.currUserId.ToString(), (int)OrderController.Instance.CurInterationType);

        //谁是托管
        for (int i = 0; i < recontent.ddzPlayerInfo.Count; i++)
        {
            LandlordsPage.Instance.PlayerTuoGuan(recontent.ddzPlayerInfo[i].playerBaseInfo.userId.ToString(), recontent.ddzPlayerInfo[i].tg == 1);
        }


    }

    /// <summary>
    /// 普通重连
    /// </summary>
    private static void NormalReconect()
    {
        if (PageManager.Instance.GetPage<LandlordsPage>() == null)
            PageManager.Instance.OpenPage<LandlordsPage>();
        LandlordsModel.Instance.RoomPlayerSort();
        LandlordsPage.Instance.InitRoom();
    }

    /// <summary>
    /// 请求比赛信息
    /// </summary>
    public static void BisaiInfoReq()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                currentMatcherInfo = new CurrentMatcherInfo()
                {
                    matcherId = LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomID
                },
                msgid = MessageId.C2G_CurrentMatcherInfo
            });
    }

    /// <summary>
    /// 收到比赛信息
    /// </summary>
    public static void BisaiInfoResp(CurrentMatcherInfoResp resp)
    {
        if (LandlordsPage.Instance != null)
            LandlordsPage.Instance.componentView.FreshMatchInfo(resp);
    }
   

    /// <summary>
    /// 收到我的排行信息
    /// </summary>
    public static void BisaiMyRankInfoResp(MyMatcherRankingResp resp)
    {
        if (LandlordsPage.Instance != null)
            LandlordsPage.Instance.componentView.FreshMatchRankInfo(resp);
    }


    #region  推送   
    /// <summary>
    /// 推送有玩家准备/取消准备
    /// </summary>
    /// <param name="resp"></param>
    public static void G2C_ZhunbeiResp(DdzPrepareResp zhunbei)
    {
        if (!LandlordsModel.Instance.IsInFight)
            LandlordsPage.Instance.PlayerZhunbei(zhunbei.userId.ToString(), true);
    }

    /// <summary>
    /// 有玩家退出
    /// </summary>
    public static void G2C_LeaveRoomResp(string uid, int reson)
    {
        if (LandlordsModel.Instance.RoomPlayerHands == null)
            return;
        LandkirdsHandCardModel player = LandlordsModel.Instance.RoomPlayerHands.Find(p => p != null && p.playerInfo.uid == uid);
        LandlordsPage.Instance.PlayerExit(uid, reson == 1);
        if (player != null)
        {
            int index = LandlordsModel.Instance.RoomPlayerHands.IndexOf(player);
            LandlordsModel.Instance.RoomPlayerHands[index] = null;
        }
        if (uid == UserInfoModel.userInfo.userId.ToString() || uid == "-1")
        {
            if (reson == 1)
                TipManager.Instance.OpenTip(TipType.SimpleTip, "您被踢出房间");
            else if (reson == 2)
                TipManager.Instance.OpenTip(TipType.SimpleTip, "房间解散");
            else if (reson == 3)
                TipManager.Instance.OpenTip(TipType.SimpleTip, "您的货币不足");
            PageManager.Instance.OpenPage<MainPage>();
        }
    }

    /// <summary>
    /// 推送我被踢
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="isKick"></param>
    public static void MeKicked()
    {
        TipManager.Instance.OpenTip(TipType.AlertTip, "您已被提出房间 ");
        PageManager.Instance.OpenPage<MainPage>();
    }

    /// <summary>
    /// 推送发牌
    /// </summary>
    public static void G2C_DealCard(DdzFaPaiResp fapai)
    {
        LandlordsModel.Instance.IsInFight = true;
        NodeManager.CloseTargetNode<JiesanNode>();
        LandlordsModel.Instance.RoomPlayerSort();
        LandlordsPage.Instance.GameStart();
        List<LandkirdsHandCardModel> hands = LandlordsModel.Instance.RoomPlayerHands;
        for (int i = 0; i < hands.Count; i++)
        {
            hands[i].Clear();
            if (hands[i].playerInfo.uid == UserInfoModel.userInfo.userId.ToString())
            {
                Debug.LogWarning("发牌数量：" + fapai.poker.Count);
                for (int j = 0; j < fapai.poker.Count; j++)
                {
                    Card card = new Card(fapai.poker[j], UserInfoModel.userInfo.userId.ToString());
                    hands[i].AddCard(card);
                }
            }
            else
            {
                for (int j = 0; j < 17; j++)
                {
                    hands[i].AddCard(null);
                }
            }
        }
        //这里可能没有主角,防止意外先创建
        LandlordsModel.Instance.CreateHandCardMode(UserInfoModel.userInfo.userId.ToString(), UserInfoModel.userInfo.sex == 0 ? Six.boy : Six.girl);
        int type = LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.GoldBar ? 2 : 0;
        OrderController.Instance.Turn(fapai.nextUserId.ToString(), type);
        LandlordsPage.Instance.playView.gameObject.SetActive(true);
        LandlordsPage.Instance.playView.InitPayerLibrary();
    }

    /// <summary>
    /// 推送该我叫分
    /// </summary>
    public static void G2C_MeCallFen()
    {
        OrderController.Instance.Init(UserInfoModel.userInfo.userId.ToString(), 2);
    }

    /// <summary>
    /// 推送该我叫地主
    /// </summary>
    public static void G2C_MeCallLandlords()
    {
        OrderController.Instance.Init(UserInfoModel.userInfo.userId.ToString(), 0);
    }

    /// <summary>
    /// 推送该我出牌
    /// </summary>
    public static void G2C_MePop()
    {
        OrderController.Instance.Init(UserInfoModel.userInfo.userId.ToString(), 3);
    }

    /// <summary>
    /// 推送叫分
    /// </summary>
    public static void G2C_PlayerCallResp(DdzJiaoFenResp jiaofen)
    {
        string uid = jiaofen.userId.ToString();
        string next_uid = jiaofen.nextUserId.ToString();
        int score = jiaofen.score[0];
        LandlordsPage.Instance.CallLandlord(uid, score);
        if (jiaofen.dzId != 0)
        {
            //发底牌
            DzCard.Instance.Clear();
            for (int i = 0; i < jiaofen.dzPoker.Count; i++)
            {
                DzCard.Instance.AddCard(new Card(jiaofen.dzPoker[i], uid));
            }
            //变地主
            LandlordsPage.Instance.PlayerToLandlord(jiaofen.dzId.ToString());
        }
        OrderController.Instance.Turn(next_uid, jiaofen.nextOper);
    }

    /// <summary>
    /// 推送抢地主
    /// </summary>
    public static void G2C_PlayerQdzResp(DdzQdzResp qdz)
    {
        if (qdz.cxfp)
        {
            LandlordsPage.Instance.LandlordsClearFight();
            return;
        }
        string uid = qdz.userId.ToString();
        string next_uid = qdz.nextUserId.ToString();
        if (qdz.isFirst)
            LandlordsPage.Instance.JiaoDizhu(uid, qdz.qdz == 1);
        else
            LandlordsPage.Instance.QiangDizhu(uid, qdz.qdz == 1);
        if (qdz.dzId != 0)
        {
            //发底牌
            DzCard.Instance.Clear();
            for (int i = 0; i < qdz.dzPoker.Count; i++)
            {
                DzCard.Instance.AddCard(new Card(qdz.dzPoker[i], uid));
            }
            //变地主
            LandlordsPage.Instance.PlayerToLandlord(qdz.dzId.ToString());
        }
        OrderController.Instance.Turn(next_uid, qdz.nextOper);
    }

    /// <summary>
    /// 推送出牌 // -1:要不起;0:不出;1;出牌
    /// </summary>
    /// <param name="chupai"></param>
    public static void G2C_PopResp(DdzChuPaiResp chupai)
    {
        if (chupai.userId == UserInfoModel.userInfo.userId)
        {
            if (chupai.act == 1 && popCall != null)
                popCall();
            else if (nopopCall != null)
                nopopCall();
        }
        switch (chupai.act)
        {
            case -1:
            case 0:
                LandlordsPage.Instance.NoPopCard(chupai.userId.ToString(), chupai.act);
                break;
            case 1:
                List<Card> cards = new List<Card>();
                for (int i = 0; i < chupai.poker.Count; i++)
                {
                    cards.Add(new Card(chupai.poker[i], chupai.userId.ToString()));
                }
                LandlordsPage.Instance.PopCard(chupai.userId.ToString(), cards);
                for (int i = 0; i < cards.Count; i++)
                {
                    LandlordsPage.Instance.componentView.jipaiqiPanel.SetValue(cards[i].GetCardWeight);
                }
                break;
        }
        OrderController.Instance.Turn(chupai.nextUserId.ToString(), chupai.nextOper);
    }

    /// <summary>
    /// 托管推送
    /// </summary>
    public static void G2C_TuoguanResp(DdzTuoguanResp tuoguan)
    {
        string tuoGuanuid = tuoguan.userId.ToString();
        int state = tuoguan.reason;//0取消1托管        
        LandlordsPage.Instance.PlayerTuoGuan(tuoGuanuid, state != 0);
    }

    /// <summary>
    /// 聊天推送 //0文字1语音2表情
    /// </summary>
    public static void G2C_ChatResp(DdzChatResp resp)
    {
        if (SetNode.chat == 0)
            return;
        LandlordsPage.Instance.PlayerChat(resp.userId.ToString(), resp.nickname, resp.ddzChatContent.text, resp.type);
    }

    /// <summary>
    /// 推送结算
    /// </summary>
    public static void G2C_Result(DdzJieSuanResp result)
    {
        LandlordsModel.Instance.IsInFight = false;
        DdzJieSuanInfo juesuanInfo = result.jieSuanInfo;
        //yield return new WaitForSecondsRealtime(0.1f);
        LandlordsModel.Instance.ResultModel.zd = result.jieSuanInfo.zdbs;
        LandlordsModel.Instance.ResultModel.jdz = result.jieSuanInfo.dzbs;
        LandlordsModel.Instance.ResultModel.ct = result.jieSuanInfo.ct ? 2 : 0;
        LandlordsModel.Instance.ResultModel.fct = result.jieSuanInfo.fc ? 2 : 0;
        for (int i = 0; i < juesuanInfo.playerInfo.Count; i++)
        {
            DdzJSPlayerInfo playerIcom = juesuanInfo.playerInfo[i];
            LandlordsModel.Instance.ResultModel.Add(playerIcom);
            if (playerIcom.income > 0)
                LandlordsModel.Instance.CurWinerIds.Add(playerIcom.userId);

            if (LandlordsModel.Instance.RoomModel.CurRoomInfo.IsMatch)
            {
                LandlordsModel.Instance.RoomPlayerHands.Find(p => p.playerInfo.uid == juesuanInfo.playerInfo[i].userId.ToString()).MatchScore += juesuanInfo.playerInfo[i].income;
            }
            else if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
                LandlordsModel.Instance.RoomPlayerHands.Find(p => p.playerInfo.uid == juesuanInfo.playerInfo[i].userId.ToString()).playerInfo.score += juesuanInfo.playerInfo[i].income;
            else
            {
                LandlordsModel.Instance.RoomPlayerHands.Find(p => p.playerInfo.uid == juesuanInfo.playerInfo[i].userId.ToString()).playerInfo.money = juesuanInfo.playerInfo[i].coin;
            }
        }
        LandlordsModel.Instance.ResultModel.curJs = result.jieSuanInfo.currJs;
        LandlordsModel.Instance.ResultModel.allJs = result.jieSuanInfo.totalJs;

        float delay = 0;
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType != RoomType.Match)
            delay = 1.5f;
        else
            delay = 0;
        SetTimeout.add(delay, () =>
            {
                CheckIsChuntianOrReverse(juesuanInfo);
                //总显示
                LandlordsPage.Instance.GameOver();
            });
    }

    /// <summary>
    /// 检查春天反春天
    /// </summary>
    static void CheckIsChuntianOrReverse(DdzJieSuanInfo result)
    {
        //反春天
        if (result.fc )
        {
            LandlordsPage.Instance.Multiples *= 2;
            NodeManager.OpenNode<LandlordsEffectNode>(null, null, false).Inits(LandlordsEventAniType.FanChunTian, 0, 0);
        }
        //春天
        else if (result.ct)
        {
            NodeManager.OpenNode<LandlordsEffectNode>(null, null, false).Inits(LandlordsEventAniType.ChunTian, 0, 0);
            LandlordsPage.Instance.Multiples *= 2;
        }
    }

    /// <summary>
    /// 推送解散的结果
    /// </summary>
    public static void G2C_JiesanResult(bool isJiesan)
    {
        if (isJiesan)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "房间解散成功");
            //PageManager.Instance.OpenPage<MainPage>();
        }
        else
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "未达成一致同意，解散失败");
        }
    }


    #endregion
}
