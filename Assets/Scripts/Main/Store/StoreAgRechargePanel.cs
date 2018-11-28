using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreAgRechargePanel : MonoBehaviour {
    public Text itemName, rmbNum, goldNum;
    public Image goldImage, goldIcon;
    public GameObject alipayBtn, weChatBtn, goldBtn, closeBtn;
    StoreAgData _data;
    public void Init()
    {
        UGUIEventListener.Get(alipayBtn).onClick = delegate { SDKManager.Instance.CreateNewAliPayOrder(_data.id); };
        UGUIEventListener.Get(weChatBtn).onClick = delegate { SDKManager.Instance.CreateNewWechatPayOrder(_data.id); };
        UGUIEventListener.Get(goldBtn).onClick = delegate { OnClickGoldBtn(); };
        UGUIEventListener.Get(closeBtn).onClick = delegate { gameObject.SetActive(false); };
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
                type = 1,
                counts = _data.itemNum,
                golds = _data.replaceNum
            }
        });
    }

    public void Open(StoreAgData data)
    {
        gameObject.SetActive(true);
        _data = data;
        goldImage.color = goldIcon.color = _data.replaceNum <= UserInfoModel.userInfo.walletGoldBarNum ? Color.white : Color.black;
        goldImage.raycastTarget = _data.replaceNum <= UserInfoModel.userInfo.walletGoldBarNum;
        itemName.text = _data.itemName;
        rmbNum.text = string.Format(_data.rmbNum + "元");
        goldNum.text = _data.replaceNum.ToString();
    }
}
