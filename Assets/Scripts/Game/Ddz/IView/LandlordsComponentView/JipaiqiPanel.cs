using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JipaiqiPanel : MonoBehaviour
{

    public List<JipaiqiItem> items = new List<JipaiqiItem>();    

    public void ReqJipaiqi()
    {
        if (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType != RoomType.RoomCard)
        {
            //请求记牌器
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_DdzQueryPokerReq
            });
        }
    }

    public void InitValue(List<DdzPokerCounter> ddzPokerCounter)
    {
        for (int i = 0; i < ddzPokerCounter.Count; i++)
        {
            Weight weight = Card.GetWeightByPoker(ddzPokerCounter[i].ds);
            int count = ddzPokerCounter[i].zs;
            JipaiqiItem item = items.Find(p => p.weight == weight);
            item.SetValue(count);
        }
        gameObject.SetActive(true);
    }

    public void SetValue(Weight weight)
    {
        JipaiqiItem item = items.Find(p => p.weight == weight);
        item.SetValue(item.curNum - 1);
    }

    public void SetValue(List<Weight> weights)
    {
        for (int i = 0; i < weights.Count; i++)
        {
            SetValue(weights[i]);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetValue(0);
        }
        gameObject.SetActive(false);
    }
}
