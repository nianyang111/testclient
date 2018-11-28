using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 物品箱物品信息
/// </summary>
public class BagGoodsItem : MonoBehaviour {
    public Text goodsName;
    public Image goodsIcon;
    public Text counts;
    public Button useBtn, saleBtn;
    public BagGoodsPanel  _panle;
    public BagGoodsData _data;
    public void Init(BagGoodsData data)
    {
        UGUIEventListener.Get(useBtn.gameObject).onClick = (g) =>
        {
            _panle.cashPanel.OpenPanel(this);
        };
        UGUIEventListener.Get(saleBtn.gameObject).onClick = (g) => 
        {
            _panle.salePanel.ShowPanel(this);
        };
        goodsName.text = data.name;
        if (data.icon != null)
        {
            goodsIcon.sprite = BundleManager.Instance.GetSprite(data.icon);
            goodsIcon.SetNativeSize();
            goodsName.gameObject.SetActive(false);
        }
        UpGoods(data);
    }
    public void UpGoods(BagGoodsData data)
    {
        _data = data;
        counts.text = _data.counts.ToString();
        if (_data.counts <= 0)
        {
            gameObject.SetActive(false);
            _panle.dataList.Remove(_data);
        }
    }
}
public class BagGoodsData{
    public int id;
    public int price;
    public int counts;
    /// <summary>1是电话费 </summary>
    public int type;
    public string icon;
    public string name;
    public string goodsType;
    public int sale_price;
}