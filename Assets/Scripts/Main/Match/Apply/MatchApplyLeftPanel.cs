using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchApplyLeftPanel : MonoBehaviour {

    public MatchReward[] RewardArray;
    public ScrollRect scrollRect;
    public Transform content;
    public Text prefab;
    public GameObject rulesBtn;
    MatchApplyNode _node;
    List<Text> textList = new List<Text>();
    public void Init(MatchApplyNode node)  
    {
        _node = node;
        UGUIEventListener.Get(rulesBtn).onClick = (g) => { _node.rulesPanel.SetActive(true); };
    }
    public void Open(List<RankReward> list)
    {
        scrollRect.vertical = list.Count > 3;
        for (int i = 0; i < list.Count; i++)
        {
            if (i > -1 && i < 3)
            {
                RewardArray[i].SetValue(list[i].reward);
            }
            else
            {
                var item = Instantiate(prefab, content);
                item.text = string.Format("第" + list[i].rank + "名     " + list[i].reward[0].name);
                textList.Add(item);
            }
        }
    }

    public void Close()
    {
        for (int i = 0; i < textList.Count; i++)
        {
            Destroy(textList[i].gameObject);
        }
        textList.Clear();
    }
}
