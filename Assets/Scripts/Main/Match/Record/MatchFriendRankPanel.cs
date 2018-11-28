using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchFriendRankPanel : MonoBehaviour {
    public MatchFriendRankItem prefab;
    public Transform content;
    public Text contentText;
    public Button weChatBtn;
    public MatchFriendNotHavePanel notHaveFriendPanel;
    public Transform show;
    private List<MatchFriendRankItem> itemList = new List<MatchFriendRankItem>();

    private MatchFriendRankItem curItem;
    public MatchFriendRankItem CurItem
    {
        get { return curItem; }
        set
        {
            if (curItem != null)
                curItem.OnSelect();
            if (curItem != value)
            {
                curItem = value;
                curItem.Select();
            }
            else
            {
                curItem.OnSelect();
                curItem = null;
            }
        }
    }

    public void Init()
    {
        
    }

    public void Open()
    {
        notHaveFriendPanel.gameObject.SetActive(itemList.Count < 1);
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage() {
         msgid = net_protocol.MessageId.C2G_UserMatcherFriendRank
        });
    }
    public void CreateItem()
    {
        var dataList = MatchModel.Instance.friendList;
        notHaveFriendPanel.gameObject.SetActive(dataList.Count < 1);
        for (int i = 0; i < dataList.Count; i++)
        {
            var item= Instantiate(prefab, content);
            item.panel = this;
            item.Init(dataList[i]);
            itemList.Add(item);
        }
    }
    public void FlushData()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].FlushData();
        }
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
