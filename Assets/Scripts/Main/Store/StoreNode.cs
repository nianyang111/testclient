using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 商店
/// </summary>
public class StoreNode : Node
{
    public Text cardNum, goldNum, agNum;
    public Toggle goldBtn, agBtn, cardBtn, vipBtn;
    public StoreGoldPanel goldPanel;
    public StoreAgPanel agPanel;
    public StoreCardPanel cardPanel;
    public StoreVipPanel vipPanel;
    public StoreRechargeResultPanel resultPanel;
    public override void Init()
    {
        base.Init();
        goldPanel.Init();
        agPanel.Init();
        cardPanel.Init();
        vipPanel.Init();
        resultPanel.Init(this);
        goldBtn.onValueChanged.AddListener((isOn) => { OpenPanel(); });
        agBtn.onValueChanged.AddListener((isOn) => { OpenPanel(); });
        cardBtn.onValueChanged.AddListener((isOn) => { OpenPanel(); });
        vipBtn.onValueChanged.AddListener((isOn) => { OpenPanel(); });
    }

    public override void Open()
    {
        base.Open();
        goldPanel.Open();
        agPanel.Open();
        cardPanel.Open();
        vipPanel.Open();
        FlushData();
        OpenPanel();
        resultPanel.gameObject.SetActive(false);
    }
    /// <summary>
    /// 选择打开界面
    /// </summary>
    public void OpenPanel()
    {
        goldPanel.gameObject.SetActive(goldBtn.isOn);
        agPanel.gameObject.SetActive(agBtn.isOn);
        cardPanel.gameObject.SetActive(cardBtn.isOn);
        vipPanel.gameObject.SetActive(vipBtn.isOn);
    }
    /// <summary>
    /// 更新显示
    /// </summary>
    public void FlushData()
    {
        cardNum.text = string.Format("X" + UserInfoModel.userInfo.roomCardNum);
        goldNum.text = UserInfoModel.userInfo.walletGoldBarNum.ToString();
        agNum.text = UserInfoModel.userInfo.walletAgNum.ToString();
    }
    /// <summary>
    /// 游戏货币购买物品回调
    /// </summary>
    public static void ShowRechargeResult(net_protocol.BuyGoodsInStoreResp resp)
    {
        if (resp.result != 1)
        {
            TipManager.Instance.OpenTip(TipType.AlertTip, "购买失败");
            return;
        }
        else
        {
            var node = NodeManager.GetNode<StoreNode>();
            if (node == null) return;
            switch (resp.type)
            {
                case 0:
                    node.goldPanel.rechargePanel.gameObject.SetActive(false);
                    break;
                case 1:
                    node.agPanel.rechargePanel.gameObject.SetActive(false);
                    break;
                case 2:
                    node.cardPanel.rechargePanel.gameObject.SetActive(false);
                    break;
                case 3:
                    node.vipPanel.rechargePanel.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
            TipManager.Instance.OpenTip(TipType.AlertTip, "购买成功");
        }
    }
    /// <summary>
    /// sdk购买物品
    /// </summary>
    public static void ChargeNoticFinish(net_protocol.ChargeNotice resp)
    {
        var node = NodeManager.GetNode<StoreNode>();
        if (node)
            node.resultPanel.Open(resp);
    }

    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
        goldPanel.Close();
        agPanel.Close();
        cardPanel.Close();
        vipPanel.Close();
    }
    private void OnClickGoldBtn(int type,int count,bool isVip=false)
    {
        if (isVip)
        {
            //  0：金条 1：银币 2：道具 3：vip
            SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
            {
                msgid = net_protocol.MessageId.C2G_BuyGoodsInStoreReq,
                BuyGoodsInStoreReq = new net_protocol.BuyGoodsInStoreReq()
                {
                    type = type,
                    vipType = count,
                }
            });
        }
        else
        {
            //  0：金条 1：银币 2：道具 3：vip
            SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
            {
                msgid = net_protocol.MessageId.C2G_BuyGoodsInStoreReq,
                BuyGoodsInStoreReq = new net_protocol.BuyGoodsInStoreReq()
                {
                    type = type,
                    counts = count,
                }
            });
        }
    }
    void OnGUI()
    {
        if (PageManager.Instance.isDebugLog)
        {
            if (GUILayout.Button("金币"))
            {
                OnClickGoldBtn(0, 600);
            }
            if (GUILayout.Button("银币"))
            {
                OnClickGoldBtn(1, 60000);
            }
            if (GUILayout.Button("房卡"))
            {
                OnClickGoldBtn(2, 23);
            }
            if (GUILayout.Button("vip"))
            {
                OnClickGoldBtn(3, 2, true);
            }
        }
    }
}
