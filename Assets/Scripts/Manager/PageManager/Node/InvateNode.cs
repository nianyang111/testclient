using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
public class InvateNode : Node
{
    public Text roomIdLb;

    /// <summary>
    /// 微信QQpanel
    /// </summary>
    public GameObject qqBtn;
    public GameObject wxBtn;

    /// <summary>
    /// 扫码panel
    /// </summary>
    public Image erweimaIcon;

    /// <summary>
    /// 在线好友panel
    /// </summary>
    public Transform parent;
    public GameObject prefab;
    public GameObject invateBtn;
    List<InvateItem> friends = new List<InvateItem>();


    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(qqBtn).onClick = delegate { GoQQ(); };
        UGUIEventListener.Get(wxBtn).onClick = delegate { GoVx(); };
        UGUIEventListener.Get(invateBtn).onClick = delegate { Inivate(); };         
        C2G_ReqFriendList();
    }
    string roomId;
    public void Inits(string roomId)
    {
        this.roomId = roomId;
        roomIdLb.text = "邀请(房号： " + roomId + ")";
        InitErweima();
    }

    #region 微信QQpanel
    void GoQQ()
    {

    }
    void GoVx()
    {
        string gameName = "";
        string shareContent = "";
        if (PageManager.Instance.CurrentPage is LandlordsPage)
        {
            gameName = "斗地主";
        }
        else if (PageManager.Instance.CurrentPage is MaJangPage)
        {
            gameName = "麻将";
        }
        shareContent = "我是"+UserInfoModel.userInfo.nickName+"("+UserInfoModel.userInfo.userId+")在" + gameName + roomId + "约牌房间中和好友在一起约牌，你也赶快加入我们吧  " + UserInfoModel.userInfo.downUrl + "  #" + roomId; //#+roomId必须放在最后！解析口令用
        //SDKManager.Instance.ShareText(SDKManager.WechatShareScene.WXSceneSession, shareContent);
        SDKManager.Instance.CopyToClipboard(shareContent);
        SDKManager.Instance.OpenWeChat();
    }


    #endregion




    #region 扫码panel
    /// <summary>
    /// 生成二维码
    /// </summary>
    void InitErweima()
    {
        Texture2D textrue = QRCode.GetQRTexture(QRCode.GetQRString(2,roomId));
        erweimaIcon.sprite = MiscUtils.TextureToSprite(textrue);
    }
    #endregion




    #region 在线好友QQpanel



    /// <summary>
    /// 邀请
    /// </summary>
    void Inivate()
    {
        List<FriendInfo> invateList = new List<FriendInfo>();
        for (int i = 0; i < friends.Count; i++)
        {
            if (friends[i].isOn())
                invateList.Add(friends[i].info);
        }
        if (invateList.Count == 0)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "请选择要邀请的好友");
            return;
        }
        for (int i = 0; i < invateList.Count; i++)
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
         {
             inviteFriendYuePai = new InviteFriendYuePai()
             {
                 tableId = roomId,
                 userId = invateList[i].userId
             },
             msgid = MessageId.C2G_InviteFriendYuePai
         });
        }

        Close();
    }

    public static void G2C_InvateSuccess()
    {
        TipManager.Instance.OpenTip(TipType.SimpleTip, "邀请成功");
    }

    void C2G_ReqFriendList()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_QueryFriendReq
        });
    }

    /// <summary>
    /// 得到服务器在线好友
    /// </summary>
    /// <param name="onLinefriends"></param>
    public static void G2C_OnLineFriend(List<FriendInfo> onLinefriends)
    {
        InvateNode node = NodeManager.GetNode<InvateNode>();
        if (node)
        {
            UIUtils.DestroyChildren(node.parent);
            node.friends.Clear();
            for (int i = 0; i < onLinefriends.Count; i++)
            {
                if (onLinefriends[i].isOnline == 1)
                {
                    InvateItem item = Instantiate(node.prefab, node.parent).GetComponent<InvateItem>();
                    item.Init(onLinefriends[i]);
                    node.friends.Add(item);
                }
            }
        }
    }


    #endregion
}
