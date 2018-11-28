using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
public class CreateGameMjPanel : CreateGameBasePanel {

    //人数
    public ToggleGroup renshuParent;
    List<Toggle> reshuToggles = new List<Toggle>();


    protected override void InitConfig()
    {
        string textAsset = BundleManager.Instance.GetJson("MjMeetConfig");
        JsonData allJson = JsonMapper.ToObject(textAsset);
        for (int i = 0; i < allJson.Count; i++)
        {
            InitFanshu(int.Parse(allJson[i]["multipleLimit"].ToString()), i == 0);
            InitJushu(int.Parse(allJson[i]["Round"].ToString()), i == 0);
            jsons.Add(allJson[i]);
        }
        InitRenshu(2, true);
        InitRenshu(4, false);
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
   void InitJushu(int num,bool isOn)
   {
       Toggle toggle = CreateToggle(jushuParent);
       toggle.transform.Find("Label").GetComponent<Text>().text = num.ToString();
       toggle.gameObject.name = num.ToString();
       toggle.onValueChanged.AddListener(b => OnJushuValueChange());
       toggle.isOn = isOn;
   }

   /// <summary>
   /// 创建人数Toggle
   /// </summary>
   /// <param name="num"></param>
   void InitRenshu(int num, bool isOn)
   {
       Toggle toggle = CreateToggle(renshuParent);
       toggle.transform.Find("Label").GetComponent<Text>().text = num.ToString();
       toggle.gameObject.name = num.ToString();
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
       int renshu = 0;
       foreach (var item in renshuParent.ActiveToggles())
       {
           renshu = int.Parse(item.gameObject.name);
       }
       return renshu;
   }
}
