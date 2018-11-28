using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionItem : MonoBehaviour {

    public Text question;
    public Text answer;
    public Text time;

    public void Init(Opinion info)
    {
        question.text = "问: " + info.content;
        answer.text = "答: " + (string.IsNullOrEmpty(info.answer) || info.answer == "null" ? "收到,我们将尽快为您处理." : info.answer);
        System.DateTime dateTime = MiscUtils.GetDateTimeByTimeStamp(info.datetime);
        string year = dateTime.Year.ToString();
        string month = dateTime.Month.ToString("D2");
        string day = dateTime.Day.ToString("D2");
        string hour = dateTime.Hour.ToString("D2");
        string min = dateTime.Minute.ToString("D2");
        string s = dateTime.Second.ToString("D2");

        time.text = string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, min, s);
    }
}
