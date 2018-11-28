using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendRankItem : MonoBehaviour
{
    public Image rankIcon;
    public Text rankLb;
    public Image headIcon;
    public Text nameLb;
    public Image sixIcon;
    public Text yinbiLb;
    public Text stateLb;
    public GameObject headBtn;
    public GameObject giveBtn;
    public GameObject chatBtn;
    public FriendInfo curInfo;
    public void Init(FriendInfo info, int rank)
    {
        curInfo = info;
        if (rank <= 3)
        {
            if (rank == 0)
            {
                rankLb.gameObject.SetActive(false);
                rankIcon.gameObject.SetActive(false);
            }
            else
            {
                rankLb.gameObject.SetActive(false);
                rankIcon.gameObject.SetActive(true);
                rankIcon.sprite = BundleManager.Instance.GetSprite("common/normal_jiangbei_" + rank);
            }
        }
        else
        {
            rankLb.gameObject.SetActive(true);
            rankIcon.gameObject.SetActive(false);
            rankLb.text = rank.ToString();
        }
        if (gameObject.activeInHierarchy)
            StartCoroutine(MiscUtils.DownloadImage(info.photo, spr =>
                {
                    headIcon.sprite = spr;
                }));
        nameLb.text = info.nickname;
        sixIcon.sprite = BundleManager.Instance.GetSprite("friend/" + (info.gender == 0 ? "haoyou_pic_nan" : "haoyou_pic_nv"));
        yinbiLb.text = info.sliver.ToString();
        if (info.isOnline == 0)
        {
            stateLb.text = "[离线]";
            stateLb.color = Color.red;
        }
        else
        {
            stateLb.text = "[在线]";
            stateLb.color = Color.green;
        }        
        UGUIEventListener.Get(giveBtn).onClick = delegate 
        {
            SocialNode.GiveCall(info);
        };
        UGUIEventListener.Get(chatBtn).onClick = delegate 
        {
            SocialNode.ChatCall(info);
        };
        UGUIEventListener.Get(headBtn).onClick = delegate
        {
            SocialNode.HeadCall(info,true);
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
