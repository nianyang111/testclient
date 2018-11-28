using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BqPanel : MonoBehaviour {

    public List<BqItem> items = new List<BqItem>();
    public Transform normalParent;

    public Transform vipParent;
    public GameObject noVip;
    public GameObject becomVipBtn;

    public GameObject root_vip;

    public GameObject isVip;
    public Text vipTimerLb;
    public GameObject xufeiBtn;

    void Start()
    {
        if (UserInfoModel.userInfo.vipCard != 0)
        {
            noVip.SetActive(false);
            isVip.SetActive(true);
        }
        else
        {
            noVip.SetActive(true);
            isVip.SetActive(false);
        }

        UGUIEventListener.Get(becomVipBtn).onClick = delegate
        {
            NodeManager.OpenNode<RechargeVIPNode>();
        };

        UGUIEventListener.Get(becomVipBtn).onClick = delegate
        {
            NodeManager.OpenNode<StoreNode>().vipBtn.isOn = true;
        };
        UGUIEventListener.Get(xufeiBtn).onClick = delegate
        {
            NodeManager.OpenNode<StoreNode>().vipBtn.isOn = true;
        };
        
        items.Clear();
        items.AddRange(normalParent.GetComponentsInChildren<BqItem>());
        items.AddRange(vipParent.GetComponentsInChildren<BqItem>());
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Init(SendChatMessage);
        }
    }

    void OnEnable()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_IsVipUserReq,
        });
    }

    void SendChatMessage(string bqId)
    {
        ChatInfo info = new ChatInfo();
        info.text = bqId;
        info.type = 2;
        NodeManager.GetNode<ChatNode>().SendMessage(info, true);        
    }

    /// <summary>
    /// Vip时间更新
    /// </summary>
    public static void FinishVipDay(IsVipUserResp isVipUserResp)
    {
        ChatNode node = NodeManager.GetNode<ChatNode>();
        if (node)
        {
            BqPanel bq = node.bqPanel;
            UserInfoModel.userInfo.vipDay = isVipUserResp.TimeLeft;
            bq.vipTimerLb.text = "剩余" + UserInfoModel.userInfo.vipDay;
        }
    }
}
