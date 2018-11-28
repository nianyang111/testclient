using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 消息
/// </summary>
public class MessageNode : Node
{
    public Transform content;
    public MessageItem prefab;
    private List<MessageItem> itemList = new List<MessageItem>();
    public GameObject msgInfoPanel,infoTrueBtn,infoCloseBtn;
    public Text titleText,desText;
    public MessageItem curItem{get;set;}
    public override void Init()
    {
        base.Init();
        UGUIEventListener.Get(infoCloseBtn).onClick = (g) => { msgInfoPanel.SetActive(false); };
        UGUIEventListener.Get(infoTrueBtn).onClick = (g) => { MsgInfoTure(); };
        msgInfoPanel.gameObject.SetActive(false);
    }
    private void MsgInfoTure()
    {
        //if (curItem.isActivity==1)
        //{
        //    //NodeManager.OpenNode
        //}
        //else
        //{
            msgInfoPanel.SetActive(false); 
       //}
    }
    public override void Open()
    {
        base.Open();

        SetValue();
    }
    /// <summary>
    /// 阅读信息
    /// </summary>
    public void ReadMsg(net_protocol.ReadMsgResp resp)
    {
        if(resp.resultCode==1)
        try
        {
            var data= MessageModel.Instance.msgData.msg.Find(p => p.msgId == resp.msgId);
            msgInfoPanel.gameObject.SetActive(true);
            titleText.text = data.msgTitle;
            desText.text = data.msgDesc;
            data.msgIsread = "1";
            curItem = itemList.Find(p => p.msgId == resp.msgId);
            curItem.ReadMsg();
        }
        catch (System.Exception)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "没有此条消息");
        }
    }
    /// <summary>
    /// 设置数据
    /// </summary>
    public void SetValue()
    {
        if (MessageModel.Instance.msgData != null)
        {
            List<net_protocol.UserMsg.Msg> msgList = MessageModel.Instance.msgData.msg;
            for (int i = 0; i < msgList.Count; i++)
            {
                MessageItem item = Instantiate(prefab, content);
                item._node = this;
                item.Init(msgList[i]);
                itemList.Add(item);
            }
        }
        else { 
         
        }
    }
 
    public override void Close(bool isOpenLast = false)
    {
        base.Close(false);
        for (int i = 0; i < itemList.Count; i++)
        {
            Destroy(itemList[i].gameObject);
        }
        itemList.Clear();
        MessageModel.Instance.PlayAction();
    }
}
