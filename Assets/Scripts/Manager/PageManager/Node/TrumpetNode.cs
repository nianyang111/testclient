using net_protocol;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TrumpetNode : Node {

    public static TrumpetNode instance;

    public InputField input;
    public GameObject sendBtn;
    public Transform parent;
    public GameObject prefab;

    void Awake()
    {
        instance = this;
    }

    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(sendBtn).onClick = Send;
        LoadLabaList();
    }


    public void LoadLabaList()
    {
        for (int i = 0; i < ChatModel.chatList.Count; i++)
        {
            LoadItem(ChatModel.chatList[i]);   
        }        
    }

    /// <summary>
    /// 发送喇叭
    /// </summary>
    /// <param name="go"></param>
    void Send(GameObject go)
    {
        if (UserInfoModel.userInfo.walletAgNum < 20000)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "银币不足20000,发送失败");
            return;
        }
        if (string.IsNullOrEmpty(input.text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "发送内容不能为空!");
            return;
        }
        if (Encoding.UTF8.GetByteCount(input.text) > 28 * 3)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "发送内容超过字符限制!");
            return;
        }
        if (BlockWordModel.CheckIsBlock(input.text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "消息中含有敏感词汇!");
            return;
        }
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            sendHornMsg = new SendHornMsg()
            {
                content = input.text
            },
            msgid = MessageId.C2G_SendHornMsg
        });
        input.text = string.Empty;
        Close();
    }

    public void LoadItem(GameMessage message)
    {
        TrumpetItem item = Instantiate(prefab, parent).GetComponent<TrumpetItem>();
        item.Init(message);
        item.transform.SetAsFirstSibling();
        if (parent.childCount > ChatModel.MaxChatCount)
        {
            Destroy(parent.GetChild(parent.childCount - 1).gameObject);
        }
    }

    /// <summary>
    /// 服务器发来的新的聊天消息
    /// </summary>
    /// <param name="chat"></param>
    public static void G2C_AddMessage(GameMessage info)
    {
        ChatModel.chatList.Add(info);
        ChatModel.chatList[0] = info;
        if (ChatModel.chatList.Count > ChatModel.MaxChatCount)
        {
            ChatModel.chatList.RemoveAt(ChatModel.chatList.Count - 1);
        }

        string message = string.Format("{0}:{1}", info.sender, info.value);
        TrumpetNode node = NodeManager.GetNode<TrumpetNode>();
        if (node)
            node.LoadItem(info);
        NoticeNode.Add(message);
        //NodeManager.OpenNode<NoticeNode>(null, null, false, false);
    }

    public override void Close(bool isOpenLast = true)
    {
        base.Close(isOpenLast);
        instance = null;
    }

   
}


/// <summary>
/// 游戏喇叭/公告
/// </summary>
public class GameMessage
{
    /// <summary>
    /// 0系统1玩家
    /// </summary>
    public int type;
    /// <summary>
    /// 发送者
    /// </summary>
    public string sender;
    /// <summary>
    /// 发送内容
    /// </summary>
    public string value;
}

/// <summary>
/// 聊天数据层
/// </summary>
public class ChatModel
{
    static List<GameMessage> infos;
    static int maxChatCount = 30;

    /// <summary>
    /// 得到聊天消息列表
    /// </summary>
    public static List<GameMessage> chatList
    {
        get
        {
            if (infos == null)
                infos = new List<GameMessage>();
            return infos;
        }
    }

    /// <summary>
    /// 显示的最大聊天数量
    /// </summary>
    public static int MaxChatCount
    {
        get { return maxChatCount; }
    }

    /// <summary>
    /// 清理聊天数据
    /// </summary>
    public static void Clear()
    {
        chatList.Clear();
    }
}
