using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreVipRechargePanel : MonoBehaviour {
    public GameObject closeBtn, alipayBtn, weChatBtn, goldBtn;
    public Text itemName, rmbNum, goldNum;
    public Image goldIcon, goldImage;
    StoreVipData _data;
    public void Init()
    {
        UGUIEventListener.Get(alipayBtn).onClick = (g) => { SDKManager.Instance.CreateNewAliPayOrder(_data.id); };
        UGUIEventListener.Get(weChatBtn).onClick = (g) => { SDKManager.Instance.CreateNewWechatPayOrder(_data.id); };
        UGUIEventListener.Get(goldBtn).onClick = (g) => { OnClickGoldBtn(); };
        UGUIEventListener.Get(closeBtn).onClick = (g) => { gameObject.SetActive(false); };
        gameObject.SetActive(false);
    }
    private void OnClickGoldBtn()
    {
        //  0：金条 1：银币 2：道具 3：vip
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_BuyGoodsInStoreReq,
            BuyGoodsInStoreReq = new net_protocol.BuyGoodsInStoreReq()
            {
                type = 3,
                golds = _data.goldNum,
                vipType = _data.vipDay == 7 ? 1 : 2,
            }
        });
    }
    public void Open(StoreVipData data)
    {
        gameObject.SetActive(true);
        _data = data;
        goldImage.color = goldIcon.color = _data.goldNum <= UserInfoModel.userInfo.walletGoldBarNum ? Color.white : Color.black;
        goldImage.raycastTarget = _data.goldNum <= UserInfoModel.userInfo.walletGoldBarNum;
        goldNum.text = _data.goldNum.ToString();
        itemName.text = _data.itemName;
        rmbNum.text = string.Format(_data.rmbNum + "元");
    }
}
