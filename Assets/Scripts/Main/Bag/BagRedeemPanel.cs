using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagRedeemPanel : MonoBehaviour
{
    public Transform content;
    public BagRedeemItem prefab;
    public BagSeePanel seePanel;
    List<BagRedeemData> dataList = new List<BagRedeemData>();
    List<BagRedeemItem> itemList = new List<BagRedeemItem>();
    BagNode _node;
    public void Init(BagNode node)
    {
        _node = node;
        seePanel.Init();
    }

    public void Open()
    {
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage() { msgid = net_protocol.MessageId.C2G_QueryTickRecord },true);
        _node.hintPanel.SetActive(dataList.Count < 1);
        _node.hintText.text = "您还没有兑奖记录哦~";
    }
    public void QueryTickFinish(net_protocol.QueryTickRecordResp resp)
    {
        var tickList = resp.tickRecord;
        for (int i = 0; i < tickList.Count; i++)
        {
            BagRedeemData data=new BagRedeemData();
            var mType = _node.iconList.Find(p => p.id == tickList[i].goods_id);
            if(mType!=null)
            data.goodsType = mType.goodsType;
            data.account = tickList[i].account;
            data.date = tickList[i].created;
            data.id = tickList[i].id;
            data.name = tickList[i].name;
            data.pwd = tickList[i].pwd;
            data.state = tickList[i].status;
            data.userId = tickList[i].userId;
            dataList.Add(data);
        }
        CreateRedeem();
    }
    public void CreateRedeem()
    {
        _node.hintPanel.SetActive(dataList.Count < 1);
        for (int i = 0; i < dataList.Count; i++)
        {
            BagRedeemItem item = Instantiate(prefab, content);
            item._panel = this;
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
        dataList.Clear();
    }


}
