using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 聊天Item
/// </summary>
public class MatchChatItem : MonoBehaviour, IPointerClickHandler
{
    public GameObject itemBg, title;
    public GameObject reportBtn, addBtn;
    public Text content, nameText;
    public MatchChatPanel panel;
    public int userId;
    public void Init(net_protocol.MatcherChatResp resp)
    {
        content.text = resp.userName + ":" + resp.content;
        nameText.text = resp.userName;
        userId = resp.userId;
        UGUIEventListener.Get(reportBtn).onClick = delegate { ReportPlay(); };
        UGUIEventListener.Get(addBtn).onClick = delegate { AddPlay(); };
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
        panel.CurItem = this;
    }
    /// <summary>
    /// 查询好友关系
    /// </summary>
    private void QueryRelation(int userId)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            queryRelationReq = new QueryRelationReq()
            {
                userId = userId
            },
            msgid = MessageId.C2G_QueryRelationReq
        }, true);
    }
    /// <summary>
    /// 查询好友关系完成
    /// </summary>
    public void QueryRelationFinish(int relation)
    {
        FriendApplyState state = SocialModel.Instance.getFriendState(relation);
        switch (state)
        {
            case FriendApplyState.Normal:
                SocialModel.Instance.AddFriend(userId); TipManager.Instance.OpenTip(TipType.SimpleTip, "发送成功，等待通过", 1f);
                break;
            case FriendApplyState.MeAppling:
                TipManager.Instance.OpenTip(TipType.SimpleTip, "已发送好友申请");
                break;
            case FriendApplyState.HisAppling:
                TipManager.Instance.OpenTip(TipType.SimpleTip, "已发送好友申请");
                break;
            case FriendApplyState.Friending:
                TipManager.Instance.OpenTip(TipType.SimpleTip, "对方已经是好友了");
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 添加好友
    /// </summary>
    private void AddPlay()
    {
        if (userId != UserInfoModel.userInfo.userId)
            QueryRelation(userId);
        else
            TipManager.Instance.OpenTip(TipType.SimpleTip, "不能添加自己为好友");
    }

    /// <summary>
    /// 举报玩家
    /// </summary>
    private void ReportPlay()
    {
        if (userId != UserInfoModel.userInfo.userId)
            NodeManager.OpenNode<ReportNode>(null, null, false, false).Inits(userId, false);
    }
    public void Select()
    {
        itemBg.SetActive(true);
        title.SetActive(true);
        panel.show.gameObject.SetActive(true);
        title.transform.SetParent(panel.show);
    }
    public void OnSelect()
    {
        itemBg.SetActive(false);
        title.SetActive(false);
        panel.show.gameObject.SetActive(false);
        title.transform.SetParent(this.transform);
    }
}
