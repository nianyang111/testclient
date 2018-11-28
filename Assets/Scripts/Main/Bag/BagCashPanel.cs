using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagCashPanel : MonoBehaviour
{
    public Text title;
    public GameObject selectPanel, goodsPanel, closeBtn;
    public BagCashItem[] cashArray;
    public BagGoodsPanel panel;
    private BagCashItem curItem;
    public BagCashItem CurItem
    {
        set
        {
            curItem = value;
            goodsIcon.sprite = curItem.cashIcon.sprite;
            title.text = curItem.cashName;
            goodsPanel.SetActive(true);
            CreateItem(panel.dataList);
        }
        get { return curItem; }
    }
    public void Init()
    {
        UGUIEventListener.Get(trueBtn.gameObject).onClick = delegate { OnClickTrue(); };
        UGUIEventListener.Get(closeBtn).onClick = delegate { gameObject.SetActive(false); };
    }
    public Image goodsIcon;
    public BagUseItem prefab;
    public Transform content;
    public Button trueBtn;
    List<BagUseItem> itemList = new List<BagUseItem>();
    BagGoodsItem _item;
    public void OpenPanel(BagGoodsItem item)
    {
        _item = item;
        gameObject.SetActive(true);
        title.text = "充值卡选择";
        selectPanel.SetActive(true);
        goodsPanel.SetActive(false);
    }
    void CreateItem(List<BagGoodsData> list)
    {
        var dataList = list.FindAll(p => p.type == _item._data.type);
        for (int i = 0; i < dataList.Count; i++)
        {
            BagUseItem item = Instantiate(prefab, content);
            item.Init(dataList[i]);
            itemList.Add(item);
        }
    }

    void OnClickTrue()
    {
        int sum = 0;
        for (int i = 0; i < itemList.Count; i++)
        {
            sum += (itemList[i].useNum * itemList[i]._data.price);
        }
        if (CurItem.cashNum != sum)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, string.Format("使用的金额和选择的{0}话费卷金额不一致，无法兑换", CurItem.cashNum));
            return;
        }

        // 0是使用 1是出售
        net_protocol.UseGoodsReq Requ = new net_protocol.UseGoodsReq();
        Requ.tyep = _item._data.type;
        for (int i = 0; i < itemList.Count; i++)
        {
            net_protocol.UseGoodsReq.Goods goods = new net_protocol.UseGoodsReq.Goods();
            goods.counts = itemList[i].useNum;
            goods.goods_id = itemList[i]._data.id;
            goods.name = itemList[i]._data.name;
            goods.price=itemList[i]._data.price;
            goods.type = itemList[i]._data.type;
            Requ.goods.Add(goods);
        }
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_UseGoods,
            UseGoodsReq = Requ
        });

    }
    void OnDisable()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            Destroy(itemList[i].gameObject);
        }
        itemList.Clear();
    }

    public void OnClickTrueFinish(int resp)
    {
        if (resp == 1)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "兑换成功，稍后会发放充值卡到“兑换记录”中，请耐心等待", 1f);
            
            for (int i = 0; i < itemList.Count; i++)
            {
                var item= panel.itemList.Find(p => p._data.id == itemList[i]._data.id);
                item._data.counts -= itemList[i].useNum;
                item.UpGoods(item._data);
            }
            Close();
        }
        else
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "兑换失败，关闭物品后使用", 1f);
        }
    }
    private void Close()
    {
        gameObject.SetActive(false);
    }
}
