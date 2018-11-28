using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatInfoItem : MonoBehaviour {

    public Image headIcon;    

    public GameObject yuyinObj;
    public Text timer;
    public GameObject yuyinBtn;

    public GameObject daziObj;
    public Text daziValue;
    public RectTransform daziBg;

    ChatInfo curInfo;
    string curYuyinPath;
    public void Inits(ChatInfo info)
    {
        curInfo = info;
        StartCoroutine(MiscUtils.DownloadImage(info.playerBaseInfo.icon,spr =>
            {
                headIcon.sprite = spr;
            }));
        if (info.type == 0)
        {//打字
            daziObj.SetActive(true);
            yuyinObj.SetActive(false);
            daziValue.text = info.text;
            daziBg.sizeDelta = new Vector2(daziValue.preferredWidth + 50, daziValue.preferredHeight + 50);
            (transform as RectTransform).sizeDelta = new Vector2((transform as RectTransform).sizeDelta.x, daziBg.sizeDelta.y);
        }
        else
        {//语音
            curYuyinPath = info.text;
            UGUIEventListener.Get(yuyinBtn).onClick = delegate { SocialModel.Instance.PlayRecord(curYuyinPath); };//播放语音
            daziObj.SetActive(false);
            yuyinObj.SetActive(true);
            timer.text = info.voice_timer + "\"";
        }
    }

    void OnEnable()
    {
        if (curInfo != null )
        {
            if (headIcon.sprite == null)
            {
                StartCoroutine(MiscUtils.DownloadImage(curInfo.playerBaseInfo.icon, spr =>
                {
                    headIcon.sprite = spr;
                }));
            }
            else if (headIcon.sprite.name != "")
            {
                StartCoroutine(MiscUtils.DownloadImage(curInfo.playerBaseInfo.icon, spr =>
                {
                    headIcon.sprite = spr;
                }));
            }
        }
    }
}
