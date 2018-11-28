using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject button1, button2;
    void Start()
    {
        UGUIEventListener.Get(button1).onClick = delegate { SDKManager.Instance.CreateNewWechatPayOrder(2001); };
        UGUIEventListener.Get(button2).onClick = delegate
        {
            SDKManager.Instance.ShareText(SDKManager.WechatShareScene.WXSceneSession, "12323sadfaseg");
        };
    }
}
