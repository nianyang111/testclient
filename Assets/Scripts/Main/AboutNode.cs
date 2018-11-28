using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 关于
/// </summary>
public class AboutNode : Node
{

    public Tab aboutBtn, serviceBtn, policyBtn, proclaimBtn;
    public Text aboutPolicy, aboutApproval, serviceBody, policyBody, proclaimBody;
    public Transform sPrefab, pPrefab;
    List<AboutData> dataList = new List<AboutData>();
    public override void Init()
    {
        base.Init();
        UGUIEventListener.Get(aboutBtn.gameObject).onClick = (g) => { SelectPanel(1); };
        UGUIEventListener.Get(serviceBtn.gameObject).onClick = (g) => { SelectPanel(2); };
        UGUIEventListener.Get(policyBtn.gameObject).onClick = (g) => { SelectPanel(3); };
        UGUIEventListener.Get(proclaimBtn.gameObject).onClick = (g) => { SelectPanel(4); };
        if (dataList.Count > 0) return;
        JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.usConfig));
        for (int i = 0; i < jd.Count; i++)
        {
            AboutData data = JsonMapper.ToObject<AboutData>(JsonMapper.ToJson(jd[i]));
            dataList.Add(data);
        }
    }
    public override void Open()
    {
        base.Open();
        SelectPanel(1);
    }
    private void SelectPanel(int num)
    {
        var data = dataList.Find(p => p.id == num);
        switch (num)
        {
            case 1:
                aboutPolicy.text = data.content;
                break;
            case 2:
                serviceBody.text = data.content;
                serviceBody.transform.SetParent(sPrefab);
                break;
            case 3:
                policyBody.text = data.content;
                policyBody.transform.SetParent(pPrefab);
                break;
            case 4:
                proclaimBody.text = data.content;
                break;
            default:
                break;
        }

    }
}
public class AboutData
{
    public string content;
    public int id;
    public string type;
}
