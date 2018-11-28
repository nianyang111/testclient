using net_protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class YuepaiLogItem : MonoBehaviour {

    public Text timeLb;
    public Text gameTypeLb;
    public Text jushuLb;
    public Text otherPlayersLb;
    public Text myResultLb;
    public GameObject chakanBtn;

    public void Inits(YuePaiTable info, CallBack<YuePaiTable> chakanCall)
    {
        DateTime dataTime = MiscUtils.GetDateTimeByTimeStamp(info.playat / 1000);
        timeLb.text = string.Format("{0}月{1}日{2}：{3}", dataTime.Month.ToString("D2"), dataTime.Day.ToString("D2"), dataTime.Hour.ToString("D2"), dataTime.Minute.ToString("D2"));
        gameTypeLb.text = info.type;
        jushuLb.text = info.round.ToString();

        for (int i = 0; i < info.yuePaiLog[0].YuePaiOther.Count; i++)
        {
            if (info.yuePaiLog[0].YuePaiOther[i].userId != UserInfoModel.userInfo.userId)
                continue;
            YuePaiOther playerInfo = info.yuePaiLog[0].YuePaiOther[i];
            int allRessult = 0;
            for (int j = 0; j < info.yuePaiLog.Count; j++)
            {
                for (int k = 0; k < info.yuePaiLog[j].YuePaiOther.Count; k++)
                {
                    if (info.yuePaiLog[j].YuePaiOther[k].userId == playerInfo.userId)
                        allRessult += info.yuePaiLog[j].YuePaiOther[k].score;
                }
            }
            myResultLb.text = allRessult > 0 ? "+" + allRessult + "分" : allRessult + "分";
        }
        otherPlayersLb.text = info.other_user;
        UGUIEventListener.Get(chakanBtn).onClick = delegate { chakanCall(info); };
    }
}
