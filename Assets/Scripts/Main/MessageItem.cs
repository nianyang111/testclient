using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageItem : MonoBehaviour, IPointerClickHandler
{
    public Text title;
    public Text content;
    public Text date;
    public Text isReadText;
    public Image isRead;
    public MessageNode _node;
    public string msgId;
    public void Init(UserMsg.Msg data)
    {
        msgId = data.msgId;
        title.text = data.msgTitle;
        if (data.msgDesc != null && data.msgDesc.Length > 8)
            content.text = data.msgDesc.Substring(0, 10) + "...";
        else
            content.text = data.msgDesc;
        date.text = data.msgCreated;
        isRead.color = data.msgIsread == "1" ? Color.green : Color.red;
        isReadText.text = data.msgIsread == "1" ? "已读" : "未读";
    }
    /// <summary>
    /// 阅读信息
    /// </summary>
    public void ReadMsg()
    {
        isRead.color = Color.green;
        isReadText.text = "已读";
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!AudioManager.Instance.IsSoundPlaying)
            AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_ReadUserMsg,
            ReadUserMsg = new net_protocol.ReadUserMsg()
            {
                msgId = msgId
            }
        },true);
    }
}