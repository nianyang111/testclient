using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResultItem : MonoBehaviour {

    public Image headIcon;
    public GameObject isMe_name;
    public GameObject isMe_mask;
    public Image identyIcon;//身份图片
    public GameObject isWinObj;
    public Text nameLb;
    public Text AllresultLb;//总成绩
    public Text curResultLb;//这次的成绩

    public void Init(DdzJSPlayerInfo info)
    {
        StartCoroutine(MiscUtils.DownloadImage(info.photo, spr =>
            {
                headIcon.sprite = spr;
            }));
        if (identyIcon)
        {
            if (info.isDz)
                identyIcon.sprite = BundleManager.Instance.GetSprite("doudizhu_icon_dizhu2", LandlordsPage.Instance.GetSpriteAB());
            else
                identyIcon.sprite = BundleManager.Instance.GetSprite("doudizhu_icon_nongmin", LandlordsPage.Instance.GetSpriteAB());
        }
        if (info.userId == UserInfoModel.userInfo.userId)
        {
            nameLb.color = new Color(248 / 255f, 228 / 255f, 178 / 255f);
        }
        else
        {
            nameLb.color = new Color(193 / 255f, 95 / 255f, 36 / 255f);
        }
        nameLb.text = info.nickname.ToString();
        AllresultLb.text = info.totalIncome > 0 ? "+" + info.totalIncome : info.totalIncome.ToString();
        curResultLb.text = info.income > 0 ? "+" + info.income : info.income.ToString();
        if (isMe_name)
            isMe_name.SetActive(info.userId == UserInfoModel.userInfo.userId);
        if (isMe_mask)
            isMe_mask.SetActive(info.userId == UserInfoModel.userInfo.userId);
        if (isWinObj)
            isWinObj.SetActive(info.totalIncome > 0);
    }
}
