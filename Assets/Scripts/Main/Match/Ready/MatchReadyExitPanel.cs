using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchReadyExitPanel : MonoBehaviour
{
    public GameObject closeBtn;
    public Image rewardIcon;
    public GameObject exitBtn, keepBtn;
    public void Init()
    {
        UGUIEventListener.Get(closeBtn).onClick = delegate { gameObject.SetActive(false); };
        UGUIEventListener.Get(keepBtn).onClick = delegate { gameObject.SetActive(false); };
        UGUIEventListener.Get(exitBtn).onClick = delegate { Eixt(); };
    }

    public void Open()
    {
        if (MatchModel.Instance.rewardIcon != null)
            rewardIcon.sprite = MatchModel.Instance.rewardIcon;
    }
    private void Eixt()
    {
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_QuitMatcher,
            quitMatcher = new net_protocol.QuitMatcher()
            {
                matcherId = MatchModel.Instance.CurData.matchId
            }
        }, true);
    }

}
