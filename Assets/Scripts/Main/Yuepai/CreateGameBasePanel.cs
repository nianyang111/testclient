using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreateGameBasePanel : MonoBehaviour {

    protected List<JsonData> jsons = new List<JsonData>();

    public GameObject prefabToggle;
    //番数
    public ToggleGroup fanshuParent;
    protected List<Toggle> fashuToggleList = new List<Toggle>();
    //局数
    public ToggleGroup jushuParent;
    protected List<Toggle> jushuToggleList = new List<Toggle>();

    public Text dizhuLb;

    public Text costFangkaLb;

    protected virtual void Start()
    {
        InitConfig();
    }

    /// <summary>
    /// 初始化配置
    /// </summary>
    protected virtual void InitConfig()
    {

    }

    protected Toggle CreateToggle(ToggleGroup parent)
    {
         Toggle toggle =Instantiate(prefabToggle, parent.transform).GetComponent<Toggle>();
         toggle.group = parent;
         return toggle;
    }

    /// <summary>
    /// 得到当前番数
    /// </summary>
    /// <returns></returns>
    public int GetCurFanshu()
    {
        int fanshu = 0;
        foreach (var item in fanshuParent.ActiveToggles())
        {
            fanshu = int.Parse(item.gameObject.name);
        }
        return fanshu;
    }

    /// <summary>
    /// 得到当前局数
    /// </summary>
    /// <returns></returns>
    public int GetCurJushu()
    {
        int jushu = 0;
        foreach (var item in jushuParent.ActiveToggles())
        {
            jushu = int.Parse(item.gameObject.name);
        }
        return jushu;
    }

    /// <summary>
    /// 得到当前消耗房卡数
    /// </summary>
    /// <returns></returns>
    public int GetCurCostFangka()
    {
        int jushu = GetCurJushu();
        JsonData json = jsons.Find(p => p["Round"].ToString() == jushu.ToString());
        if (json != null)
            return int.Parse(json["expendRoomcard"].ToString());
        return 0;
    }

    /// <summary>
    /// 得到当前底注
    /// </summary>
    /// <returns></returns>
    public int GetCurDizhu()
    {
        int fanshu = GetCurFanshu();
        JsonData json = jsons.Find(p => p["multipleLimit"].ToString() == fanshu.ToString());
        if (json != null)
            return int.Parse(json["ante"].ToString());
        return 0;
    }

    /// <summary>
    /// 得到当前人数
    /// </summary>
    /// <returns></returns>
    public virtual int GetCurRenshu()
    {
        return 0;
    }

    public void SetVisible(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}
