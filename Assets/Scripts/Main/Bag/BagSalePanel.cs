using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 使用或出售
/// </summary>
public class BagSalePanel : MonoBehaviour
{
    public Text showItem, goodsName, cost, allCost, haveNum;
    public InputField input;
    public Button addBtn, cutBtn, MaxBtn, trueBtn;
    public Image closeBg;
    private int useNum = 1;
    BagGoodsItem _Item;
    void Start()
    {
        UGUIEventListener.Get(closeBg.gameObject).onClick = (g) => { Close(); };
        UGUIEventListener.Get(addBtn.gameObject).onClick = (g) => { useNum++; AddOrCut(); };
        UGUIEventListener.Get(cutBtn.gameObject).onClick = (g) => { useNum--; AddOrCut(); };
        UGUIEventListener.Get(MaxBtn.gameObject).onClick = (g) => { AddOrCut(true); };
        UGUIEventListener.Get(trueBtn.gameObject).onClick = (g) => { OnClickTrue(); };
    }
    /// <summary>
    /// 显示 0是使用 1是出售
    /// </summary>
    public void ShowPanel(BagGoodsItem item)
    {
        _Item = item;
        gameObject.SetActive(true);
        input.text = useNum.ToString();
        showItem.text = _Item._data.name;
        haveNum.text = string.Format("拥有数量：" + _Item._data.counts);
        goodsName.text = _Item._data.goodsType;
        cost.text = _Item._data.sale_price.ToString();
        allCost.text = (_Item._data.sale_price * useNum).ToString();

    }
    /// <summary>
    ///  0是加 1是减
    /// </summary>
    public void AddOrCut(bool max = false)
    {
        if (useNum > _Item._data.counts)
            useNum = _Item._data.counts;
        if (useNum < 1)
            useNum = 1;
        if (max)
            useNum = _Item._data.counts;
        input.text = useNum.ToString();
        allCost.text = (_Item._data.sale_price * useNum).ToString();
    }
    public void OnClickTrue()
    {
        if (int.Parse(input.text) > _Item._data.counts)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "出售数量不足", 0.5f);
            return;
        }
        else
        {
            SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
            {
                SaleUserGoodsReq = new net_protocol.SaleUserGoodsReq()
                {
                    counts = int.Parse(input.text),
                    goods_id = _Item._data.id,
                    type = _Item._data.type,
                    totalMoney = _Item._data.sale_price * useNum,
                    name = _Item._data.name
                },
                msgid = net_protocol.MessageId.C2G_SaleUserGoods
            });

        }

    }
    public void OnClickTrueFinish(int resp)
    {
        if (resp == 1)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "出售成功", 1f);
            _Item._data.counts -= int.Parse(input.text);
            _Item.UpGoods(_Item._data);
            Close();
        }
    }

    private void Close()
    {
        useNum = 1;
        gameObject.SetActive(false);
    }
}
