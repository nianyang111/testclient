using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDdzPanel : MonoBehaviour {
    public SelectItem prefab;
    public Transform content;
    public Tab agBtn, goldBtn;
    public List<SelectData> dataList = new List<SelectData>();
    public List<SelectItem> itemList = new List<SelectItem>();
    private string[] roomType = new string[2] { "银币场", "金条场" };
    /// <summary> 当前游戏币类型 </summary>
    public string ddzType;
    public void Init()
    {
        UGUIEventListener.Get(agBtn.gameObject).onClick = (g) => { if (ddzType == roomType[0])return; ddzType = roomType[0]; SelectType(); };
        UGUIEventListener.Get(goldBtn.gameObject).onClick = (g) => { if (ddzType == roomType[1])return; ddzType = roomType[1]; SelectType(); };
        JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.ddzRoomGradeConfig));
        for (int i = 0; i < jd.Count; i++)
        {
            SelectData data = JsonMapper.ToObject<SelectData>(JsonMapper.ToJson(jd[i]));
            dataList.Add(data);
        }
        ddzType = roomType[0];
        SelectType();
    }
    void OnEnable()
    {
        //1是斗地主 2是麻将
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_QueryGameRoomReq,
            queryGameRoomReq = new net_protocol.QueryGameRoomReq()
            {
                gid = 1
            }
        },true);
    }
    public void SelectType()
    {
        var list = dataList.FindAll(p => p.roomType == ddzType);
        FlushItemList(list);
    }

    void FlushItemList(List<SelectData> list)
    {
        if (itemList.Count < list.Count)
        {
            for (int i = 0; i < list.Count; i++)
            {
                SelectItem item = Instantiate(prefab, content);
                item.gameObject.SetActive(true);
                item.Init(list.Find(p => p.roomGradeTypeId == i + 1));
                itemList.Add(item);
            }
        }
        else
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < list.Count; i++)
            {
                itemList[i].FlushData(list.Find(p => p.roomGradeTypeId == i + 1));
                itemList[i].gameObject.SetActive(true);
            }
        }
    }
}
