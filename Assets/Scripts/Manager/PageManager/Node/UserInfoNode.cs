using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class UserInfoNode : Node {

    [HideInInspector]
    public Text idLb, agLb, goldLb, roomCardLb, nameLb, cityLb, lvLb, vipLb, expLb;
    //[HideInInspector]
    public GameObject cityBtn, vipMessageBtn,vipRechargeBtn, changeUserBtn, useRechargeBtn, cityPanel;
    [HideInInspector]
    public Toggle boy, girl;
    [HideInInspector]
    public Slider expSlider;
    public Image headIcon,vipIcon,userCode;

    public override void Init()
    {
        base.Init();
        idLb.text = UserInfoModel.userInfo.userId.ToString();
        agLb.text = UserInfoModel.userInfo.walletAgNum.ToString();
        goldLb.text = UserInfoModel.userInfo.walletGoldBarNum.ToString();
        roomCardLb.text = UserInfoModel.userInfo.roomCardNum.ToString();
        nameLb.text = UserInfoModel.userInfo.nickName;
        cityLb.text = UserInfoModel.userInfo.province;
        headIcon.sprite = UserInfoModel.userInfo.headIconSprite;        
        JsonData curLvJson = UserInfoModel.GetLvJsonData(UserInfoModel.userInfo.level);
        string levelDes = curLvJson["designation"].ToString();
        lvLb.text = "Lv:" + UserInfoModel.userInfo.level + "(" + levelDes + ")";

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
                             
        boy.isOn = UserInfoModel.userInfo.sex == 0;
        girl.isOn = UserInfoModel.userInfo.sex == 1;        
    }

    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(cityBtn).onClick = delegate { cityPanel.gameObject.SetActive(true); };
        UGUIEventListener.Get(vipMessageBtn).onClick = delegate { NodeManager.OpenNode<StoreNode>().vipBtn.isOn = true; };
        UGUIEventListener.Get(vipRechargeBtn).onClick = delegate { NodeManager.OpenNode<StoreNode>().vipBtn.isOn = true; };
        UGUIEventListener.Get(changeUserBtn).onClick = delegate { ChangeUser(); };
        UGUIEventListener.Get(useRechargeBtn).onClick = delegate { print("点击充值卡"); };
        boy.onValueChanged.AddListener(delegate { SetSix(0, boy.isOn); });
        girl.onValueChanged.AddListener(delegate { SetSix(1, girl.isOn); });
        vipIcon.gameObject.SetActive(UserInfoModel.userInfo.vipCard > 0);
        GetCodeTextrue();
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_IsVipUserReq,
        });
    }
    /// <summary>
    /// 获取二维码图片
    /// </summary>
    private void GetCodeTextrue()
    {
        Texture2D textrue = QRCode.GetQRTexture(QRCode.GetQRString(1,UserInfoModel.userInfo.userId.ToString()));
        userCode.sprite = MiscUtils.TextureToSprite(textrue);
    }

    /// <summary>
    /// 切换账号
    /// </summary>
    void ChangeUser()
    {
        TipManager.Instance.OpenTip(TipType.ChooseTip, "确定退出当前账号吗?", 10, () =>
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_LoginOut,
                loginOut = new LoginOut()
                {
                    id=UserInfoModel.userInfo.userId
                }
            });
            LoginPage.DeleteLoginInfo();
            PageManager.Instance.OpenPage<LoginPage>();
        });
    }

    /// <summary>
    /// 设置性别  0男1女
    /// </summary>
    void SetSix(int six, bool isOn)
    {
        if (isOn)
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_UpdateUser,
                updateUser = new UpdateUser()
                {
                    type = 0,
                    gender = six
                }
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
            expSlider.value = 1;
            expLb.text = "已满级"; // 1 * 100 + "%";
        }
        else
        {
            float ratio = UserInfoModel.userInfo.exp / (allExp * 1f);
            expSlider.value = ratio;
            expLb.text = (ratio * 100).ToString("F1") + "%";
        }
    }

    /// <summary>
    /// 设置地名
    /// </summary>
    public static void SetCity(string province)
    {
        UserInfoNode node = NodeManager.GetNode<UserInfoNode>();
        if (node)
        {
            node.cityLb.text = province;
        }
    }
    /// <summary>
    /// Vip时间更新
    /// </summary>
    public static void FinishVipDay(IsVipUserResp isVipUserResp)
    {
        var node = NodeManager.GetNode<UserInfoNode>();
        if (node)
        {
            UserInfoModel.userInfo.vipDay = isVipUserResp.TimeLeft;
            if (UserInfoModel.userInfo.vipCard == 0)
            {
                node.vipLb.text = "尚未开通";
                node.vipMessageBtn.SetActive(true);
                node.vipRechargeBtn.SetActive(false);
            }
            else
            {
                node.vipLb.text = "剩余" + UserInfoModel.userInfo.vipDay;
                node.vipMessageBtn.SetActive(false);
                node.vipRechargeBtn.SetActive(true);
            }
        }
    }

    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
    }
}
