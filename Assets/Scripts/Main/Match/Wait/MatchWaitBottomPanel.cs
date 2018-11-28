using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchWaitBottomPanel : MonoBehaviour
{
    public MatchWaitStageItem prefab;
    public Transform content;
    public Image StageLine;

    public Image playIcon;
    public Text myRank;
    public GameObject rankRewardBtn, formatBtn;
    public MatchWaitFormatPanel formatPanel;
    public MatchWaitRankRewardPanel rankRewardPanel;

    List<MatchWaitStageItem> itemList = new List<MatchWaitStageItem>();
    public void Init()
    {
        UGUIEventListener.Get(rankRewardBtn).onClick = delegate { rankRewardPanel.gameObject.SetActive(!rankRewardPanel.gameObject.activeInHierarchy); };
        UGUIEventListener.Get(formatBtn).onClick = delegate { formatPanel.gameObject.SetActive(!formatPanel.gameObject.activeInHierarchy); };
        playIcon.sprite = UserInfoModel.userInfo.headIconSprite;
        rankRewardPanel.Init();
    }
    /// <summary>
    /// 更新数据
    /// </summary>
    public void FlushData(WaitMatcherRiseResp resp)
    {
        if (itemList.Count < 1)
            CreateStage(resp.stageNum);
        myRank.text = resp.myRank + "/" + resp.totalNum;
        SetCurStage(resp.currStageNum,resp.stageNum.Count);
    }
    /// <summary>
    /// 设置当前阶段
    /// </summary>
    private void SetCurStage(int curStage,int stageAllNum)
    {
        var curItem = itemList.Find(p => p.index == curStage);
        if (curItem)
        {
            curItem.Select();
            playIcon.transform.SetParent(curItem.show);
            playIcon.transform.localPosition = Vector3.zero;
            StageLine.fillAmount = (float)curItem.index / stageAllNum;
            try
            {
                string cur = "当前阶段：前几名晋级下一轮";
                string next = "下一阶段：前几名晋级下一轮";
                if (curItem != itemList[itemList.Count - 1])
                {
                    cur = "当前阶段：前" + curItem.staNum + "名晋级下一轮";
                    next = "下一阶段：前" + itemList[curItem.index].staNum + "名晋级下一轮";
                }

                formatPanel.SetValue(cur, next);
            }
            catch (System.Exception)
            {
                formatPanel.SetValue("当前阶段：前" + curItem.staNum + "名晋级下一轮", "当前阶段：无该阶段");
            }
        }
    }
    /// <summary>
    /// 创建比赛阶段
    /// </summary>
    private void CreateStage(List<int> stageList)
    {
        for (int i = 0; i < stageList.Count; i++)
        {
            MatchWaitStageItem item = Instantiate(prefab, content);
            item.itemTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 76 + 20 * i);
            item.index = i + 1;
            string title = stageList[i]+"人晋级";
            bool isWin = false;
            if (i == 0)
                title = stageList[i] + "人开赛";
            if (i == stageList.Count - 1)
            {
                title = "冠军";
                isWin = true;
            }
            item.Init(stageList[i], title, isWin);
            itemList.Add(item);
        }
    }

}
