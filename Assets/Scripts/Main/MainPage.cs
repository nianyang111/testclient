using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainPage : Page
{
    [HideInInspector]
    public GameObject btnCustomService, btnRankList, btnSet, btnMore;
    [HideInInspector]
    public GameObject btnMaJang, btnFightLandlord, btnMatch, btnAppoint;
    [HideInInspector]
    public GameObject btnFriend, btnMail, btnInfo, btnAddGoldBar, btnAddRoomCard, btnSilverCoin;
    [HideInInspector]
    public GameObject btnSafeBox, btnBag, btnAppoint2, btnTask, btnStore, btnActivity, btnMatch2;
    [HideInInspector]
    public Image headIcon, vip;
    [HideInInspector]
    public Text nickName, goldBarText, roomCardText, silverCoinText;
    [HideInInspector]
    public Text ddzOnlineNum, mjOnlineNum;
    //[HideInInspector]
    public GameObject friendRed, msgRed, taskRed;
    public override void Init()
    {
        base.Init();
        PageManager.Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        InitButton();
        NodeManager.OpenNode<NoticeNode>(null, null, false, false);
        InitUserInfo();
        NodeManager.OpenNode<NoticeNode>(null, null, false, false);
    }


    /// <summary>
    /// 重连斗地主
    /// </summary>
    void ReConnectLandlords()
    {
        if (UserInfoModel.userInfo.inDzz)
        {
            string pageName = typeof(LandlordsPage).ToString().ToLower();
            CallBack call = () =>
            {
                LandlordsNet.C2G_ReqConnect();
                TipManager.Instance.OpenTip(TipType.SimpleTip, "正在重连中......", 5);
                //NodeManager.OpenNode<LoadingGameNode>("Hall").Inits(GameType.LandlordsController);
            };

            //if (!GameDownModel.GetGameIsDownState(pageName))
            //{
            //    GameDownModel.Down(pageName, (progress) =>
            //    {
            //        LoadingNode.OpenLoadingNode(LoadingType.Progress, "正在下载斗地主资源", progress);
            //    }, call);
            //}
            //else
            call();
        }
    }

    public void InitUserInfo()
    {
        nickName.text = UserInfoModel.userInfo.nickName;
        goldBarText.text = UserInfoModel.userInfo.walletGoldBarNum.ToString();
        roomCardText.text = UserInfoModel.userInfo.roomCardNum.ToString();
        silverCoinText.text = UserInfoModel.userInfo.walletAgNum.ToString();
        vip.gameObject.SetActive(UserInfoModel.userInfo.vipCard > 0);
        if (UserInfoModel.userInfo.headIconSprite == null)
        {
            StartCoroutine(MiscUtils.DownloadImage(UserInfoModel.userInfo.headIcon, spr =>
                 {
                     UserInfoModel.userInfo.headIconSprite = spr;
                     headIcon.sprite = UserInfoModel.userInfo.headIconSprite;
                 }));
        }
        else
            headIcon.sprite = UserInfoModel.userInfo.headIconSprite;
    }

    void InitButton()
    {
        UGUIEventListener.Get(btnCustomService).onClick = delegate { OpenTargetNode<FeedbackNode>(); };
        UGUIEventListener.Get(btnRankList).onClick = delegate { OpenTargetNode<RankNode>(); };
        UGUIEventListener.Get(btnSet).onClick = delegate { OpenTargetNode<SetNode>(); };
        UGUIEventListener.Get(btnMore).onClick = delegate { };

        UGUIEventListener.Get(btnMaJang).onClick = delegate { PageManager.Instance.OpenPage<SelectRoomPage>(() => PageManager.Instance.GetPage<SelectRoomPage>().OpenPanel(2)); };
        UGUIEventListener.Get(btnFightLandlord).onClick = delegate { PageManager.Instance.OpenPage<SelectRoomPage>(() => PageManager.Instance.GetPage<SelectRoomPage>().OpenPanel(1)); };
        UGUIEventListener.Get(btnMatch).onClick = delegate { OpenTargetPage<MatchPage>(); };
        UGUIEventListener.Get(btnAppoint).onClick = delegate { OpenTargetNode<JoinGameRoonNode>(); };

        UGUIEventListener.Get(btnFriend).onClick = delegate { OpenTargetNode<SocialNode>(); };
        UGUIEventListener.Get(btnMail).onClick = delegate { OpenTargetNode<MessageNode>(); };
        UGUIEventListener.Get(btnInfo).onClick = delegate { OpenTargetNode<UserInfoNode>(); };
        UGUIEventListener.Get(btnAddGoldBar).onClick = delegate { NodeManager.OpenNode<StoreNode>(null, () => { NodeManager.GetNode<StoreNode>().goldBtn.isOn = true; }); };
        UGUIEventListener.Get(btnAddRoomCard).onClick = delegate { NodeManager.OpenNode<StoreNode>(null, () => { NodeManager.GetNode<StoreNode>().cardBtn.isOn = true; }); };
        UGUIEventListener.Get(btnSilverCoin).onClick = delegate { NodeManager.OpenNode<StoreNode>(null, () => { NodeManager.GetNode<StoreNode>().agBtn.isOn = true; }); };

        UGUIEventListener.Get(btnSafeBox).onClick = delegate { OpenTargetNode<SafeBoxNode>(); };
        UGUIEventListener.Get(btnBag).onClick = delegate { OpenTargetNode<BagNode>(); };
        UGUIEventListener.Get(btnAppoint2).onClick = delegate { OpenTargetNode<JoinGameRoonNode>(); };
        UGUIEventListener.Get(btnTask).onClick = delegate { OpenTargetNode<TaskNode>(); };
        UGUIEventListener.Get(btnStore).onClick = delegate { OpenTargetNode<StoreNode>(); };
        //UGUIEventListener.Get(btnActivity).onClick = delegate { OpenTargetNode<UserInfoNode>(); };
        UGUIEventListener.Get(btnMatch2).onClick = delegate { OpenTargetPage<MatchPage>(); };
    }
    public override void Open()
    {
        base.Open();
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_QueryAmountOfPlayerInGameReq
        });
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_QueryUserMsg,
        });
        MessageModel.Instance.msgAct = SetMsgRed;
        ReConnectLandlords();
        SetFriendRed();
        TaskNode.ReqTasks();
        SetNode.FloatBall();
    }
    public void SetOnlineNum(net_protocol.QueryAmountOfPlayerInGameResp resp)
    {
        for (int i = 0; i < resp.playerInGameCounter.Count; i++)
        {
            if ("ddz" == resp.playerInGameCounter[i].gameName)
            {
                ddzOnlineNum.text = resp.playerInGameCounter[i].amount.ToString();
            }
            if ("mj" == resp.playerInGameCounter[i].gameName)
            {
                mjOnlineNum.text = resp.playerInGameCounter[i].amount.ToString();
            }
        }
    }

    private void SetMsgRed(bool isShow)
    {
        msgRed.SetActive(isShow);
    }
    public void SetFriendRed()
    {
        friendRed.SetActive(SocialModel.Instance.isHaveNewMessage);
    }
    public void SetTaskRed(bool isShow)
    {        
        taskRed.SetActive(isShow);
    }

    void OpenTargetNode<T>() where T : Node
    {
        NodeManager.OpenNode<T>();
    }

    void OpenTargetPage<T>() where T : Page
    {
        PageManager.Instance.OpenPage<T>();
    }
    public override void Close()
    {
        base.Close();
        MessageModel.Instance.msgAct = null;
    }
}
