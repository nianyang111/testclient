using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YuepaiResultItem : MonoBehaviour {

    public GameObject winObj;
    public Image headIcon;
    public Text nameLb;
    public Text allResultLb;

    public void Inits(CardResultShowNode.YuepaiLogPlayerInfo info)//是否赢最多
    {
        winObj.SetActive(info.isMax);
        StartCoroutine(MiscUtils.DownloadImage(info.headIcon, spr =>
            {
                headIcon.sprite = spr;
                info.headIconSpr = spr;
            }));
        nameLb.text = info.nickname;
        allResultLb.text = info.allResult + "分";
    }
}
