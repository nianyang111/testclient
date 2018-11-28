using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShareItem : MonoBehaviour {

    public Text rankLb;
    public GameObject winerObj;
    public Image headIcon;
    public Text nameLb;
    public Text winCountLb;
    public Text scoreLb;
    public Image bg;

    public void Inits(ShareItemInfo info)
    {
        rankLb.text = info.rank.ToString();
        winerObj.SetActive(info.rank == 1);
        headIcon.sprite = info.headIcon;
        nameLb.text = info.name;
        winCountLb.text = "胜局:" + info.winCount;
        scoreLb.text = info.score > 0 ? "+" + info.score + "分" : info.score + "分";
        bg.sprite = BundleManager.Instance.GetSprite(info.rank == 1 ? "yuepai/weixinyuepai_panel_yunwenhuang" : "yuepai/weixinyuepai_panel_yunwenqianhuang");
    }

}

