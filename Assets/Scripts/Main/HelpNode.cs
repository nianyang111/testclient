using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class HelpNode : Node {
    public GameObject leftBtn;
    public Transform leftContent;
    public HelpItem rightBtn;
    public Transform rightContent;

    List<HelpData> dataList = new List<HelpData>();
    List<string> strList = new List<string>();
    List<HelpLeftBtn> objList = new List<HelpLeftBtn>();
    List<HelpItem> itemList = new List<HelpItem>();

    private HelpLeftBtn curBtn;
    public HelpLeftBtn CurBtn
    {
        set
        {
            if (curBtn != null)
                curBtn.OnSelect();
            curBtn = value;
            curBtn.Select();
            SetRightPanel();
        }
        get
        {
            for (int i = 0; i <  itemList.Count; i++)
            {
                Destroy(itemList[i].gameObject);
            }
            itemList.Clear();
            return curBtn;
        }
    }
    public override void Init()
    {
        base.Init();
        string textAsset = BundleManager.Instance.GetJson("GameHelpConfig");
        JsonData jd = JsonMapper.ToObject(textAsset);
        for (int i = 0; i < jd.Count; i++)
         {
             HelpData data = JsonMapper.ToObject<HelpData>(JsonMapper.ToJson(jd[i]));
             dataList.Add(data);
         }
        for (int i = 0; i < dataList.Count; i++)
        {
            if (strList.Find(p => p == dataList[i].type)==null)
            {
                strList.Add(dataList[i].type);
            }
        }
    }
    public override void Open()
    {
        base.Open();
        if (objList.Count <1)
        {
            for (int i = 0; i < strList.Count; i++)
            {
                GameObject go = Instantiate(leftBtn, leftContent);
                HelpLeftBtn item = go.AddComponent<HelpLeftBtn>();
                item._node = this;
                item.Init(strList[i]);
                objList.Add(item);
            }
            CurBtn = objList[0];
        }
    }
    private void SetRightPanel()
    {
        var curDataList = dataList.FindAll(p => p.type == CurBtn._str);
        for (int i = 0; i < curDataList.Count; i++)
        {
            HelpItem item= Instantiate(rightBtn, rightContent);
            item.Init(curDataList[i]);
            itemList.Add(item);
        }
    }

    public override void Close(bool isOpenLast = true)
    {
        base.Close(isOpenLast);
        for (int i = 0; i < itemList.Count; i++)
        {
            Destroy(itemList[i].gameObject);
        }
        itemList.Clear();
    }


    public class HelpLeftBtn : MonoBehaviour
    {
        private Text text, text2;
        private GameObject select;
        public HelpNode _node;
        public string _str;
        public void Init(string str)
        {
            _str = str;
            select = transform.Find("Select").gameObject;
            gameObject.GetComponentInChildren<Text>().text = transform.Find("Select/Text").GetComponent<Text>().text = str;
            gameObject.GetComponent<Button>().onClick.AddListener(() => {
                if (!AudioManager.Instance.IsSoundPlaying)
                    AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
                _node.CurBtn = this; 
            });
            OnSelect();
        }
        public void Select()
        {
            select.gameObject.SetActive(true);
        }
        public void OnSelect()
        {
            select.gameObject.SetActive(false);
        }
    }
}

public class HelpData
{
    public string answerType;
    public int id;
    public string questionType;
    public string type;
    public int typeId;
}
