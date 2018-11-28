using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatNode : Node {

    public ChangyongPanel changyongYuPanel;
    public BqPanel bqPanel;
    public ChatView chatPanel;

    /// <summary>
    /// 发消息
    /// </summary>
    /// <param name="type"></param>
    public void SendMessage(ChatInfo info, bool isClose)
    {
        if (PageManager.Instance.CurrentPage is LandlordsPage)
        {//斗地主
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                ddzChatReq = new DdzChatReq()
                {
                    type = info.type,
                    ddzChatContent = new DdzChatContent()
                    {
                        text = info.text,
                    }
                },
                msgid = MessageId.C2G_DdzChatReq
            });
        }
        else
        {//麻将
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                UserTalk =new UserTalk()
                {
                    msg=info.text,
                    type = info.type
                },
                msgid = MessageId.C2G_UserTalk
            });
        }
        if (SetNode.chat == 0)
            TipManager.Instance.OpenTip(TipType.SimpleTip, "您已关闭聊天功能,如要查看聊天信息请在设置里打开聊天功能");
        if (isClose)
            ChatNode.Close();
    }


    public static void Close()
    {
        NodeManager.CloseTargetNode<ChatNode>();
    }
}
