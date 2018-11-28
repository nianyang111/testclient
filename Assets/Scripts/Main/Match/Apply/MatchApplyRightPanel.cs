using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchApplyRightPanel : MonoBehaviour {
    public GameObject applyBtn;
    public Image applyNeedIcon;
    public Text rangeTime, matchTime, applyNum, applyNeed;
    MatchApplyNode _node;
    MatcherInfo _data;
    public void Init(MatchApplyNode node)
    {
        _node = node;
        UGUIEventListener.Get(applyBtn).onClick = delegate { JoinMatch(); };
    }
    /// <summary> 参加 </summary>
    public void JoinMatch()
    {
        MatchModel.Instance.JoinMatch((int)_data.cost, _data.costType, _data.matchId, _data.name, _node);
    }
    public void Open()
    {
        _data = MatchModel.Instance.CurData;
        if (_data.costType > 0)
        {
            applyNeedIcon.gameObject.SetActive(true);
            applyNeedIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_log_" + _data.costType);
            applyNeed.text = _data.cost.ToString("#,##");
        }
        else
        {
            applyNeedIcon.gameObject.SetActive(false);
            applyNeed.text = "免费报名";
        }
        matchTime.text = string.Format(_data.spendTime + "分钟");
        applyNum.text = string.Format(_data.joinUser + "/" + _data.minUser);
        StartCoroutine(UpMyTime());
    }
    /// <summary>
    /// 刷新时间
    /// </summary>
    IEnumerator UpMyTime()
    {
        while (_data.distance > 0)
        {
            rangeTime.text = MatchPage.GetTimerText(MatchModel.Instance.CurData.distance, 2);
            yield return new WaitForSeconds(0.5f);
        }
        rangeTime.text = "00:00";
        yield return new WaitForSeconds(1.2f);
        var data = MatchModel.Instance.matcherInfoList.Find(p => p.name == _data.name);
        if (data != null)
            MatchModel.Instance.CurData = data;
        StartCoroutine(UpMyTime());
    }
}
