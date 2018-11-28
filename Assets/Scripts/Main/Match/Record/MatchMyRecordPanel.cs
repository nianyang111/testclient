using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMyRecordPanel : MonoBehaviour {
    public Text lvText;
    public Text lvExplainText;
    public GameObject HelpBtn;
    public Text finalistNum, promotionNum, successNum;
    public MatchMyRecordRewardItem prefab;
    public Transform content;
    public Text desText;
    public Transform group;
    public GameObject panelBtn;
    public List<MatchMyRecordRewardItem> ItemList = new List<MatchMyRecordRewardItem>();
    public void Init()
    {
        UGUIEventListener.Get(HelpBtn).onClick = (g) => { panelBtn.SetActive(true); };
        UGUIEventListener.Get(panelBtn).onClick = (g) => { panelBtn.SetActive(false); };
        panelBtn.SetActive(false);
    }

    public void Open()
    {
        int masterLv=UserInfoModel.userInfo.masterLevel;
        lvText.text = string.Format("Lv." + masterLv);
        int nextLv = masterLv;
        if (nextLv < 20)
            nextLv += 1;
        var data = MatchModel.Instance.GetLvJsonData(nextLv);
        long upExp = data != null ? data.upExp - UserInfoModel.userInfo.masterScore : 00;
        lvExplainText.text = string.Format("升级<color=#00FF01FF>Lv." + nextLv + "</color>还需要<color=#00FF01FF>" + upExp + "</color>大师分");
        
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
        {
            msgid = net_protocol.MessageId.C2G_QueryMatcherCount,
            QueryMatcherCount = new net_protocol.QueryMatcherCount()
            {
                userId=UserInfoModel.userInfo.userId
            }
        });
        SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage() {
            msgid = net_protocol.MessageId.C2G_UserMatcherReward,
        });
    }
    public void FlushData()
    {
        finalistNum.text = MatchModel.Instance.matcherCount.finalistNum.ToString();
        promotionNum.text = MatchModel.Instance.matcherCount.promotionNum.ToString();
        successNum.text = MatchModel.Instance.matcherCount.successNum.ToString();
    }

    public void CreateItem()
    {
        var rewardList = MatchModel.Instance.rewardList;
        for (int i = 0; i < rewardList.Count; i++)
        {
            var item = Instantiate(prefab, content);
            item.Init(rewardList[i]);
            ItemList.Add(item);
        }
        desText.text = rewardList.Count<0 ? "未获得任何奖励：" : "获得奖励：";
    }
    public void Close()
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            Destroy(ItemList[i].gameObject);
        }
        ItemList.Clear();
    }

}
