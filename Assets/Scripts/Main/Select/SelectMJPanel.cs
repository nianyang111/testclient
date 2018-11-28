using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMJPanel : MonoBehaviour
{
    public SelectItem prefab;
    public Transform content;
    public Tab agTwoBtn, goldTwoBtn, agFourBtn, goldFourBtn;
    public List<SelectData> dataList = new List<SelectData>();
    public List<SelectItem> itemList = new List<SelectItem>();
    private string[] roomType = new string[4] { "二人银币场", "二人金条场", "四人银币场", "四人金条场" };
    /// <summary> 当前游戏类型 </summary>
    public string mjType;
    public void Init()
    {
        UGUIEventListener.Get(agTwoBtn.gameObject).onClick = (g) => { mjType = roomType[0]; SelectType(); };
        UGUIEventListener.Get(goldTwoBtn.gameObject).onClick = (g) => { mjType = roomType[1]; SelectType(); };
        UGUIEventListener.Get(agFourBtn.gameObject).onClick = (g) => { mjType = roomType[2]; SelectType(); };
        UGUIEventListener.Get(goldFourBtn.gameObject).onClick = (g) => { mjType = roomType[3]; SelectType(); };
        JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.mjRoomGradeConfig));
        for (int i = 0; i < jd.Count; i++)
        {
            SelectData data = JsonMapper.ToObject<SelectData>(JsonMapper.ToJson(jd[i]));
            dataList.Add(data);
        }
        mjType = roomType[0];
        SelectType();
    }
    void OnEnable()
    {
        //1是斗地主 2是麻将
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage() {
         msgid=  net_protocol.MessageId.C2G_QueryGameRoomReq,
         queryGameRoomReq = new net_protocol.QueryGameRoomReq()
         {
             gid=2
         }
        },true);
    }
    public void SelectType()
    {
        var _agList = dataList.FindAll(p => p.roomType == mjType);
        FlushItemList(_agList);
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

    void OnDesable()
    {

    }
}
