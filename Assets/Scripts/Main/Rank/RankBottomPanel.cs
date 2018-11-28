using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 排行榜玩家个人信息
/// </summary>
public class RankBottomPanel : MonoBehaviour {
    [HideInInspector]
    public Image rankNumIcon ,playIcon;
    [HideInInspector]
    public Text rankNumText,playName,playGetNum,curRankNum;
    
    public GameObject PromoteBtn;
    public void Init() {
        UGUIEventListener.Get(PromoteBtn).onClick = (g) => { NodeManager.OpenNode<StoreNode>(); };
    }
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="rank"></param>
    /// <param name="reap"></param>
    public void SetValue(int rank, long reap)
    {
        if (rank > 0&&rank < 4)
        {
            rankNumText.gameObject.SetActive(false);
            rankNumIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_jiangbei_" + rank);
            rankNumIcon.SetNativeSize();
        }
        else
        {
            rankNumText.gameObject.SetActive(true);
            rankNumIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_icon_mingci");
            rankNumText.text = rank.ToString();
        }
        playIcon.sprite = UserInfoModel.userInfo.headIconSprite;
        playName.text = UserInfoModel.userInfo.nickName;
        playGetNum.text = "今日收益：" + reap;
        curRankNum.text = "当前排名：" + rank;
    }
}
