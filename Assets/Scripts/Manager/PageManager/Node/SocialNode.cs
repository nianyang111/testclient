using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UiEffect;
using UnityEngine;
using UnityEngine.UI;

public class SocialNode : Node {

    public SocialSmallTab smallTab;
    public Tab friendTab;
    public Tab msgTab;

    public GameObject friendObj;
    public GameObject messageObj;
    public MessagePanel messagePanel;

    public FriendRankPanel friendRankPanel;
    public KnowPanelPanel knownPanel;
    public NearPanel nearPanel;
    public FindFriendPanel findPanel;

    GameObject curSmallPanel;

    public GameObject findFriendBtn;
    public RoleInfoView roleInfoView;
    public GiveView giveView;   


    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(findFriendBtn).onClick=delegate
        {
            findPanel.SetVisbel(true);
        };

        smallTab.InitData(ShowSmallTab, 0);
    }   

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">0好友排行/1认识的人/2附近的人</param>
    void ShowSmallTab(int type)
    {
        if (curSmallPanel != null)
            curSmallPanel.SetActive(false);
        switch (type)
        {
            case 0:
                curSmallPanel = friendRankPanel.gameObject;                
                break;
            case 1:
                curSmallPanel = knownPanel.gameObject;
                break;
            case 2:
                curSmallPanel = nearPanel.gameObject;
                break;
            default:
                break;
        }
        curSmallPanel.SetActive(true);
    }

    /// <summary>
    /// 头像按钮回调
    /// </summary>
    /// <param name="info"></param>
    public static void HeadCall(FriendInfo info,bool isFriend)
    {
        SocialNode node = NodeManager.GetNode<SocialNode>();
        if (node)
        {
            node.roleInfoView.SetVisibel(true);
            BasePlayerInfo playerInfo = new BasePlayerInfo();
            playerInfo.exp = info.exp;
            playerInfo.icon = info.photo;
            playerInfo.lv = info.level;
            playerInfo.money = info.sliver;
            playerInfo.six = info.gender == 0 ? Six.boy : Six.girl;
            playerInfo.uid = info.userId.ToString();
            playerInfo.userNickname = info.nickname;
            playerInfo.relation = info.relation;
            node.roleInfoView.Init(playerInfo, isFriend);
            node.roleInfoView.AddFriendCall = (relation) => info.relation = relation;
        }
    }

    /// <summary>
    /// 赠送按钮回调
    /// </summary>
    public static void GiveCall(FriendInfo info)
    {
        SocialNode node = NodeManager.GetNode<SocialNode>();
        if (node)
        {
            node.giveView.SetVisibel(true);
            node.giveView.Init(info);
        }
    }

    /// <summary>
    /// 聊天按钮回调
    /// </summary>
    public static void ChatCall(FriendInfo info)
    {
        SocialNode node = NodeManager.GetNode<SocialNode>();
        if (node)
        {
            node.msgTab.GetComponent<UGUIEventListener>().onClick(null);
            node.messagePanel.OnClickCall(info);
        }
    }

    public override void Close(bool isOpenLast = true)
    {
        base.Close(isOpenLast);
        SocialModel.Instance.isHaveNewMessage = false;
        if (PageManager.Instance.CurrentPage is MainPage)
            PageManager.Instance.GetPage<MainPage>().SetFriendRed();
        else if (PageManager.Instance.CurrentPage is SelectRoomPage)
            PageManager.Instance.GetPage<SelectRoomPage>().bottomPanel.SetFriendRedPoint();
    }
}
