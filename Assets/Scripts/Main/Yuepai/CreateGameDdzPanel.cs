using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameDdzPanel : CreateGameBasePanel {

    protected override void InitConfig()
    {
        string textAsset = BundleManager.Instance.GetJson("DdzMeetConfig");
        JsonData allJson = JsonMapper.ToObject(textAsset);
        for (int i = 0; i < allJson.Count; i++)
        {
            InitFanshu(int.Parse(allJson[i]["multipleLimit"].ToString()), i == 0);
            InitJushu(int.Parse(allJson[i]["Round"].ToString()), i == 0);
            jsons.Add(allJson[i]);
        }
    }

    /// <summary>
    /// 创建番数Toggle
    /// </summary>
    /// <param name="num"></param>
    void InitFanshu(int num, bool isOn)
    {
        Toggle toggle = CreateToggle(fanshuParent);
        toggle.transform.Find("Label").GetComponent<Text>().text = num.ToString();
        toggle.gameObject.name = num.ToString();
        toggle.onValueChanged.AddListener(b => OnFanshuValueChange());
        toggle.isOn = isOn;
    }

    /// <summary>
    /// 创建局数Toggle
    /// </summary>
    /// <param name="num"></param>
    void InitJushu(int num, bool isOn)
    {
        Toggle toggle = CreateToggle(jushuParent);
        toggle.transform.Find("Label").GetComponent<Text>().text = num.ToString();
        toggle.gameObject.name = num.ToString();
        toggle.onValueChanged.AddListener(b => OnJushuValueChange());
        toggle.isOn = isOn;
    }
    

    void OnFanshuValueChange()
    {
        dizhuLb.text = GetCurDizhu().ToString();
    }

    void OnJushuValueChange()
    {
        costFangkaLb.text = GetCurCostFangka().ToString();
    }

    public override int GetCurRenshu()
    {
        return 3;
    }
}
