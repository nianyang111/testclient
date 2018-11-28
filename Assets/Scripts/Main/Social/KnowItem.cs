using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowItem : MonoBehaviour {

    public Image headIcon;
    public Text nameLb;
    public Image sixIcon;
    public Text yinbiLb;

    public FriendInfo curInfo;

    public void Init(FriendInfo info)
    {
        curInfo = info;
        if (gameObject.activeInHierarchy)
            StartCoroutine(MiscUtils.DownloadImage(info.photo, spr =>
            {
                headIcon.sprite = spr;
            }));
        nameLb.text = info.nickname;
        sixIcon.sprite = BundleManager.Instance.GetSprite("friend/" + (info.gender == 0 ? "haoyou_pic_nan" : "haoyou_pic_nv"));
        yinbiLb.text = info.sliver.ToString();

        UGUIEventListener.Get(headIcon.gameObject).onClick = delegate
        {
            SocialNode.HeadCall(info, false);
        };
    }

    void OnEnable()
    {
        if (curInfo != null)
        {
            if (headIcon.sprite == null)
            {
                StartCoroutine(MiscUtils.DownloadImage(curInfo.photo, spr =>
                {
                    headIcon.sprite = spr;
                }));
            }
            else if (headIcon.sprite.name != "")
            {
                StartCoroutine(MiscUtils.DownloadImage(curInfo.photo, spr =>
                {
                    headIcon.sprite = spr;
                }));
            }
        }
    }
}
