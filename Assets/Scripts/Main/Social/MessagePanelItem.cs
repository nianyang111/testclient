using net_protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MessagePanelItem : MonoBehaviour {

    public Image headIcon;
    public Text nameLb;
    public Image sixIcon;
    public Text textLb;
    public Text timeLb;
    public GameObject refuseBtn;
    public GameObject agreeBtn;
    public Button btn;
    public MessagePanel.MessagePanelInfo info;

    public void Inits(MessagePanel.MessagePanelInfo info, CallBack<FriendInfo> click)
    {
        this.info = info;
        if (info.messageType == 0)
        {//玩家
            if (gameObject.activeInHierarchy)
                StartCoroutine(MiscUtils.DownloadImage(info.headIcon, spr =>
                      {
                          headIcon.sprite = spr;
                      }));
            RefreshNowMessage(info.type, info.text, info.timer);
            nameLb.text = info.name;            
            sixIcon.sprite = BundleManager.Instance.GetSprite(info.six == 0 ? "friend/haoyou_pic_nan" : "friend/haoyou_pic_nv");
            sixIcon.SetNativeSize();
            btn.onClick.AddListener(() =>
                click(new FriendInfo()
                {
                    photo = info.headIcon,
                    nickname = info.name,
                    userId = info.id,
                    gender = info.six,
                }));
        }
        else
        {//系统
            UGUIEventListener.Get(refuseBtn).onClick = delegate
            {
                SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
                {
                    replyFriendReq = new ReplyFriendReq()
                    {
                        userId = info.id,
                        reply = -1
                    },
                    msgid = MessageId.C2G_ReplyFriendReq
                });
                Destroy(gameObject);
            };
            UGUIEventListener.Get(agreeBtn).onClick = delegate
            {
                SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
                {
                    replyFriendReq = new ReplyFriendReq()
                    {
                        userId = info.id,
                        reply = 1
                    },
                    msgid = MessageId.C2G_ReplyFriendReq
                });
                Destroy(gameObject);
            };
            textLb.text = info.name + "[" + info.id + "]" + "请求加您为好友";
        }
    }    

    /// <summary>
    /// 刷新最新消息
    /// </summary>
    public void RefreshNowMessage(int type, string message, long time)
    {
        DateTime dateTime = MiscUtils.GetDateTimeByTimeStamp(time / 1000);
        if (type == 1)
        {
            textLb.text = "(语音消息)";
            timeLb.text = dateTime.Hour.ToString("D2") + ":" + dateTime.Minute.ToString("D2");
            return;
        }
        textLb.text = TextHide(message);
        timeLb.text = dateTime.Hour.ToString("D2") + ":" + dateTime.Minute.ToString("D2");
    }

    /// <summary>
    /// 隐藏部分文本
    /// </summary>
    string TextHide(string sourcesText)
    {
        string str = sourcesText;
        str = str.Length > 6 ? str.Substring(0, 6) + "..." : str;
        return str;
    }

    void OnEnable()
    {
        if (info != null && info.messageType == 0)
        {
            if (headIcon.sprite == null)
            {
                StartCoroutine(MiscUtils.DownloadImage(info.headIcon, spr =>
                {
                    headIcon.sprite = spr;
                }));
            }
            else if (headIcon.sprite.name != "")
            {
                StartCoroutine(MiscUtils.DownloadImage(info.headIcon, spr =>
                {
                    headIcon.sprite = spr;
                }));
            }
        }

    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
