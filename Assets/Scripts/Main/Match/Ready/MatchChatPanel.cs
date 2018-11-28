using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchChatPanel : MonoBehaviour {
    public Transform content,show;
    public MatchChatItem prefab;
    public InputField input;
    public Button sendBtn;
    MatchReadyNode _node;
    List<MatchChatItem> itemList = new List<MatchChatItem>();
    MatchChatItem curItem;
    public MatchChatItem CurItem
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
    public void Init(MatchReadyNode node)
    {
        _node = node;
        UGUIEventListener.Get(sendBtn.gameObject).onClick = delegate { SendMessages(string.Empty); };
        UGUIEventListener.Get(show.gameObject).onClick = delegate { CurItem = CurItem; };
    }
    public void SendMessage()
    {
        string str = "进入房间";
        SendMessages(str);
    }
    private void SendMessages(string sendStr)
    {
        string message = "";
        if (!string.IsNullOrEmpty(sendStr))
            message = sendStr;
        else
            message = input.text;
        if (message.Replace(" ", "") == "")
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "输入内容不能为空", 1f);
            return;
        }
        if (message.Length > 20)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "超过20个字数", 1f);
            return;
        }
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage() { 
          msgid=     net_protocol.MessageId.C2G_MatcherChat,
          matcherChat = new net_protocol.MatcherChat()
          {
              content=message,
              matcherId=_node.GetData().matchId,
              userId=UserInfoModel.userInfo.userId
          }
        });
        input.text = "";
    }
    public void AnalysisMessage(net_protocol.MatcherChatResp resp)
    {
        var item = Instantiate(prefab, content);
        item.panel = this;
        item.Init(resp);
        itemList.Add(item);
        StartCoroutine(SetTransform());
    }
    IEnumerator SetTransform()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        RectTransform rect = (RectTransform)content;
        RectTransform rectParent = ((RectTransform)content.parent);
        float size = rect.sizeDelta.y;
        if (size > rectParent.rect.size.y)
        ((RectTransform)content).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, size);
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
