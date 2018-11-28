using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchJoinItem : MonoBehaviour {
    public Image log;
    public Text matchName,matchTime,matchNum;
    public Text distance;
    public Button targetBtn;

    public void Init(MatchJoinData data)
    {
        matchName.text = data.matchName;
        matchNum.text = data.matchNum.ToString();
        StartCoroutine(FlushTime(data.distance));
        targetBtn.onClick.AddListener(delegate {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            NodeManager.OpenNode<MatchApplyNode>("match").OpenQuitPanel(data.matchId); 
        });
    }
    IEnumerator FlushTime(long dis)
    {
        while (dis>0)
        {
            distance.text = MatchPage.GetTimerText(dis, 2);
            dis--;
            yield return new WaitForSecondsRealtime(1f);
        }
        distance.text = "00:00";
    }
}
public class MatchJoinData
{
    public string rewardIcon;
    public string matchId;
    public string matchName;
    public int matchType;
    public int matachTime, matchNum;
    public long distance;
}