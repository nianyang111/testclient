using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMyRecordRewardItem : MonoBehaviour {
    public GameObject hintBtn ,hintPanel;
    public Text matchName, hintText, rewardText;
    public Image icon;

    public void Init(MatcherRewardlog reward)
    {
        hintPanel.SetActive(false);
        matchName.text = reward.matcherName;
        hintText.text = reward.rewardDesc;
        UGUIEventListener.Get(hintBtn).onClick = delegate { StartCoroutine(OpenHint()); };
        StartCoroutine(MiscUtils.DownloadImage(reward.rewardIcon, spr => {
            if (spr != null)
            {
                icon.sprite = spr;
                rewardText.gameObject.SetActive(false);
            }
            else
                rewardText.text = reward.rewardName;
        }));
    }
    IEnumerator OpenHint()
    {
        hintPanel.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        hintPanel.SetActive(false);
    }
}
