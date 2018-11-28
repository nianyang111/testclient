using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchHistoryRecordPanel : MonoBehaviour
{
    public MatchHistoryRecordItem prefab;
    public Transform group, show;
    public List<MatchHistoryRecordItem> itemList = new List<MatchHistoryRecordItem>();

    private MatchHistoryRecordItem curItem;
    public MatchHistoryRecordItem CurItem
    {
        get { return curItem; }
        set
        {
            if (curItem != null)
                curItem.OnSelect();
            if (curItem != value)
            {
                curItem = value;
                curItem.Select();
            }
            else
            {
                curItem.OnSelect();
                curItem = null;
            }
        }
    }
    public void Init()
    {

    }
    public void Open()
    {
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_QueryMatcherHistory,
        });

    }

    public void CreateItem()
    {
        var dataList = MatchModel.Instance.histoyList;
        for (int i = 0; i < dataList.Count; i++)
        {
            MatchHistoryRecordItem item = Instantiate(prefab, group);
            item.panel = this;
            item.Init(dataList[i]);
            itemList.Add(item);
        }
    }
    public void Close()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            Destroy(itemList[i].gameObject);
        }
        itemList.Clear();
    }
}
