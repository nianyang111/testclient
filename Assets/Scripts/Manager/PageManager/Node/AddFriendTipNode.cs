using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendTipNode : Node {

    public Text nameLb;
    public Image headIcon;
    public Timer timer;
    public GameObject agreeBtn;
    public GameObject refuseBtn;
    public FriendInfo curInfo;


    public void Inits(FriendInfo info)
    {
        curInfo = info;
        StartCoroutine(MiscUtils.DownloadImage(info.photo, spr =>
              {
                  headIcon.sprite = spr;
              }));
        nameLb.text = info.nickname;
        timer.allLength = 10;
        timer.isOnce = true;
        timer.endAction = () => Close(false);
        timer.StartTime();
        UGUIEventListener.Get(agreeBtn).onClick = delegate { AgreeAddFriend(true); };
        UGUIEventListener.Get(refuseBtn).onClick = delegate { AgreeAddFriend(false); };
    }

    void OnEnable()
    {
        //timer.StartTime();
    }

    /// <summary>
    /// 同意加好友按钮回调
    /// </summary>
    void AgreeAddFriend(bool isAgree)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                replyFriendReq = new ReplyFriendReq()
                {
                    userId = curInfo.userId,
                    reply = isAgree ? 1 : -1
                },
                msgid = MessageId.C2G_ReplyFriendReq
            });
        Close(false);
    }
}
