using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections.Generic;
using ProtoBuf;
using net_protocol;
using System.Collections.Specialized;
using System.IO;
using UnityEngine.Events;

public class SocketClient : MonoBehaviour
{
    private static SocketClient instance;
    public static SocketClient Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<SocketClient>();
                go.name = instance.GetType().ToString();
                if (instance.Init())
                    UIUtils.Log("建立连接成功!");
                else
                {
                    TipManager.Instance.OpenTip(TipType.AlertTip, "网络错误，请检查您的网络状况");
                    instance = null;
                    Destroy(go);
                }
            }
            return instance;
        }
        set { instance = value; }
    }

    public static string ip;
    public static int testLogicId;
    public static int port = 9001;
    public static bool canHandleMessage = true;
    const int packageMaxLength = 1024;
    static bool hadHeart = false;

    Socket mSocket;
    Thread threadSend;
    Thread threadRecive;
    float lastHeartTime = 0;
    /// <summary>
    /// 判断当前连接状态 -1-未开始连接，0-连接失败，1-连接成功
    /// </summary>
    int isConnected = -1;
    int reconnectTime = 0;//重连次数
    Queue<G2CMessage> allPackages;
    List<byte[]> sendList;

    bool Init()
    {
        ip = PageManager.Instance.ips[PageManager.Instance.ipIndex];
        testLogicId = PageManager.Instance.testLoginIds;
        allPackages = new Queue<G2CMessage>();
        sendList = new List<byte[]>();
        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        return SocketConnection();
    }

    /// <summary>
    /// 每5秒添加心跳包
    /// </summary>
    /// <returns></returns>
    IEnumerator AddHeartPackage()
    {
        yield return new WaitForSecondsRealtime(5);
        AddSendMessageQueue(new C2GMessage { msgid = MessageId.C2G_HeartBeat });
        StartCoroutine(AddHeartPackage());
    }

    /// <summary>
    /// 建立服务器连接
    /// </summary>
    bool SocketConnection()
    {
        try
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), port);
            IAsyncResult asyncresult = mSocket.BeginConnect(ipep, ConnectCallBack, mSocket);
            if (asyncresult.AsyncWaitHandle.WaitOne(10000, false))
            {
                while (isConnected == -1)
                    Thread.Sleep(100);
                if (isConnected == 1)
                {
                    threadSend = new Thread(new ThreadStart(SendMessage));
                    threadSend.Start();
                    threadRecive = new Thread(new ThreadStart(ReceiveMessage));
                    threadRecive.Start();
                    StartCoroutine(AnalysisMessage());
                    if (!hadHeart)
                    {
                        StartCoroutine(AddHeartPackage());
                        hadHeart = true;
                    }
                    return true;
                }
                else
                    return false;
            }
            else
            {
                Close();
                OpenTipNetError();
                return false;
            }
        }
        catch (Exception e)
        {
            UIUtils.Log(e.ToString());
            Close();
            OpenTipNetError();
            return false;
        }
    }

    /// <summary>
    /// 连接服务器的回调
    /// </summary>
    /// <param name="asyncresult"></param>
    void ConnectCallBack(IAsyncResult asyncresult)
    {
        try
        {
            isConnected = -1;
            Socket socketClient = asyncresult.AsyncState as Socket;
            if (socketClient != null)
            {
                socketClient.EndConnect(asyncresult);
                isConnected = 1;
            }
        }
        catch (Exception e)
        {
            isConnected = 0;
            UIUtils.Log(e.ToString());
        }
        finally
        {
            asyncresult.AsyncWaitHandle.Close();
        }
    }

    #region ...发送消息
    /// <summary>
    /// 添加数据到发送队列
    /// </summary>
    /// <param name="c2g">消息主体</param>
    /// <param name="isPrevent">是否显示loading</param>
    public void AddSendMessageQueue(C2GMessage c2g, bool isPrevent = false)
    {
        if (!PageManager.Instance.isCanSend)
            return;

        if (isPrevent && mSocket.Connected)
            LoadingNode.OpenLoadingNode(LoadingType.Common);
        c2g.testLogicId = testLogicId;
        byte[] bytes = BuildPackage(c2g);
        if (sendList.Count > 0)
            if (!sendList.Contains(bytes))
                sendList.Add(bytes);
            else
                UIUtils.Log("消息重复");
        else
            sendList.Add(bytes);
        if (c2g.msgid != MessageId.C2G_HeartBeat)
            UIUtils.Log("发送消息：" + c2g.msgid.ToString());
    }

    void SendMessage()
    {
        while (true)
        {
            if (sendList.Count == 0)
            {
                Thread.Sleep(50);
                continue;
            }
            if (!mSocket.Connected && !IsConnected())
            {
                Loom.QueueOnMainThread(() => { StartCoroutine(Reconnect()); });
                break;
            }
            else
                Send(sendList[0]);
        }
    }

    void Send(byte[] bytes)
    {
        try
        {
            mSocket.Send(bytes, SocketFlags.None);
            sendList.RemoveAt(0);
        }
        catch (SocketException ex)
        {
            if (ex.NativeErrorCode.Equals(10035))
                Send(bytes);
            else
            {
                Loom.QueueOnMainThread(() => { TipManager.Instance.OpenTip(TipType.SimpleTip, "请求失败!"); });
            }
        }
    }
    #endregion

    #region ...接收消息
    /// <summary>
    /// 解析收到的消息
    /// </summary>
    IEnumerator AnalysisMessage()
    {
        while (true)
        {
            if (allPackages.Count > 0)
            {
                G2CMessage message = allPackages.Dequeue();
                MessageId messageId = message.msgid;
                if (messageId != MessageId.G2C_HeartBeatResp && messageId != MessageId.G2C_SystemNotice)
                {
                    LoadingNode.CloseLoadingNode();
                    UIUtils.Log("收到消息：" + messageId.ToString() + ",result:：" + message.result);
                }
                if (message.result == 1)
                {
                    #region ...消息处理
                    switch (messageId)
                    {
                        #region ...登录注册相关
                        case MessageId.G2C_Kick_ReLogin:
                            LoginPage.TakeLogin();
                            break;
                        case MessageId.G2C_Kick:
                            TipManager.Instance.OpenTip(TipType.ChooseTip, "您被踢出登录是否重登?", 0, () =>
                                {
                                    PageManager.Instance.OpenPage<LoginPage>();
                                });
                            break;
                        case MessageId.G2C_LoginResp:
                            UserInfoModel.userInfo.inDzz = message.loginresp.inDdz;
                            UserInfoModel.userInfo.release = message.loginresp.vno;
                            LoginPage.LoginResult(message.loginresp.token);
                            print("是否在斗地主:" + message.loginresp.inDdz);
                            break;
                        case MessageId.G2C_UserFlushData:
                            UserInfoModel.userInfo.InitUserData(message.flushdata);
                            break;
                        case MessageId.G2C_UserSysNotice:
                            //message.UserSysNotice.msg
                            break;
                        #endregion
                        #region  大厅相关
                        case MessageId.G2C_QueryAmountOfPlayerInGameResp:
                            var mainPage = PageManager.Instance.GetPage<MainPage>();
                            if (mainPage)
                                mainPage.SetOnlineNum(message.queryAmountOfPlayerInGameResp);
                            break;
                        case MessageId.G2C_QueryUserMsgResp:
                            MessageModel.Instance.QueryUserMsgFinish(message.UserMsg);
                            break;
                        case MessageId.G2C_ReadUserMsgResp:
                            NodeManager.GetNode<MessageNode>().ReadMsg(message.ReadMsgResp);
                            break;
                        case MessageId.G2C_QueryGoodsResp:
                            NodeManager.GetNode<BagNode>().goodsPanel.SetData(message.QueryGoodsResp.goods);
                            break;
                        case MessageId.G2C_UseGoodsResp:
                            NodeManager.GetNode<BagNode>().goodsPanel.cashPanel.OnClickTrueFinish(message.UseGoodsResult.result);
                            break;
                        case MessageId.G2C_SaleUserGoodsResp:
                            NodeManager.GetNode<BagNode>().goodsPanel.salePanel.OnClickTrueFinish(message.SaleUserGoodsResp.result);
                            break;
                        case MessageId.G2C_QueryTickRecordResp:
                            NodeManager.GetNode<BagNode>().redeemPanel.QueryTickFinish(message.QueryTickRecordResp);
                            break;
                        case MessageId.G2C_UpdateUserSafeBoxResp:
                            NodeManager.GetNode<SafeBoxNode>().OnClickTrueFinish(message.UpdateUserSafeBoxResp.result);
                            break;
                        case MessageId.G2C_BuyGoodsInStoreResp://购买物品
                            StoreNode.ShowRechargeResult(message.BuyGoodsInStoreResp);
                            break;
                        case MessageId.G2C_ChargeNotic://sdk购买回调
                            StoreNode.ChargeNoticFinish(message.ChargeNotice);
                            break;
                        case MessageId.G2C_QueryRankResp:
                            RankNode.QueryRankFinish(message.queryRankResp);
                            break;
                        case MessageId.G2C_UpdateUser:
                            TipManager.Instance.OpenTip(TipType.SimpleTip, "操作成功");
                            break;
                        case MessageId.G2C_IsVipUserResp:
                            UserInfoNode.FinishVipDay(message.IsVipUserResp);
                            BqPanel.FinishVipDay(message.IsVipUserResp);
                            break;
                        case MessageId.G2C_QueryTableInfoResp://请求房间信息
                            JoinGameRoonNode.G2C_EnterSuccess(message.queryTableInfoResp);
                            break;
                        case MessageId.G2C_YuePaiTableResp:
                            if (MaJangPage.Instance)
                            {
                                int finalIndex = message.yuePaiTableResp.yuePaiTable.Count - 1;
                                GameLogNode.G2C_Load(message.yuePaiTableResp.yuePaiTable[finalIndex]);
                            }
                            else if (PageManager.Instance.CurrentPage is LandlordsPage)
                            {
                                int finalIndex = message.yuePaiTableResp.yuePaiTable.Count - 1;
                                CardResultShowNode cardResultNode = NodeManager.GetNode<CardResultShowNode>();
                                if (cardResultNode)
                                    cardResultNode.Inits(message.yuePaiTableResp.yuePaiTable[finalIndex]);
                                else
                                    GameLogNode.G2C_Load(message.yuePaiTableResp.yuePaiTable[finalIndex]);
                            }
                            else if (PageManager.Instance.CurrentPage is MainPage)
                                YuepaiLogNode.G2C_ReceiveLog(message.yuePaiTableResp);
                            break;
                        case MessageId.G2C_UnifiedOrder:
                            SDKManager.Instance.SendWechatPay(message.UnifiedOrderResp);
                            break;
                        case MessageId.G2C_AliOrder:
                            SDKManager.Instance.SendAliPay(message.AliOrderResp);
                            break;
                        case MessageId.G2C_QueryGameRoomResp:
                            SelectRoomPage.Instance.QueryGameRoomFinish(message.queryGameRoomResp);
                            break;
                        case MessageId.G2C_SystemNotice://系统消息
                            for (int i = 0; i < message.SystemNotice.notice.Count; i++)
                            {
                                NoticeNode.Add(message.SystemNotice.notice[i]);
                            }
                            break;
                        #region 好友相关
                        case MessageId.G2C_QueryFriendResp://好友列表
                            SocialNode node = NodeManager.GetNode<SocialNode>();
                            if (node)
                                node.friendRankPanel.G2C_FriendRank(message.queryFriendResp.friendInfo);
                            else
                                InvateNode.G2C_OnLineFriend(message.queryFriendResp.friendInfo);
                            break;
                        case MessageId.G2C_QueryRenshiResp://认识的人
                            KnowPanelPanel.G2C_Know(message.queryRenshiResp.friendInfo);
                            break;
                        case MessageId.G2C_QueryNearbyResp://附近的人
                            NearPanel.G2C_Near(message.queryNearbyResp.friendInfo);
                            break;
                        case MessageId.G2C_SharePositionResp://位置分享
                            TipManager.Instance.OpenTip(TipType.SimpleTip, "分享位置信息成功!");
                            //请求附近的人
                            AddSendMessageQueue(new C2GMessage()
                             {
                                 msgid = MessageId.C2G_QueryNearbyReq
                             });
                            break;
                        case MessageId.G2C_ApplyFriendNotice://有人加我好友
                            NodeManager.OpenNode<AddFriendTipNode>(null, null, false, false).Inits(message.applyFriendNotice.friendInfo);
                            SocialNode socialsNode = NodeManager.GetNode<SocialNode>();
                            if (socialsNode)
                                socialsNode.messagePanel.G2C_AddFriends(new List<FriendInfo>() { message.applyFriendNotice.friendInfo });
                            break;
                        case MessageId.G2C_ApplyFriendResp://加好友结果
                            TipManager.Instance.OpenTip(TipType.SimpleTip, "发送成功,等待验证");
                            break;
                        case MessageId.G2C_ReplyFriendNotice:
                            if (message.replyFriendNotice.friendInfo.userId == UserInfoModel.userInfo.userId)
                                break;
                            string tipStr = "";
                            switch (message.replyFriendNotice.friendInfo.relation)
                            {
                                case -1:
                                    tipStr = message.replyFriendNotice.friendInfo.nickname + " 已拒绝您的好友申请";
                                    break;
                                case 1:
                                    FriendRankPanel.Inits();
                                    tipStr = message.replyFriendNotice.friendInfo.nickname + " 已同意您的好友申请";
                                    break;
                            }
                            TipManager.Instance.OpenTip(TipType.SimpleTip, tipStr);
                            RankNode ranknode = NodeManager.GetNode<RankNode>();
                            if (ranknode)
                                ranknode.playinfoPanel.QueryRelation(message.replyFriendNotice.friendInfo.userId);
                            break;
                        case MessageId.G2C_ReplyFriendResp:
                            FriendRankPanel.Inits();
                            break;
                        case MessageId.G2C_FindFriendResp://查找好友
                            FindFriendPanel.G2C_Find(message.findFriendResp);
                            break;
                        case MessageId.G2C_FriendChatResp://收到好友聊天
                            SocialModel.Instance.ReceiveMessage(message.friendChatResp);
                            SocialModel.Instance.isHaveNewMessage = true;
                            if (PageManager.Instance.CurrentPage is MainPage)
                                PageManager.Instance.GetPage<MainPage>().SetFriendRed();
                            else if (PageManager.Instance.CurrentPage is SelectRoomPage)
                                PageManager.Instance.GetPage<SelectRoomPage>().bottomPanel.SetFriendRedPoint();
                            break;
                        case MessageId.G2C_GiveSilver://赠送成功
                            TipManager.Instance.OpenTip(TipType.SimpleTip, "赠送成功!");
                            break;
                        case MessageId.G2C_QueryFriendOfflineChatResp:
                            SocialNode social_node = NodeManager.GetNode<SocialNode>();
                            if (social_node)
                            {
                                social_node.messagePanel.G2C_RecentContacts(message.queryFriendOfflineChatResp.friendOfflineChat);

                            }
                            break;
                        case MessageId.G2C_QueryRelationResp:
                            GameRoleInfoNode gameRole_node = NodeManager.GetNode<GameRoleInfoNode>();
                            if (gameRole_node)
                            {
                                gameRole_node.SetFriend(message.queryRelationResp.relation);
                            }
                            RankNode rankNode = NodeManager.GetNode<RankNode>();
                            if (rankNode)
                                rankNode.playinfoPanel.QueryRelationFinish(message.queryRelationResp.relation);
                            MatchReadyNode matchReadyNode = NodeManager.GetNode<MatchReadyNode>();
                            if (matchReadyNode)
                                matchReadyNode.chatPanel.CurItem.QueryRelationFinish(message.queryRelationResp.relation);
                            break;
                        case MessageId.G2C_QueryApplicantsResp:
                            SocialNode socialNode = NodeManager.GetNode<SocialNode>();
                            if (socialNode)
                            {
                                socialNode.messagePanel.G2C_AddFriends(message.queryApplicantsResp.friendInfo);
                            }
                            break;
                        case MessageId.G2C_RemoveFriendResp:
                            TipManager.Instance.OpenTip(TipType.SimpleTip, "删除成功!");
                            FriendRankPanel.Inits();
                            break;
                        #endregion

                        #region 意见反馈
                        case MessageId.G2C_QueryOpinionResp://我的历史反馈
                            FeedbackNode feed_node = NodeManager.GetNode<FeedbackNode>();
                            if (feed_node != null)
                            {
                                feed_node.G2C_LoadItems(message.queryOpinionResp.opinion);
                            }
                            break;
                        case MessageId.G2C_SubmitOpinionResp:
                            TipManager.Instance.OpenTip(TipType.SimpleTip, "提交成功");
                            FeedbackNode.G2C_Submit();
                            break;
                        #endregion

                        #region 任务
                        case MessageId.G2C_DaliyTask://每日任务列表
                            TaskNode.TaskRefresh(message.DailyTaskResp.dailyTask);
                            break;
                        case MessageId.G2C_CashingPrize://领奖
                            TaskNode.Reward(message.CashingPrizeResp.awardCount);
                            break;
                        case MessageId.G2C_Complete1Task://完成任务
                            TaskNode.ReqTasks();
                            break;
                        #endregion
                        #endregion
                        #region  比赛
                        case MessageId.G2C_LoadMatcherResp:
                            MatchModel.Instance.LoadMatcherFinish(message.loadMatcherResp);
                            break;
                        case MessageId.G2C_JoinMatcherResp:
                            MatchApplyNode.JoinMatcherFinish(message.JoinMatcherResp);
                            break;
                        case MessageId.G2C_PrepareMatcherResp:
                            MatchModel.Instance.PrepareMatcherFinish(message.prepareMatcherResp);
                            break;
                        case MessageId.G2C_QuitMatcherResp:
                            var readyNode = NodeManager.GetNode<MatchReadyNode>();
                            if (readyNode)
                                readyNode.QuitMatcher();
                            var waitNode = NodeManager.GetNode<MatchWaitNode>();
                            if (waitNode)
                                waitNode.QuitMatcher();
                            break;
                        case MessageId.G2C_MatcherRiseSuccess:
                            var waitNode2 = NodeManager.GetNode<MatchWaitNode>();
                            if (waitNode2)
                                waitNode2.MatcherRise(message.waitMatcherRiseResp);
                            break;
                        case MessageId.G2C_StartMatcher:
                            MatchReadyNode.StartMatch();
                            break;
                        case MessageId.G2C_MatcherCountResp:
                            MatchModel.Instance.MatcherCountFinish(message.matcherCountResp);
                            break;
                        case MessageId.G2C_UserMatcherRewardResp:
                            MatchModel.Instance.UserMatcherRewardFinish(message.userMatcherRewardResp);
                            break;
                        case MessageId.G2C_MatcherHistoryResp:
                            MatchModel.Instance.MatcherHistoryFinish(message.matcherHistoryResp);
                            break;
                        case MessageId.G2C_UserMatcherFriendRankResp:
                            MatchModel.Instance.MatcherFriendRankFinish(message.userMatcherFriendRankResp);
                            break;
                        case MessageId.G2C_MatcherChatResp:
                            MatchReadyNode.MatcherChat(message.matcherChatResp);
                            break;
                        case MessageId.G2C_MatcherJoinCount:
                            MatchReadyNode.JoinCountFlush(message.matcherJoinCount);
                            break;
                        case MessageId.G2C_WaitMatcherRiseResp:
                            MatchWaitNode.OpenMatchWait(message.waitMatcherRiseResp);
                            break;
                        case MessageId.G2C_MatcherPlayerRankingResp://比赛排行
                            MatchRankRewardNode.QueryRankFinish(message.matcherPlayerRankingResp);
                            MatchWaitRankRewardPanel.QueryRankFinish(message.matcherPlayerRankingResp);
                            break;
                        case MessageId.G2C_DieMatcherPlayerResp:
                            NodeManager.OpenNode<MatchResultNode>("match", null, true, false).DieMatcherFinish(message.dieMatcherPlayerResp);
                            break;
                        case MessageId.G2C_MedalResp:
                            NodeManager.OpenNode<MatchResultNode>("match", null, true, false).MedalFinish(message.medalResp);
                            break;
                        case MessageId.G2C_CurrentMatcherInfoResp://收到比赛消息
                            MatchRulesNode.DisplayMatchInfo(message.currentMatcherInfoResp);
                            //LandlordsNet.BisaiInfoResp(message.currentMatcherInfoResp);
                            break;
                        case MessageId.G2C_MyMatcherRankingResp://收到我的排行信息
                            if (PageManager.Instance.GetPage<LandlordsPage>())
                                LandlordsNet.BisaiMyRankInfoResp(message.myMatcherRankingResp);
                            else if (PageManager.Instance.GetPage<MaJangPage>())
                                MaJangPage.Instance.RefreshMatchInfo(message.myMatcherRankingResp);
                            break;
                        case MessageId.G2C_MatcherFriendResp:
                            MatchReadyNode.InviteFriend(message.matcherFriendResp);
                            break;
                        case MessageId.G2C_InviteFriendMatcherResp:
                            var inviteResp = message.inviteFriendMatcherResp;
                            if (inviteResp.code == 1)
                                TipManager.Instance.OpenTip(TipType.SimpleTip, "已发送邀请");
                            else if (inviteResp.code == 2)
                            {
                                TipManager.Instance.OpenTip(
                                    TipType.ChooseTip,
                                    "您的好友 " + inviteResp.hostName + " 邀请您参加" + inviteResp.matcherName,
                                    10f,
                                    delegate
                                    {
                                        MatchModel.Instance.JoinMatch((int)inviteResp.cost, inviteResp.costType, inviteResp.matcherId, inviteResp.matcherName);
                                        MatcherInfo cur = new MatcherInfo() { matchId = inviteResp.matcherId };
                                        MatchModel.Instance.CurData = cur;
                                    }
                                    );
                            }
                            break;
                        case MessageId.G2C_InviteFriendYuePaiResp:
                            if (message.inviteFriendYuePaiResp.code == 3)
                            {
                                TipManager.Instance.OpenTip(TipType.SimpleTip, "对方已拒绝您的邀请");
                            }
                            break;
                        case MessageId.G2C_MatcherTickUser:
                            if (NodeManager.GetNode<MatchReadyNode>())
                            {
                                NodeManager.CloseTargetNode<MatchReadyNode>();
                                MatchModel.Instance.CurData = null;
                                TipManager.Instance.OpenTip(TipType.SimpleTip, "人数不符合比赛要求，请重新报名");
                            }
                            break;
                        #endregion
                        #region ...麻将相关
                        case MessageId.G2C_CreateTableResp:
                            PageManager.Instance.OpenPage<MaJangPage>();
                            break;
                        case MessageId.G2C_GameJoinTableResp:
                        case MessageId.G2C_MjChangeTableResp:
                            if (!PageManager.Instance.GetPage<MaJangPage>())
                                PageManager.Instance.OpenPage<MaJangPage>();
                            MaJangPage.Instance.JoinRoomResult(message.GameJoinTableResp.user, message.GameJoinTableResp.detail, message.GameJoinTableResp.isChangeTable);
                            break;
                        case MessageId.G2C_GameJoinTableNotice:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.OtherJoinRoomResult(message.GameJoinTableNotice.user);
                            break;
                        case MessageId.G2C_GameLeaveTableResp:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.LeaveByPos(message.GameLeaveTableResp.pos, false);
                            break;
                        case MessageId.G2C_GamePrepareNotice:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.Ready(message.GamePrepareNotice.pos, message.GamePrepareNotice.prepared);
                            break;
                        case MessageId.G2C_GameStartResp:
                            if (message.GameStartResp.isReconnect)
                                PageManager.Instance.OpenPage<MaJangPage>();
                            MaJangPage.Instance.StartOrReConnectGame(message.GameStartResp);
                            if (message.GameStartResp.isReconnect && message.GameStartResp.state != 4)
                                canHandleMessage = false;//重连完成前，禁止处理所有消息
                            break;
                        case MessageId.G2C_UserStepOverNotice:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.StartOutMj(message.UserStepOverNotice);
                            break;
                        case MessageId.G2C_UserGetMjResp:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.GetUserGetMj(message.UserGetMjResp);
                            break;
                        case MessageId.G2C_UserActionNotice:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.GetUserAction(message.UserActionNotice);
                            break;
                        case MessageId.G2C_UserMjActionResp:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.currentPlayer.ExecuteAction(message.UserActionResp);
                            break;
                        case MessageId.G2C_ChangeBaoPaiNotice:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.OpenBao(message.changeBaoPaiNotice);
                            break;
                        case MessageId.G2C_MjResult:
                            if (MaJangPage.Instance)
                                if (message.MjResult.isFinal)
                                {
                                    MaJangPage.Instance.GameResult(message.MjResult);
                                    canHandleMessage = false;//弹出当前局结算后，禁止处理所有消息
                                }
                                else
                                    MaJangPage.Instance.Xjxd(message.MjResult);
                            break;
                        case MessageId.G2C_MjChangePosResp:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.ChangeSeatResult(message.mjChangePosResp);
                            break;
                        case MessageId.G2C_MjChangePosApplyResp:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.ChangeSeatReply(message.mjChangePosApplyResp);
                            break;
                        case MessageId.G2C_MjChangePosNotice:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.OtherPlayerChangeSeat(message.mjChangePosNotice.pos1, message.mjChangePosNotice.pos2);
                            break;
                        case MessageId.G2C_MjTuoguanResp:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.TrusteeshipResult(true);
                            break;
                        case MessageId.G2C_MjCancelTuoguanResp:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.TrusteeshipResult(false);
                            break;
                        case MessageId.G2C_MjTuoguanNotice:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.TrusteeshipResult(true, message.mjTuoguanNotice.pos);
                            break;
                        case MessageId.G2C_MjCancelTuoguanNotice:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.TrusteeshipResult(false, message.mjCancelTuoguanNotice.pos);
                            break;
                        case MessageId.G2C_UserStateChangeNotice:
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.SetPlayerStatu(message.UserStateChangeNotice);
                            break;
                        case MessageId.G2C_DisbandChooiseNotice:
                            JiesanNode.G2C_Vote(message.DisbandChooiseNotice.pos, message.DisbandChooiseNotice.agree);
                            break;
                        case MessageId.G2C_DisbandChooiseResult:
                            if (message.DisbandChooiseResult.disband)
                            {
                                if (message.DisbandChooiseResult.YuePaiTable != null)
                                    NodeManager.OpenNode<CardResultShowNode>().Inits(message.DisbandChooiseResult.YuePaiTable);
                                else
                                {
                                    PageManager.Instance.OpenPage<MainPage>();
                                    TipManager.Instance.OpenTip(TipType.SimpleTip, "房间解散成功！");
                                }
                            }
                            else
                                TipManager.Instance.OpenTip(TipType.SimpleTip, "房间解散失败，有人拒绝解散");
                            break;
                        case MessageId.G2C_DisbandNotice:
                            NodeManager.OpenNode<JiesanNode>().Inits(message.DisbandNotice.applyPos);
                            break;
                        case MessageId.G2C_DisbandResp:
                            TipManager.Instance.OpenTip(TipType.SimpleTip, "解散申请已发出");
                            break;
                        case MessageId.G2C_UserTalkResp://麻将聊天
                            if (MaJangPage.Instance)
                                MaJangPage.Instance.PlayerChat(message.UserTalkResp);
                            break;
                        case MessageId.G2C_MjKickPlayerRNotice://麻将踢人
                            MaJangPage.Instance.LeaveById(message.mjKickedPlayerNotice.userId, true);
                            break;
                        case MessageId.G2C_MjKickPlayerResp:
                            PageManager.Instance.OpenPage<MainPage>();
                            TipManager.Instance.OpenTip(TipType.SimpleTip, "您被踢出房间");
                            break;
                        #endregion
                        #region 斗地主
                        case MessageId.G2C_EnterDdzRoomResp://进房间
                            LandlordsNet.G2C_EnterRoom(message.enterDdzRoomResp);
                            break;
                        case MessageId.G2C_DdzPrepareResp:
                            LandlordsNet.G2C_ZhunbeiResp(message.ddzPrepareResp);
                            break;
                        case MessageId.G2C_PlayerRemovedNotice://有玩家强退房间推送
                            for (int i = 0; i < message.playerRemovedNotice.userId.Count; i++)
                            {
                                LandlordsNet.G2C_LeaveRoomResp(message.playerRemovedNotice.userId[i].ToString(), message.playerRemovedNotice.reason);
                            }
                            break;
                        case MessageId.G2C_DdzLeaveRoomResp://有玩家退出房间推送
                            LandlordsNet.G2C_LeaveRoomResp(message.ddzLeaveRoomResp.userId.ToString(), 0);
                            break;
                        case MessageId.G2C_DdzKickedPlayerNotice://我被踢
                            LandlordsNet.MeKicked();
                            break;
                        case MessageId.G2C_DdzQueryPokerResp://记牌器
                            LandlordsPage.Instance.componentView.jipaiqiPanel.InitValue(message.ddzQueryPokerResp.ddzPokerCounter);
                            break;
                        case MessageId.G2C_DdzFaPaiResp://发牌推送
                            LandlordsNet.G2C_DealCard(message.ddzFaPaiResp);
                            break;
                        case MessageId.G2C_DdzJiaoFenNotice://该我叫分推送
                            LandlordsNet.G2C_MeCallFen();
                            break;
                        case MessageId.G2C_DdzQdzNotice://该我叫地主
                            LandlordsNet.G2C_MeCallLandlords();
                            break;
                        case MessageId.G2C_DdzChuPaiNotice://该我出牌推送
                            LandlordsNet.G2C_MePop();
                            break;
                        case MessageId.G2C_DdzJiaoFenResp://叫分推送
                            LandlordsNet.G2C_PlayerCallResp(message.ddzJiaoFenResp);
                            break;
                        case MessageId.G2C_DdzQdzResp://叫分推送
                            LandlordsNet.G2C_PlayerQdzResp(message.ddzQdzResp);
                            break;
                        case MessageId.G2C_DdzChuPaiResp://出牌推送
                            LandlordsNet.G2C_PopResp(message.ddzChuPaiResp);
                            break;
                        case MessageId.G2C_DdzTuoguanResp://托管推送
                            LandlordsNet.G2C_TuoguanResp(message.ddzTuoguanResp);
                            break;
                        case MessageId.G2C_DdzJieSuanResp://结算单局推送
                            LandlordsNet.G2C_Result(message.ddzJieSuanResp);
                            break;
                        case MessageId.G2C_DdzYuePaiResult://房卡房总结算
                            //NodeManager.OpenNode<CardResultShowNode>().Inits(message.yuePaiTableResp);
                            break;
                        case MessageId.G2C_DdzReconnectResp://断线重连回应
                            if (message.ddzReconnectResp.tnf == 1)
                            {
                                TipManager.Instance.OpenTip(TipType.SimpleTip, "重连斗地主失败,进入大厅");
                                UserInfoModel.userInfo.inDzz = false;
                                break;
                            }
                            StartCoroutine(LandlordsNet.G2C_Reconnect(message.ddzReconnectResp));
                            break;
                        case MessageId.G2C_RemoveCRTable:
                            PageManager.Instance.OpenPage<MainPage>();
                            break;
                        case MessageId.G2C_DdzCanLeaveResp://是否可以离开房间响应
                            LandlordsNet.G2C_IsCanLeaveResp(message.ddzCanLeaveResp);
                            break;
                        case MessageId.G2C_DdzJieSanNotice://收到有人发起解散
                            if (message.ddzJieSanNotice.userId != UserInfoModel.userInfo.userId)
                                NodeManager.OpenNode<JiesanNode>().Inits(message.ddzJieSanNotice.userId);
                            break;
                        case MessageId.G2C_DdzVoteNotice://收到有人投票
                            JiesanNode.G2C_Vote(message.ddzVoteNotice.userId, message.ddzVoteNotice.yn);
                            break;
                        case MessageId.G2C_DdzJieSanResp://推送解散结果
                            LandlordsNet.G2C_JiesanResult(message.ddzJieSanResp.success);
                            break;
                        case MessageId.G2C_DdzReplaceTableResp:
                            if (message.ddzReplaceTableResp.userId == UserInfoModel.userInfo.userId)
                            {
                                LandlordsModel.Instance.Clear();
                                LandlordsPage.Instance.InitRoom();
                            }
                            else
                            {
                                LandlordsNet.G2C_LeaveRoomResp(message.ddzReplaceTableResp.userId.ToString(), 0);
                            }
                            break;
                        //case MessageId.inva
                        //TipManager.Instance.OpenTip(TipType.ChooseTip, string.Format("您的好友{0}邀请您进入{1}一起玩耍", name, game), 0, () =>
                        //    {
                        //        NodeManager.OpenNode<TipsEnterNode>().Inits();
                        //    });
                        //break;
                        case MessageId.G2C_DdzChatResp://聊天
                            LandlordsNet.G2C_ChatResp(message.ddzChatResp);
                            break;
                        case MessageId.G2C_QueryPlayerBaseInfoResp://请求玩家信息（弹框）
                            GameRoleInfoNode.SetPlayerInfo(message.playerBaseInfo);
                            break;
                        #endregion
                        #region 喇叭
                        case MessageId.G2C_BroadCastMsg://有人发喇叭
                            GameMessage msg = new GameMessage()
                            {
                                type = message.broadCastMsg.horn.type,
                                sender = message.broadCastMsg.horn.nickname,
                                value = message.broadCastMsg.horn.content
                            };
                            ChatModel.chatList.Add(msg);
                            if (ChatModel.chatList.Count > ChatModel.MaxChatCount)
                            {
                                ChatModel.chatList[0] = msg;
                                ChatModel.chatList.RemoveAt(ChatModel.chatList.Count - 1);
                            }

                            TrumpetNode trump_node = NodeManager.GetNode<TrumpetNode>();
                            if (trump_node)
                                trump_node.LoadItem(msg);
                            NoticeNode.Add(string.Format("{0}:{1}", msg.sender, msg.value));
                            if (message.broadCastMsg.horn.nickname == UserInfoModel.userInfo.nickName)
                                TipManager.Instance.OpenTip(TipType.SimpleTip, "喇叭发送成功!");
                            break;
                        #endregion
                        //case MessageId.G2C_InviteFriendYuePaiResp:
                        //  TipManager.Instance.OpenTip(TipType.ChooseTip,"您的好友邀请您进入"+message.inviteFriendYuePaiResp.)
                        //break;
                        default:
                            if (messageId != MessageId.G2C_HeartBeatResp)
                                UIUtils.Log("未处理的消息id :" + messageId);
                            break;
                    }
                    #endregion
                }
                else
                    ErrorMessage(message);
            }
            while (!canHandleMessage)
                yield return new WaitForSecondsRealtime(0.1f);
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }

    /// <summary>
    /// 接收数据
    /// </summary>
    void ReceiveMessage()
    {
        while (true)
        {
            if (!mSocket.Connected && !IsConnected())
                break;
            byte[] recvBytesHead = GetBytesReceive(2);
            int bodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recvBytesHead, 0));
            byte[] nullByte = GetBytesReceive(1);
            //TODO 对于预留字节的处理
            byte[] recvBytesBody = GetBytesReceive(bodyLength);
            allPackages.Enqueue(ProtobufSerilizer.DeSerialize<G2CMessage>(recvBytesBody));
        }
    }

    /// <summary>
    /// 接收数据并处理
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    byte[] GetBytesReceive(int length)
    {
        byte[] recvBytes = new byte[length];
        while (length > 0)
        {
            byte[] receiveBytes = new byte[length < packageMaxLength ? length : packageMaxLength];
            int iBytesBody = 0;
            if (length >= receiveBytes.Length)
                iBytesBody = mSocket.Receive(receiveBytes, receiveBytes.Length, 0);
            else
                iBytesBody = mSocket.Receive(receiveBytes, length, 0);
            receiveBytes.CopyTo(recvBytes, recvBytes.Length - length);
            length -= iBytesBody;
        }
        return recvBytes;
    }
    #endregion

    /// <summary>
    /// 构建消息数据包
    /// </summary>
    /// <param name="protobufModel"></param>
    /// <param name="messageId"></param>
    byte[] BuildPackage(C2GMessage c2g)
    {
        byte[] b = ProtobufSerilizer.Serialize(c2g);
        ByteBuffer buf = ByteBuffer.Allocate(b.Length + 3);
        buf.WriteShort((short)(b.Length));
        //TODO 对预留字节的处理
        buf.WriteByte(new byte());
        buf.WriteBytes(b);
        return buf.GetBytes();
    }

    /// <summary>
    /// 获取连接状态
    /// </summary>
    /// <returns></returns>
    bool IsConnected()
    {
        bool ConnectState = true;
        bool state = mSocket.Blocking;
        try
        {
            ByteBuffer buf = ByteBuffer.Allocate(4 + 4);
            buf.WriteInt(4);
            buf.WriteInt((int)MessageId.C2G_HeartBeat);

            mSocket.Blocking = false;
            int length = mSocket.Send(buf.GetBytes(), 0, SocketFlags.None);
            ConnectState = length != 0;
        }
        catch (SocketException e)
        {
            ConnectState = e.NativeErrorCode.Equals(10035);
        }
        finally
        {
            mSocket.Blocking = state;
        }
        return ConnectState;
    }

    /// <summary>
    /// 断线重连
    /// </summary>
    IEnumerator Reconnect(bool isReLogin = true)
    {
        lastHeartTime = Time.realtimeSinceStartup;
        //TODO 显示重连状态
        LoadingNode.OpenLoadingNode(LoadingType.Common);
        Close();
        reconnectTime++;
        bool isSuccess = false;
        isSuccess = Init();
        if (!isSuccess)
        {
            if (reconnectTime < 3)
            {
                yield return new WaitForSecondsRealtime(5);
                StartCoroutine(Reconnect());
            }
            else
                OpenTipNetError();
        }
        else
        {
            reconnectTime = 0;
            //重新登录
            if (isReLogin)
            {
                AddSendMessageQueue(new C2GMessage()
                {
                    msgid = MessageId.C2G_Login,
                    login = new Login() { token = LoginPage.token }
                }, true);
                sendList.Add(sendList[0]);
                if (sendList.Count > 1)
                    sendList.RemoveAt(0);
            }
            LoadingNode.instance.Close();
        }
    }

    void OnDestroy()
    {
        Close();
        sendList.Clear();
        allPackages.Clear();
    }

    /// <summary>
    /// 关闭socket,终止线程
    /// </summary>
    public void Close()
    {
        if (mSocket != null)
        {
            if (mSocket.Connected)
                mSocket.Shutdown(SocketShutdown.Both);
            mSocket.Close();
            mSocket = null;
        }
        if (threadSend != null)
            threadSend.Abort();
        if (threadRecive != null)
            threadRecive.Abort();
        threadSend = null;
        threadRecive = null;
    }

    /// <summary>
    /// 打开网络错误弹窗
    /// </summary>
    void OpenTipNetError()
    {
        TipManager.Instance.OpenTip(TipType.AlertTip, "网络错误，请重新登录", 0, () =>
        {
            Close();
            LoginPage.DeleteLoginInfo();
            PageManager.Instance.OpenPage<LoginPage>();
            Destroy(gameObject);
        });
    }

    /// <summary>
    /// 错误消息处理
    /// </summary>
    /// <param name="message">错误消息</param>
    void ErrorMessage(G2CMessage message)
    {
        if (!string.IsNullOrEmpty(message.warning))
            TipManager.Instance.OpenTip(TipType.AlertTip, message.warning);
        print("错误的result:" + message.result + "--id:" + message.msgid);
        switch (message.msgid)
        {
            case MessageId.G2C_QueryTableInfoResp:
                JoinGameRoonNode.G2C_RoomNumberError();
                break;
            case MessageId .G2C_UserMjActionResp:
                MaJangPage.Instance.RefreshTimer();
                break;
            case MessageId.G2C_Kick_ReLogin:
                LoginPage.TakeLogin();
                break;
            case MessageId.G2C_Kick:
                TipManager.Instance.OpenTip(TipType.ChooseTip, "您被踢出登录是否重登?", 0, () =>
                {
                    PageManager.Instance.OpenPage<LoginPage>();
                });
                break;
        }
    }


}
