using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 排行榜玩家详细信息
/// </summary>
public class RankPlayInfoPanel : MonoBehaviour
{
    [HideInInspector]
    public Button closeBtn, reportBtn;
    [HideInInspector]
    public Image playicon, sexIcon, exeSlider, addBtn;
    [HideInInspector]
    public Text playName, playId, agNum, goldNum, LvText, addText;
    RankData _data;
    public void Init()
    {
        UGUIEventListener.Get(closeBtn.gameObject).onClick = (g) => { ClosePanel(); };
        UGUIEventListener.Get(reportBtn.gameObject).onClick = (g) => { ReportPlay(); };
        UGUIEventListener.Get(addBtn.gameObject).onClick = (g) => { AddFriends(); };
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 打开Panel
    /// </summary>
    public void OpenPanel(RankData data)
    {
        _data = data;
        SetValue();
        gameObject.SetActive(true);
        if (_data.userId != UserInfoModel.userInfo.userId)
            QueryRelation(_data.userId);
    }
    /// <summary>
    /// 关闭Panel
    /// </summary>
    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 设置数据
    /// </summary>
    private void SetValue()
    {
        addBtn.gameObject.SetActive(UserInfoModel.userInfo.userId != _data.userId);
        sexIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_btn_" + _data.gender);
        playName.text = _data.nickName;
        LvText.text = "Lv" + _data.level;
        playId.text = "ID:" + _data.userId;
        playicon.sprite = _data.iconSprite;
        agNum.text = _data.ag.ToString();
        goldNum.text = _data.gold.ToString();
        LitJson.JsonData nextLvJson = UserInfoModel.GetLvJsonData(UserInfoModel.userInfo.level + 1);
        long allExp = long.Parse(nextLvJson["exp"].ToString());
        exeSlider.fillAmount = (float)(_data.curExp / allExp);
        print("玩家经验：" + _data.curExp + "——" + allExp);
    }
    /// <summary>
    /// 查询好友关系
    /// </summary>
    public void QueryRelation(int userId)
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
                addText.text = "+好友";
                addBtn.color = Color.white;
                addBtn.raycastTarget = true;
                break;
            case FriendApplyState.MeAppling:
                addText.text = "已申请";
                addBtn.color = Color.black;
                addBtn.raycastTarget = false;
                break;
            case FriendApplyState.HisAppling:
                addText.text = "+好友";
                addBtn.color = Color.white;
                addBtn.raycastTarget = true;
                break;
            case FriendApplyState.Friending: 
                addText.text = "已是好友";
                addBtn.color = Color.black;
                addBtn.raycastTarget = false;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 举报玩家
    /// </summary>
    private void ReportPlay()
    {
        NodeManager.OpenNode<ReportNode>(null, null, false, false).Inits(_data.userId, false); 
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 加好友
    /// </summary>
    private void AddFriends()
    {
        SocialModel.Instance.AddFriend(_data.userId);
        addText.text = "已申请";
        addBtn.color = Color.black;
        addBtn.raycastTarget = false;
    }
}