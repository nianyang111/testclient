using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInvitePanel : MonoBehaviour
{
    public GameObject invitePanel, inviteBtn;
    public GameObject weFriendBtn, gameFriendBtn;
    public GameObject friendPanel;
    public GameObject notHaveFriendPanel;
    public GameObject frinedTrueBtn;
    public MatchReadyInviteItem prefab;
    public Transform content;
    List<MatchReadyInviteItem> ItemList = new List<MatchReadyInviteItem>();
    private MatchReadyInviteItem curItem;
    public MatchReadyInviteItem CurItem
    {
        get { return curItem; }
        set
        {
            if (curItem != null)
                curItem.OnSelect();
            if (curItem == value)
            {
                curItem.OnSelect();
                curItem = null;
            }
            else
            {
                curItem = value;
                curItem.Select();
            }
        }
    }
    public void Init()
    {
        UGUIEventListener.Get(inviteBtn).onClick = delegate { invitePanel.SetActive(!invitePanel.activeInHierarchy); friendPanel.SetActive(false); };
        UGUIEventListener.Get(weFriendBtn).onClick = delegate { WeFriend(); };
        UGUIEventListener.Get(gameFriendBtn).onClick = delegate { GameFriend(); };
        UGUIEventListener.Get(frinedTrueBtn).onClick = delegate { FriendTrue(); };
        friendPanel.SetActive(false);
    }
    private void FriendTrue()
    {
        if (CurItem != null)
        {
            SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
            {
                inviteFriendMatcherReq = new InviteFriendMatcherReq()
                {
                    costType=MatchModel.Instance.CurData.costType,
                    cost= (int)MatchModel.Instance.CurData.cost,
                    matcherId=MatchModel.Instance.CurData.matchId,
                    matcherName=MatchModel.Instance.CurData.name,
                    userId=CurItem.userId
                },
                msgid= MessageId.C2G_InviteFriendMatcherReq
            });
        }
        for (int i = 0; i < ItemList.Count; i++)
        {
            Destroy(ItemList[i].gameObject);
        }
        ItemList.Clear();
        friendPanel.SetActive(false);
        
    }
    /// <summary>
    /// 微信分享
    /// </summary>
    private void WeFriend()
    {
        var icon= BundleManager.Instance.GetSprite("task/meirirenwu_pic_1");
        byte[] thumb = MiscUtils.SizeTextureBilinear(icon.texture, Vector2.one * 150).EncodeToJPG();
        SDKManager.Instance.ShareWebPage(SDKManager.WechatShareScene.WXSceneSession,
            UserInfoModel.userInfo.downUrl,
            "雪瑶明水棋牌",
            "来参加雪瑶明水棋牌" + MatchModel.Instance.CurData.name+"一起赢大奖！",
            thumb);
    }
    /// <summary>
    /// 游戏好友
    /// </summary>
    private void GameFriend()
    {
        friendPanel.SetActive(true);
        invitePanel.SetActive(false);
        notHaveFriendPanel.SetActive(true);
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = MessageId.C2G_MatcherFriendReq,
        });
    }
    public void CreateItem(net_protocol.MatcherFriendResp resp)
    {
        var dataList = resp.inviteUser;
        notHaveFriendPanel.SetActive(dataList.Count <1);
        for (int i = 0; i < dataList.Count; i++)
        {
            var item = Instantiate(prefab, content);
            item.panel = this;
            item.Init(dataList[i]);
            ItemList.Add(item);
        }
    }
    public void Close()
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            Destroy(ItemList[i].gameObject);
        }
        ItemList.Clear();
    }
}
