using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangyongItem : MonoBehaviour {

    public Button btn;
    public Text text;
    public int id;

    public void Init(CallBack<string> call)
    {
        JsonData jd = JsonMapper.ToObject(BundleManager.Instance.GetJson(ConstantUtils.changyongyuConfig));
        for (int i = 0; i < jd.Count; i++)
        {
            if (id.ToString() == jd[i].TryGetString("id"))
            {
                text.text = jd[i].TryGetString("chatContent");
                break;
            }
        }

        btn.onClick.AddListener(() => call(text.text));
    }
}
