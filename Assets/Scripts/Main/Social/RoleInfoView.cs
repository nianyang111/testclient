using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoleInfoView : MonoBehaviour {

    public Image headIcon;
    public Text idLb;
    public Text nameLb;
    public Image sixIcon;
    public Text yinbiLb;
    public Slider lvSlider;
    public Text lvLb;
    public GameObject vipObj;    
    public GameObject closeBtn;

    public GameObject delBtn;
    public GameObject chatBtn;
    public GameObject jubaoBtn;
    public Button addFriendBtn;
    public Text addFriendText;
    public CallBack<int> AddFriendCall;
    public BasePlayerInfo curInfo;
    void Start()
    {
        UGUIEventListener.Get(closeBtn).onClick = delegate { SetVisibel(false); };
    }

    public void Init(BasePlayerInfo info,bool isFriend)
    {
        curInfo = info;
        StartCoroutine(MiscUtils.DownloadImage(info.icon, spr =>
             {
                 headIcon.sprite = spr;
             }));
        idLb.text = info.uid;
        nameLb.text = info.userNickname;
        sixIcon.sprite = BundleManager.Instance.GetSprite(info.six == Six.boy ? "friend/haoyou_pic_nan" : "friend/haoyou_pic_nv");
        sixIcon.SetNativeSize();
        yinbiLb.text = info.money.ToString();
        vipObj.SetActive(info.vip > 0);
        try
        {//没到最大等级
            JsonData nextLvJson = UserInfoModel.GetLvJsonData(UserInfoModel.userInfo.level + 1);
            long allExp = long.Parse(nextLvJson["exp"].ToString());
            SetExpSlider(false, allExp);
        }
        catch
        {//到了最大等级
            SetExpSlider(true, 0);
        }
        if (isFriend)
        {
            delBtn.SetActive(true);
            chatBtn.SetActive(true);
            jubaoBtn.SetActive(false);
            addFriendBtn.gameObject.SetActive(false);
            UGUIEventListener.Get(delBtn).onClick = delegate { Del(); };
            UGUIEventListener.Get(chatBtn).onClick = delegate { Chat(); };
        }
        else
        {
            SetBtnState(info.relation);
            delBtn.SetActive(false);
            chatBtn.SetActive(false);
            jubaoBtn.SetActive(true);
            addFriendBtn.gameObject.SetActive(true);
            UGUIEventListener.Get(jubaoBtn).onClick = delegate { Jubao(); };
            UGUIEventListener.Get(addFriendBtn.gameObject).onClick = delegate { AddFriend(); };
        }        
    }

    void SetBtnState(int relation)
    {
        FriendApplyState state = SocialModel.Instance.getFriendState(relation);
        switch (state)
        {
            case FriendApplyState.Normal:
                addFriendBtn.interactable = true;
                addFriendText.text = "加好友";
                break;
            case FriendApplyState.MeAppling:
                addFriendBtn.interactable = false;
                addFriendText.text = "已申请";
                break;
            case FriendApplyState.HisAppling:
                addFriendBtn.interactable = false;
                addFriendText.text = "对方已申请";
                break;
            case FriendApplyState.Friending:
                addFriendBtn.interactable = false;
                addFriendText.text = "已是好友";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 删除按钮回调
    /// </summary>
    void Del()
    {
        TipManager.Instance.OpenTip(TipType.ChooseTip, "确定删除该好友?删除后将无法与TA聊天,相应的聊天记录也将立即清除", 0, () =>
            {
                SocialModel.Instance.DeleteFriend(int.Parse(curInfo.uid));
            });
        SetVisibel(false);
    }

    /// <summary>
    /// 会话按钮回调
    /// </summary>
    void Chat()
    {
        FriendInfo info = new FriendInfo();
        info.exp = curInfo.exp;
        info.gender = curInfo.six == Six.boy ? 0 : 1;
        info.level = curInfo.lv;
        info.nickname = curInfo.userNickname;
        info.photo = curInfo.icon;
        info.sliver = curInfo.money;
        info.userId = int.Parse(curInfo.uid);
        SocialNode.ChatCall(info);
        SetVisibel(false);
    }

    void Jubao()
    {
        NodeManager.OpenNode<ReportNode>(null, null, false).Inits(int.Parse(curInfo.uid), true);
    }

    void AddFriend()
    {
        if (!addFriendBtn.interactable)
            return;
        SocialModel.Instance.AddFriend(int.Parse(curInfo.uid));
        curInfo.relation = 1;
        if (AddFriendCall != null)
            AddFriendCall(curInfo.relation);
        SetVisibel(false);
    }

    /// <summary>
    /// 设置经验
    /// </summary>
    void SetExpSlider(bool isMax, long allExp)
    {
        if (isMax)
        {
            lvSlider.value = 1;
        }
        else
        {
            float ratio = curInfo.exp / (allExp * 1f);
            lvSlider.value = ratio;
        }
    }


    public void SetVisibel(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}
