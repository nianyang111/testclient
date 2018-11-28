using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreAgPanel : MonoBehaviour {
    public StoreAgItem prefab;
    public Transform content;
    public StoreAgRechargePanel rechargePanel;
    public List<StoreAgData> dataList = new List<StoreAgData>();
    public List<StoreAgItem> itemList = new List<StoreAgItem>();
    public void Init()
    {
        rechargePanel.Init();
        //读配置表
        JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.storeAgConfig));
        for (int i = 0; i < jd.Count; i++)
        {
            StoreAgData data = JsonMapper.ToObject<StoreAgData>(JsonMapper.ToJson(jd[i]));
            data.replaceNum = data.itemNum / data.ratioNum;
            data.rmbNum = data.replaceNum / data.ratioNum;
            data.itemName = string.Format(data.itemNum.ToString("#,##0") + "银币");
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
            StoreAgItem item = Instantiate(prefab, content);
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
