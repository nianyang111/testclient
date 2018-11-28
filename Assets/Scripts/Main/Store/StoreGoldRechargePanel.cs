using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreGoldRechargePanel : MonoBehaviour
{
    public Text itemName, rmbNum, agNum;
    public Image agImage, agIcon;
    public GameObject alipayBtn, weChatBtn, agBtn, closeBtn;
    StoreGoldData _data;
    public void Init()
    {
        UGUIEventListener.Get(alipayBtn).onClick = (g) => { SDKManager.Instance.CreateNewAliPayOrder(_data.id); };
        UGUIEventListener.Get(weChatBtn).onClick = (g) => { SDKManager.Instance.CreateNewWechatPayOrder(_data.id); };
        UGUIEventListener.Get(agBtn).onClick = (g) => { OnClickAgBtn(); };
        UGUIEventListener.Get(closeBtn).onClick = (g) => { gameObject.SetActive(false); };
        gameObject.SetActive(false);
    }
    private void OnClickAgBtn()
    {
        //  0：金条 1：银币 2：道具 3：vip
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_BuyGoodsInStoreReq,
            BuyGoodsInStoreReq = new net_protocol.BuyGoodsInStoreReq()
            {
                type = 0,
                counts = _data.itemNum,
                sliver = _data.replaceNum
            }
        });
    }
    public void Open(StoreGoldData data)
    {
        gameObject.SetActive(true);
        _data = data;
        agImage.color = agIcon.color = _data.replaceNum <= UserInfoModel.userInfo.walletAgNum ? Color.white : Color.black;
        agImage.raycastTarget = _data.replaceNum <= UserInfoModel.userInfo.walletAgNum;
        itemName.text = string.Format(_data.itemNum + "金条");
        rmbNum.text = string.Format(_data.rmbNum + "元");
        agNum.text = _data.replaceNum.ToString();
    }
}
