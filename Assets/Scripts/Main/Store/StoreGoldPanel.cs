using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreGoldPanel : MonoBehaviour
{
    public Transform content;
    public StoreGoldItem prefab;
    public StoreGoldRechargePanel rechargePanel;
    public List<StoreGoldData> dataList = new List<StoreGoldData>();
    public List<StoreGoldItem> itemList = new List<StoreGoldItem>();
    public void Init()
    {
        rechargePanel.Init();
        //读配置表
        JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.storeGoldConfig));
        for (int i = 0; i < jd.Count; i++)
        {
            StoreGoldData data = JsonMapper.ToObject<StoreGoldData>(JsonMapper.ToJson(jd[i]));
            data.rmbNum = data.itemNum / data.ratioNum;
            data.replaceNum = data.itemNum * data.ratioNum;
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
            StoreGoldItem item= Instantiate(prefab, content);
            item._panel = this;
            item.Init(dataList[i]);
            itemList.Add(item);
        }
    }
    /// <summary>
    /// 关闭购买
    /// </summary>
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
