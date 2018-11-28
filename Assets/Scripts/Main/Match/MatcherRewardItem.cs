using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatcherRewardItem : MonoBehaviour {
    public Text rank;
    public Text reward;
    public Image rankIcon;

    public void Init(RankReward data)
    {
        rank.text = data.rank;
        reward.text = data.reward[0].name;
        rankIcon.gameObject.SetActive(true);
        rank.gameObject.SetActive(false);
        switch (data.rank)
        {
            case "1":
                reward.color = Color.white;
                reward.color = new Color(255f / 255, 228f / 255, 75f / 255);
                rankIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_jiangbei_" + data.rank);
                break;
            case   "2":
                reward.color = Color.white;
                reward.color = new Color(148f / 255, 148f / 255, 188f / 255);
                rankIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_jiangbei_" + data.rank);
                break;
            case "3":
                reward.color = Color.white;
                reward.color = new Color(177f / 255, 96f / 255, 30f / 255);
                rankIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_jiangbei_" + data.rank);
                break;
            default:
                rankIcon.gameObject.SetActive(false);
                rank.gameObject.SetActive(true);
                break;
        }
    }
}
