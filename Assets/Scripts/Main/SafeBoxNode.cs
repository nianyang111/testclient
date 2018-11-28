using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 保险箱
/// </summary>
public class SafeBoxNode : Node
{
    private enum SafeBoxType
    {
        Ag,
        Gold
    }
    public Image safeIcon1, safeIcon2, addBtn, cutBtn;
    public Text title, wall, bank, Des;
    public Tab agBtn, goldBtn;
    public Button reseBtn, trueBtn;
    public Slider safeSlider;
    private SafeBoxType boxType = SafeBoxType.Ag;
    private long allNum;
    private long curNum;
    private int pitchNum = 10000;
    private bool isChange = false;
    public override void Init()
    {
        base.Init();
        UGUIEventListener.Get(agBtn.gameObject).onClick = (g) => { if (boxType == SafeBoxType.Ag)return; SelectType(SafeBoxType.Ag); };
        UGUIEventListener.Get(goldBtn.gameObject).onClick = (g) => { if (boxType == SafeBoxType.Gold)return; SelectType(SafeBoxType.Gold); };
        UGUIEventListener.Get(addBtn.gameObject).onClick = (g) => { curNum += pitchNum; AOrS(); };
        UGUIEventListener.Get(cutBtn.gameObject).onClick = (g) => { curNum -= pitchNum; AOrS(); };
        UGUIEventListener.Get(reseBtn.gameObject).onClick = (g) => { SelectType(boxType); };
        UGUIEventListener.Get(trueBtn.gameObject).onClick = (g) => { OnClickTrue(); };
        safeSlider.onValueChanged.AddListener(value => SliderChangeValue(value));
    }
    public override void Open()
    {
        base.Open();
        SelectType();
    }
    /// <summary>
    /// 确定交易
    /// </summary>
    private void OnClickTrue()
    {
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_UpdateUserSafeBox,
            UpdateUserSafeBoxReq = new net_protocol.UpdateUserSafeBoxReq()
            {
                type = boxType == SafeBoxType.Ag ? 1 : 2,
                userCount = curNum,
                BoxCount = (allNum - curNum)
            }
        }, true);

    }
    /// <summary>
    /// 确定交易回调
    /// </summary>
    /// <param name="resp"></param>
    public void OnClickTrueFinish(int resp)
    {
        TipManager.Instance.OpenTip(TipType.SimpleTip, "操作成功", 1f);
        if (boxType == SafeBoxType.Ag)
        {
            UserInfoModel.userInfo.walletAgNum = curNum;
            UserInfoModel.userInfo.bankAgNum = (allNum - curNum);
        }
        else
        {
            UserInfoModel.userInfo.walletGoldBarNum = curNum;
            UserInfoModel.userInfo.bankGoldBarNum = (allNum - curNum);
        }
    }
    /// <summary>
    /// 选择种类
    /// </summary>
    /// <param name="type"></param>
    private void SelectType(SafeBoxType type = SafeBoxType.Ag)
    {
        boxType = type;
        switch (boxType)
        {
            case SafeBoxType.Ag:
                safeIcon1.sprite = safeIcon2.sprite = BundleManager.Instance.GetSprite("common/normal_log_1");
                Des.text = string.Format("1.资产在20000银币以上的玩家可通过保险箱存取银币\n 2.通过“+”,“-”可以10000银币为单位进行调节");
                title.text = "银币保险箱";
                pitchNum = 10000;
                break;
            case SafeBoxType.Gold:
                safeIcon1.sprite = safeIcon2.sprite = BundleManager.Instance.GetSprite("common/normal_log_2");
                Des.text = string.Format("1.资产在200金条以上的玩家可通过保险箱存取银币\n 2.通过“+”,“-”可以100金条为单位进行调节");
                title.text = "金条保险箱";
                pitchNum = 100;
                break;
            default:
                break;
        }
        GetData();
    }
    /// <summary>
    /// 根据类型显示数据
    /// </summary>
    private void GetData()
    {
        switch (boxType)
        {
            case SafeBoxType.Ag:
                allNum = UserInfoModel.userInfo.walletAgNum + UserInfoModel.userInfo.bankAgNum;
                curNum = UserInfoModel.userInfo.walletAgNum;
                wall.text = UserInfoModel.userInfo.walletAgNum.ToString();
                bank.text = UserInfoModel.userInfo.bankAgNum.ToString();
                isChange = allNum > 20000;
                addBtn.raycastTarget = cutBtn.raycastTarget = allNum > 20000;
                addBtn.color = cutBtn.color = allNum > 20000 ? Color.white : Color.black;
                break;
            case SafeBoxType.Gold:
                allNum = UserInfoModel.userInfo.walletGoldBarNum + UserInfoModel.userInfo.bankGoldBarNum;
                curNum = UserInfoModel.userInfo.walletGoldBarNum;
                wall.text = UserInfoModel.userInfo.walletGoldBarNum.ToString();
                bank.text = UserInfoModel.userInfo.bankGoldBarNum.ToString();
                isChange = allNum > 200;
                addBtn.raycastTarget = cutBtn.raycastTarget = allNum > 200;
                addBtn.color = cutBtn.color = allNum > 200 ? Color.white : Color.black;
                break;
            default:
                break;
        }
        if (isChange)
            SetSliderValue(curNum, allNum);
        else
            SetSliderValue(0, 0);
    }
    /// <summary>
    /// 设置滑动条
    /// </summary>
    private void SetSliderValue(long cur, long all)
    {
        float dif = (float)cur / (float)all;
        safeSlider.onValueChanged.RemoveAllListeners();
        safeSlider.value = dif;
        safeSlider.onValueChanged.AddListener(value => SliderChangeValue(value));
    }
    /// <summary>
    /// 增减金额
    /// </summary>
    private void AOrS()
    {
        if (curNum > allNum)
            curNum = allNum;
        if (curNum < 0)
            curNum = 0;
        wall.text = curNum.ToString();
        bank.text = (allNum - curNum).ToString();
        SetSliderValue(curNum, allNum);
    }
    /// <summary>
    /// 滑动条事件
    /// </summary>
    private void SliderChangeValue(float value)
    {
        if (isChange)
        {
            if (1 - value < 0.01)
                value = 1f;
            curNum = (long)(allNum * value);
            wall.text = curNum.ToString();
            bank.text = (allNum - curNum).ToString();
        }
    }
    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
        allNum = 0;
        curNum = 0;
        isChange = false;
    }
}
