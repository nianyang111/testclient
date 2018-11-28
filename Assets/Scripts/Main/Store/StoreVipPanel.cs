using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreVipPanel : MonoBehaviour {
    public StoreVipItem prefab;
    public Transform content;
    public StoreVipRechargePanel rechargePanel;
    public List<StoreVipData> dataList = new List<StoreVipData>();
    public List<StoreVipItem> itemList = new List<StoreVipItem>();

    public void Init()
    {
        rechargePanel.Init();
        JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.storeVipConfig));
        for (int i = 0; i < jd.Count; i++)
        {
            StoreVipData data = JsonMapper.ToObject<StoreVipData>(JsonMapper.ToJson(jd[i]));
            data.goldNum = data.rmbNum * 100;
            data.itemName = string.Format(data.itemName + " " + (data.rmbNum == 10 ? "周卡" : "月卡"));
            data.itemHint = string.Format("购买立刻享受" + data.vipDay + "天VIP特权");
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
            StoreVipItem item = Instantiate(prefab, content);
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
