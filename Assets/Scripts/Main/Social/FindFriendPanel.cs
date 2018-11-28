using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindFriendPanel : MonoBehaviour {

    public GameObject closeBtn;
    public InputField input;
    public GameObject findBtn;

    /// <summary>
    /// item
    /// </summary>
    public GameObject itemObj;
    public Image headIcon;
    public Text nameLb;
    public Button addBtn;
    public Text btnText;

    void Start()
    {
        UGUIEventListener.Get(closeBtn).onClick = delegate { SetVisbel(false); };
        UGUIEventListener.Get(findBtn).onClick = delegate { Find(); };        
    }

    void OnEnable()
    {
        input.text = string.Empty;
        itemObj.SetActive(false);
    }

    /// <summary>
    /// 查找好友按钮回调
    /// </summary>
    void Find()
    {
        if (string.IsNullOrEmpty(input.text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "请输入要查找的玩家ID!");
            return;
        }
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                findFriendReq = new FindFriendReq()
                {
                    userId = int.Parse(input.text)
                },
                msgid = MessageId.C2G_FindFriendReq
            });
    }

    /// <summary>
    /// 服务器查找好友响应
    /// </summary>
    public static void G2C_Find(FindFriendResp resp)
    {
        if (resp == null || resp.friendInfo == null)
        {
            TipManager.Instance.OpenTip(TipType.AlertTip, "用户不存在");
            return;
        }

        var node = NodeManager.GetNode<SocialNode>();
        if (node)
            node.findPanel.LoadFriendItem(resp.friendInfo);
    }

    void LoadFriendItem(FriendInfo info)
    {
        input.text = string.Empty;
        itemObj.SetActive(true);
        StartCoroutine(MiscUtils.DownloadImage(info.photo, (spr) =>
        {
            headIcon.sprite = spr;
        }));
        nameLb.text = info.nickname;
        SetBtnState(info.relation);
        UGUIEventListener.Get(addBtn.gameObject).onClick = delegate
        {
            if (!addBtn.interactable)
                return;
            SocialModel.Instance.AddFriend(info.userId);
            SetBtnState(1);
        };
    }


    //设置按钮状态
    void SetBtnState(int relation)
    {
        FriendApplyState state = SocialModel.Instance.getFriendState(relation);
        btnText.horizontalOverflow = HorizontalWrapMode.Overflow;
        switch (state)
        {
            case FriendApplyState.Normal:
                addBtn.interactable = true;
                btnText.text = "添加";
                break;
            case FriendApplyState.MeAppling:
                addBtn.interactable = false;
                btnText.text = "已申请";
                break;
            case FriendApplyState.HisAppling:
                addBtn.interactable = false;
                btnText.text = "对方已申请";
                break;
            case FriendApplyState.Friending:
                addBtn.interactable = false;
                btnText.text = "已是好友";
                break;
            default:
                break;
        }
    }

    public void SetVisbel(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

}
