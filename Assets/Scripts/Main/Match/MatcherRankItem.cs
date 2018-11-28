using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatcherRankItem : MonoBehaviour {
    public Text rank, nick, score;
    public Image rankIcon;
    public MatcherPlayerRanking data;
    public int num;
    public void Init(MatcherPlayerRanking data,int num)
    {
        this.data = data;
        this.num = num;
        rank.text = data.rank.ToString();
        nick.text = data.userName;
        score.text = data.score.ToString();
        rankIcon.gameObject.SetActive(true);
        rank.gameObject.SetActive(false);
        if (num > 0 && num < 4)
            rankIcon.sprite = BundleManager.Instance.GetSprite("Common/normal_jiangbei_" + num);
        switch (num)
        {
            case 1:
                score.color = Color.white;
                score.color = new Color(255f / 255, 228f / 255, 75f / 255);
                break;
            case 2:
                score.color = Color.white;
                score.color = new Color(148f / 255, 148f / 255, 188f / 255);
                break;
            case 3:
                score.color = Color.white;
                score.color = new Color(177f / 255, 96f / 255, 30f / 255);
                break;
            default:
                rankIcon.gameObject.SetActive(false);
                rank.gameObject.SetActive(true);
                break;
        }

    }
}