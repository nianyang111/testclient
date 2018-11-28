using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchRankShowPanel : MonoBehaviour {
    public MatchReward[] RewardArray;
    public Transform content;
    public Text prefab;
    public Text rankBtnText;
    public GameObject rankPanel;
    public GameObject rankBtn;
    List<Text> textList = new List<Text>();
    public void Init()
    {
        UGUIEventListener.Get(rankBtn).onClick = delegate { rankPanel.SetActive(!rankPanel.activeInHierarchy); };
    }
    public void Open()
    {
        var list = MatchModel.Instance.CurData.rankReard;
        for (int i = 0; i < list.Count; i++)
        {
            if (i > -1 && i < 3)
            {
                RewardArray[i].SetValue(list[i].reward);
            }
            else
            {
                Text item = Instantiate(prefab, content);
                item.text = string.Format("第" + list[i].rank + "名     " + list[i].reward[0].name);
                textList.Add(item);
            }
        }
        if (list.Count > 3)
        {
            string rankText = list[list.Count - 1].rank;
            int index = rankText.IndexOf("-");
            rankText = rankText.Substring(index + 1);
            rankBtnText.text = string.Format("前" + rankText + "有奖励");
            rankBtn.SetActive(true);
        }
        else
        {
            rankBtn.SetActive(false);
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
