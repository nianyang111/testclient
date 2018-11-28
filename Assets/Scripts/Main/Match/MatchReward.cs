using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchReward : MonoBehaviour {
    public Text rewardName;
    public Image rewardIcon;
    public List<Reward> _data;
    public void SetValue(List<Reward> data)
    {
        _data = data;
        rewardName.text = data[0].name;
        if (_data[0].icon != "")
            StartCoroutine(MiscUtils.DownloadImage(_data[0].icon, spr => { if (spr != null)rewardIcon.sprite = spr; }));
        else
            Debug.Log("奖励图片地址为空：" + data[0].name);
    }

}