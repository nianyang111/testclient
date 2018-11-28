using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsEnterNode : Node
{
    QueryTableInfoResp info;

    public Text titleLb;

    public Text roomerLb;
    public Text roomId;
    public Text gameTypeLb;
    public Text dizhuLb;
    public Text jushuLb;
    public Text maxBeishuLb;

    public GameObject goBtn;
    public GameObject refuseBtn;

    public void Inits(QueryTableInfoResp info)
    {
        this.info = info;

        if (info.isInvite)
        {//是别人邀请
            goBtn.transform.localPosition = new Vector3(-196.5f, -285.6f);
            refuseBtn.SetActive(true);
            UGUIEventListener.Get(refuseBtn).onClick = delegate { Refuse(); };
            titleLb.text = "你收到来自: " + info.hostName + " 的邀请,确认进入以下约牌房?";
        }
        else
        {//自己输入房号
            goBtn.transform.localPosition = new Vector3(0, -285.6f);
            refuseBtn.SetActive(false);
            titleLb.text = "确认进入以下约牌房?";
        }

        roomerLb.text = info.hostName;
        roomId.text = info.roomId;

        gameTypeLb.text = info.gameName;
        dizhuLb.text = info.baseScore + "积分";
        jushuLb.text = info.round.ToString();
        maxBeishuLb.text = info.maxRate.ToString();

        UGUIEventListener.Get(goBtn).onClick = delegate { EnterRoom(); };
    }

    void EnterRoom()
    {
        if (info.gameName == "斗地主")
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                enterDdzRoomReq = new EnterDdzRoomReq()
                    {
                        tno = int.Parse(info.roomId),
                        roomId = 7
                    },
                msgid = MessageId.C2G_EnterDdzRoomReq
            }, true);
        }
        else if (info.gameName == "麻将")
        {
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_UserJoinTable,
                UserJoinTable = new UserJoinTable()
                {
                    tableId = info.roomId
                }
            }, true);
        }
    }

    /// <summary>
    /// 拒绝邀请
    /// </summary>
    void Refuse()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                rejectFriendYuePai = new RejectFriendYuePai()
                {
                    userId = info.userId
                },
                msgid = MessageId.C2G_RejectFriendYuePai
            });
        Close();
    }
}
