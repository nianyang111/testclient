using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreCardPanel : MonoBehaviour {
    public StoreCardItem prefab;
    public Transform content;
    public StoreCardRechargePanel rechargePanel;
    public List<StoreCardData> dataList = new List<StoreCardData>();
    public List<StoreCardItem> itemList = new List<StoreCardItem>();

    public void Init()
    {
        rechargePanel.Init();
        //临时读取
        JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.storeCardConfig));
        for (int i = 0; i < jd.Count; i++)
        {
            StoreCardData data = JsonMapper.ToObject<StoreCardData>(JsonMapper.ToJson(jd[i]));
            data.itemName = string.Format(data.cardNum+ "张房卡");
            dataList.Add(data);
        }
    }
    public void Open()
    {
        CloseRechargePanel();
        if (itemList.Count > 0)
            return;
        for (int i = 0; i < dataList.Count; i++)
        {
            StoreCardItem item = Instantiate(prefab, content);
            item._panel = this;
            item.Init(dataList[i]);
            itemList.Add(item);
        }
    }
    public void CloseRechargePanel()
    {
        rechargePanel.gameObject.SetActive(false);
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
