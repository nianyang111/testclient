using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagGoodsPanel : MonoBehaviour
{
    public Transform content;
    public BagGoodsItem prefab;
    public BagSalePanel salePanel;
    public BagCashPanel cashPanel;
    public List<BagGoodsData> dataList = new List<BagGoodsData>();
    public List<BagGoodsItem> itemList = new List<BagGoodsItem>();
    BagNode _node;
    
    public void Init(BagNode node)
    {
        _node = node;
        cashPanel.Init();
        salePanel.gameObject.SetActive(false);
        cashPanel.gameObject.SetActive(false);
    }

    public void Open()
    {
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage(){ msgid = net_protocol.MessageId.C2G_QueryGoods},true);
        _node.hintPanel.SetActive(dataList.Count < 1);
        _node.hintText.text = "您暂时还没有物品";
    }
    public void SetData(List<net_protocol.QueryGoodsResp.Goods> resp )
    {
        for (int i = 0; i < resp.Count; i++)
        {
            BagGoodsData data=new BagGoodsData();
            data.price = resp[i].price;
            data.counts = resp[i].counts;
            data.type = resp[i].type;
            data.id = resp[i].goods_id;
            data.sale_price = resp[i].sale_price;
            data.name = _node.iconList.Find(p => p.id == resp[i].goods_id).goodsName;
            data.icon = _node.iconList.Find(p => p.id == resp[i].goods_id).goodsIcon;
            data.goodsType = _node.iconList.Find(p => p.id == resp[i].goods_id).goodsType;
            dataList.Add(data);
        }
        CreateGoods();
    }
    private void CreateGoods()
    {
        _node.hintPanel.SetActive(dataList.Count < 1);
        for (int i = 0; i < dataList.Count; i++)
        {
            BagGoodsItem item = Instantiate(prefab, content);
            item._panle = this;
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
public class BagIconData
{
    public string goodsIcon;
    public string goodsName;
    public string goodsType;
    public int id;
}