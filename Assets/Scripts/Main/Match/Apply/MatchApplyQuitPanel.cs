using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchApplyQuitPanel : MonoBehaviour {
    public GameObject quitBtn, hintBtn;
    public GameObject hintPanel;

    public void Init(string matchId)
    {
        gameObject.SetActive(true);
        hintPanel.SetActive(false);
        UGUIEventListener.Get(quitBtn).onClick = delegate { QuitMatch(matchId); };
        UGUIEventListener.Get(hintBtn).onClick = delegate { hintPanel.SetActive(!hintPanel.activeInHierarchy); };
    }
    void QuitMatch(string matchId)
    {
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_QuitMatcher,
            quitMatcher = new net_protocol.QuitMatcher()
            {
                matcherId = matchId
            }
        });
    }
}
