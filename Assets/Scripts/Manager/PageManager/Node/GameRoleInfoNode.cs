using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoleInfoNode : Node
{

    public Image headIcon;
    public Text idLb;
    public Text nameLb;
    public Image sixIcon;
    public Text yinbiLb;
    public Slider lvSlider;
    public Text lvLb;
    public Text zhanjiLb;
    public Text ratioLb;
    public Text posLb;
    public GameObject vipObj;

    public GameObject friendParentObj;
    public GameObject addFriendBtn;
    public Text isFriendText;

    public GameObject jubaoBtn;

    public GameObject tizouBtn;
    public Text tizouText;

    public GameObject closeBtn;

    public PlayerBaseInfo curInfo;

    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(closeBtn).onClick = delegate { Close(); };
    }

    public void Inits(int userId)
    {
        //请求玩家数据
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                queryDdzPlayerInfo = new QueryDdzPlayerInfo()
                {
                    id = userId
                },
                msgid = MessageId.C2G_QueryPlayerBaseInfoReq
            });
    }
    public static void SetPlayerInfo(PlayerBaseInfo info)
    {
        GameRoleInfoNode gameRole_node = NodeManager.GetNode<GameRoleInfoNode>();
        if (gameRole_node)
        {
            gameRole_node.SetRolrInfo(info);
        }

    }

    void SetRolrInfo(PlayerBaseInfo info)
    {
        curInfo = info;
        ReqIsFriend(info.userId);
        StartCoroutine(MiscUtils.DownloadImage(info.photo, spr =>
            {
                headIcon.sprite = spr;
            }));

        idLb.text = info.userId.ToString();
        nameLb.text = info.nickname;
        yinbiLb.text = info.silver.ToString();
        posLb.text = info.location;
        vipObj.SetActive(info.vip > 0);
        SetSix();
        SetLv();
        SetFightLog();
        SetBtnVisible();

    }

    void ReqIsFriend(int userid)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                queryRelationReq = new QueryRelationReq()
                {
                    userId = userid
                },
                msgid = MessageId.C2G_QueryRelationReq
            });
    }

    void SetSix()
    {
        string six = "";
        switch (curInfo.gender)
        {
            case 0:
                six = "gerenhzongxin_btn_nan";
                break;
            case 1:
                six = "gerenhzongxin_btn_nv";
                break;
        }
        sixIcon.sprite = BundleManager.Instance.GetSprite(six, PageManager.Instance.gamecommonBundle);
    }

    void SetLv()
    {
        JsonData curLvJson = UserInfoModel.GetLvJsonData(UserInfoModel.userInfo.level);
        lvLb.text = "LV" + curInfo.level;

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
    }

    void SetFightLog()
    {
        zhanjiLb.text = string.Format("{0}胜/{1}负", curInfo.won, curInfo.lost);
        ratioLb.text = curInfo.rate * 100 + "%";
    }

    public void SetFriend(int relation)
    {
        FriendApplyState state = SocialModel.Instance.getFriendState(relation);
        switch (state)
        {
            case FriendApplyState.Normal:
                addFriendBtn.SetActive(true);
                isFriendText.gameObject.SetActive(false);
                UGUIEventListener.Get(addFriendBtn).onClick = delegate { SocialModel.Instance.AddFriend(curInfo.userId); Close(); };
                break;
            case FriendApplyState.MeAppling:
                addFriendBtn.SetActive(false);
                isFriendText.gameObject.SetActive(true);
                isFriendText.text = "我已申请";
                break;
            case FriendApplyState.HisAppling:
                addFriendBtn.SetActive(false);
                isFriendText.gameObject.SetActive(true);
                isFriendText.text = "对方已申请";
                break;
            case FriendApplyState.Friending:
                addFriendBtn.SetActive(false);
                isFriendText.gameObject.SetActive(true);
                isFriendText.text = "已是好友";
                break;
            default:
                break;
        }

        friendParentObj.SetActive(curInfo.userId != UserInfoModel.userInfo.userId);
        UGUIEventListener.Get(addFriendBtn).onClick = delegate { SocialModel.Instance.AddFriend(curInfo.userId); Close(); };
    }

    void SetBtnVisible()
    {
        if (PageManager.Instance.CurrentPage is LandlordsPage)
        {
            if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType == RoomType.RoomCard)
            {
                jubaoBtn.SetActive(false);
                tizouBtn.SetActive(false);
            }
            else
            {
                if (curInfo.userId == UserInfoModel.userInfo.userId)
                {
                    jubaoBtn.SetActive(false);
                    tizouBtn.SetActive(false);
                }
                else
                {
                    jubaoBtn.SetActive(true);
                    tizouBtn.SetActive(true);
                    UGUIEventListener.Get(jubaoBtn).onClick = delegate { Jubao(); };
                    UGUIEventListener.Get(tizouBtn, AudioManager.AudioSoundType.BtnTizou).onClick = delegate
                    {
                        gameObject.SetActive(false);
                        Tizou();
                    };
                    tizouText.text = curInfo.vip > 0 ? "请他离开" : "踢走";
                }
            }
        }
        else
        {
            if (MaJangPage.Instance.roomType == RoomType.RoomCard)
            {
                jubaoBtn.SetActive(false);
                tizouBtn.SetActive(false);
            }
            else
            {
                if (curInfo.userId == UserInfoModel.userInfo.userId)
                {
                    jubaoBtn.SetActive(false);
                    tizouBtn.SetActive(false);
                }
                else
                {
                    jubaoBtn.SetActive(true);
                    tizouBtn.SetActive(true);
                    UGUIEventListener.Get(jubaoBtn).onClick = delegate { Jubao(); };
                    UGUIEventListener.Get(tizouBtn, AudioManager.AudioSoundType.BtnTizou).onClick = delegate
                    {
                        gameObject.SetActive(false);
                        Tizou();
                    };
                    tizouText.text = curInfo.vip > 0 ? "请他离开" : "踢走";
                }
            }
        }
    }

    void Jubao()
    {
        NodeManager.OpenNode<ReportNode>(null, null, false).Inits(curInfo.userId, true);
    }

    /// <summary>
    /// 踢人
    /// </summary>
    void Tizou()
    {
        if (UserInfoModel.userInfo.vipCard == 0)
        {//自己非VIP
            NodeManager.OpenNode<StoreNode>();
            return;
        }
        if (curInfo.vip > 0)
        {//对方是VIP
            TipManager.Instance.OpenTip(TipType.SimpleTip, "对方是VIP，不能被踢走哦");
            return;
        }
        if (PageManager.Instance.CurrentPage is LandlordsPage)
        {
            if (LandlordsModel.Instance.IsInFight)
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "游戏中不能踢人哦！");
                return;
            }
            LandlordsNet.C2G_KickRep(curInfo.userId);
        }
        else if (PageManager.Instance.CurrentPage is MaJangPage)
        {
            if (MaJangPage.Instance.currentStatu == 1)
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "游戏中不能踢人哦！");
                return;
            }
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
                {
                    mjKickPlayerReq = new MjKickPlayerReq()
                    {
                        userId = curInfo.userId
                    },
                    msgid = MessageId.C2G_MjKickPlayer
                });
        }
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
}
