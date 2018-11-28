using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NearPanel : MonoBehaviour {

    public GameObject findObj;
    public GameObject goBtn;

    public GameObject showObj;
    public GameObject clearPosBtn;
    public Transform parent;
    public GameObject Prefab;

    [HideInInspector]
    public List<FriendInfo> nearInfos = new List<FriendInfo>();
    void Start()
    {
        UGUIEventListener.Get(clearPosBtn).onClick = delegate { ClearPos(); };
        UGUIEventListener.Get(goBtn).onClick = delegate { Go(); };        

        findObj.SetActive(true);
        showObj.SetActive(false); 
    }


    /// <summary>
    /// 看看哪些人在玩按钮回调
    /// </summary>
    void Go()
    {
        findObj.SetActive(false);
        showObj.SetActive(true);
        StartCoroutine(LoadGPS());
    }

    /// <summary>
    /// 定位并请求附近玩家消息
    /// </summary>
    IEnumerator LoadGPS()
    {
        StartCoroutine(MiscUtils.StartGPS());
        yield return MiscUtils.StartGPS();
        float[] pos = MiscUtils.GetLocation();
        if (pos[0] == pos[1] && pos[1] == 0)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "分享位置失败");
            findObj.SetActive(true);
            showObj.SetActive(false);
            yield break;
        }
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                sharePositionReq = new SharePositionReq()
                {
                    lat = pos[1],
                    lng = pos[0],
                },
                msgid = MessageId.C2G_SharePositionReq
            });        
    }



    /// <summary>
    /// 服务器响应附近的人列表
    /// </summary>
    public static void G2C_Near(List<FriendInfo> nearInfos)
    {
        SocialNode node = NodeManager.GetNode<SocialNode>();
        if (node)
        {
            node.nearPanel.nearInfos = nearInfos;
            node.nearPanel.LoadItems(nearInfos);
        }
    }

    void LoadItems(List<FriendInfo> infos)
    {
        for (int i = 0; i < infos.Count; i++)
        {
            GameObject go = Instantiate(Prefab, parent);
            go.GetComponent<NearItem>().Init(infos[i], (info) => SocialModel.Instance.AddFriend(info.userId));
        }
    }

    /// <summary>
    /// 清除地理位置按钮回调
    /// </summary>
    void ClearPos()
    {
        TipManager.Instance.OpenTip(TipType.ChooseTip, "确认清楚地理位置信息?清除位置信息后,其他玩家将不能找到你、跟你起玩哦~", 10, () =>
            {
                SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
                    {                        
                        msgid=MessageId.C2G_ClearPositionReq
                    });
                UIUtils.DestroyChildren(parent);
                nearInfos.Clear();
                findObj.SetActive(true);
                showObj.SetActive(false);
            });
    }   
}


