using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagSeePanel : MonoBehaviour {
    public Button tureBtn;
    public Text showItem, redeemName, acc, pwd;

    public void Init()
    {
        UGUIEventListener.Get(tureBtn.gameObject).onClick = (g) => { gameObject.SetActive(false); };
        gameObject.SetActive(false);
    }
    public void OpenPanel(BagRedeemData data)
    {
        gameObject.SetActive(true);
        showItem.text = data.name;
        redeemName.text = data.name;
        acc.text = data.account;
        pwd.text = data.pwd;
    }
}
