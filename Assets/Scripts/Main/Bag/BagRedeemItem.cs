using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagRedeemItem : MonoBehaviour {
    public Text date;
    public Text itemName;
    public Button SeeBtn;
    public BagRedeemPanel _panel;
    BagRedeemData _data;
    public void Init(BagRedeemData data)
    {
        _data=data;
        date.text = _data.date;
        itemName.text = _data.name;
        SeeBtn.onClick.AddListener(() => {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            _panel.seePanel.OpenPanel(_data);
        });
    }
}
public class BagRedeemData
{
    public string id;
    public string date;
    public string name;
    public string goodsType;
    public string account;
    public string pwd;
    public int state;
    public int userId;
}