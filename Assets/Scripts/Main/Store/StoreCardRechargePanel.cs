using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreCardRechargePanel : MonoBehaviour {
    public GameObject closeBtn, alipayBtn, weChatBtn;
    public Text itemName, rmbNum;
    StoreCardData _data;
    public void Init()
    {
        UGUIEventListener.Get(alipayBtn).onClick = (g) => { SDKManager.Instance.CreateNewAliPayOrder(_data.id); };
        UGUIEventListener.Get(weChatBtn).onClick = (g) => { SDKManager.Instance.CreateNewWechatPayOrder(_data.id); };
        UGUIEventListener.Get(closeBtn).onClick = (g) => { gameObject.SetActive(false); };
        gameObject.SetActive(false);
    }
    public void Open(StoreCardData data)
    {
        gameObject.SetActive(true);
        _data = data;
        itemName.text = _data.itemName;
        rmbNum.text = string.Format(_data.rmbNum + "元");
    }
}
