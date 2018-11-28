using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchJoinNode :Node {
    public MatchJoinItem prefab;
    public Transform content;

    List<MatchJoinItem> itemList = new List<MatchJoinItem>();
    public override void Init()
    {
        base.Init();
    }

    public void OpenPanel(List<MatchJoinData> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            var item = Instantiate(prefab, content);
            item.Init(dataList[i]);
            itemList.Add(item);
        }
    }
    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
        for (int i = 0; i < itemList.Count; i++)
        {
            Destroy(itemList[i].gameObject);
        }
        itemList.Clear();
    }
}
