using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class YuepaiHorizontalItem : MonoBehaviour
{

    public Text jushuLb;
    public Text player0Lb;
    public Text player1Lb;
    public Text player2Lb;
    public Text player3Lb;
    public void Init(YuePaiLog info)
    {
        jushuLb.text = info.curr_round + "/" + info.round + "局";
        if (info.YuePaiOther.Count > 0)
            player0Lb.text = info.YuePaiOther[0].score > 0 ? "+" + info.YuePaiOther[0].score : info.YuePaiOther[0].score.ToString();
        if (info.YuePaiOther.Count > 1)
            player1Lb.text = info.YuePaiOther[1].score > 0 ? "+" + info.YuePaiOther[1].score : info.YuePaiOther[1].score.ToString();
        if (info.YuePaiOther.Count > 2)
            player2Lb.text = info.YuePaiOther[2].score > 0 ? "+" + info.YuePaiOther[2].score : info.YuePaiOther[2].score.ToString();
        if (info.YuePaiOther.Count > 3)
            player3Lb.text = info.YuePaiOther[3].score > 0 ? "+" + info.YuePaiOther[3].score : info.YuePaiOther[3].score.ToString();
    }
}
