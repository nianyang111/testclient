using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvateItem : MonoBehaviour {

    public Text nameLb;
    public Text idLb;
    public Toggle chooseToggle;
    public CallBack<FriendInfo> OnValueChange;
    public FriendInfo info;
    public void Init(FriendInfo info)
    {
        this.info = info;
        nameLb.text = info.nickname;
        idLb.text = info.userId.ToString();
    }


    public bool isOn()
    {
        return chooseToggle.isOn;
    }
}
