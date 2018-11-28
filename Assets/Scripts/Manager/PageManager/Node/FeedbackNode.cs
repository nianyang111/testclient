using net_protocol;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class FeedbackNode : Node
{

    public Toggle loginTg, rechargeTg, gameTg, otherTg;
    public InputField bugInput, phoneInput;
    public GameObject screnBtn, subMitBtn,hotTelBtn, questionPrefab;
    public Transform itemParent;
    public Text hotPhone;
    public Image spr;
    string texName = "FeedbackNode_Screen";
    byte[] textureByte;
    public override void Open()
    {        
        base.Open();
        UGUIEventListener.Get(screnBtn).onClick = Screen;
        UGUIEventListener.Get(subMitBtn).onClick = Submit;
        UGUIEventListener.Get(hotTelBtn).onClick = HotTle;        
        ReqMyQuestion();
    }

    /// <summary>
    /// 提交
    /// </summary>
    void Submit(GameObject go)
    {
        if (string.IsNullOrEmpty(bugInput.text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "请输入反馈内容");
            return;
        }
        if (Encoding.UTF8.GetByteCount(bugInput.text) > 150 * 3)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "字数不能超过150字哦!");
            return;
        }
        if (string.IsNullOrEmpty(phoneInput.text))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "请输入联系方式!");
            return;
        }
        if (phoneInput.text.Length > 20)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "联系方式不能超过20个数字哦!");
            return;
        }
        int type = 0;//0登录问题1充值2游戏3其他;// MiscUtils.SizeTextureBilinear(spr.sprite.texture, Vector3.one * 0.05f).EncodeToJPG();
        if (loginTg.isOn)
            type = 0;
        else if (rechargeTg.isOn)
            type = 1;
        else if (gameTg.isOn)
            type = 2;
        else if (otherTg.isOn)
            type = 3;
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                submitOpinionReq = new SubmitOpinionReq()
                {
                    type = type,
                    contact = phoneInput.text,
                    content = bugInput.text,
                    image = textureByte
                },
                msgid = MessageId.C2G_SubmitOpinionReq
            });
    }

    /// <summary>
    /// 热线
    /// </summary>
    /// <param name="go"></param>
    void HotTle(GameObject go)
    {
        TipManager.Instance.OpenTip(TipType.AlertTip, "复制客服热线", 0, () =>
            {
                SDKManager.Instance.CopyToClipboard(hotPhone.text);                
                TipManager.Instance.OpenTip(TipType.SimpleTip, "复制成功!");
            });
    }

    /// <summary>
    /// 提交成功服务器回调
    /// </summary>
    public static void G2C_Submit()
    {
        FeedbackNode node = NodeManager.GetNode<FeedbackNode>();
        if (node != null)
        {
            node.ClearInfo();
            node.ReqMyQuestion();            
        }
        
    }
    
    /// <summary>
    /// 截图
    /// </summary>
    void Screen(GameObject go)
    {
        SDKManager.Instance.OpenPhoto(texName, () =>
            {
                StartCoroutine(LoadImage());
            });
    }

    IEnumerator LoadImage()
    {
        string url = "file://" + Application.persistentDataPath + "/" + texName;
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            textureByte = www.bytes;
            spr.sprite = MiscUtils.TextureToSprite(www.texture);
        }
        else
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "资源加载失败");
        }
    }

    /// <summary>
    /// 请求我历史的问题
    /// </summary>
    void ReqMyQuestion()
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid=MessageId.C2G_QueryOpinionReq
        });
    }

    /// <summary>
    /// 清除信息
    /// </summary>
    void ClearInfo()
    {
        bugInput.text = string.Empty;
        phoneInput.text = string.Empty;
        textureByte = null;
        spr.sprite = BundleManager.Instance.GetSprite("majiang_duijuliushu_panel", PageManager.Instance.gamecommonBundle);
    }

    /// <summary>
    /// 服务器回调加载我的问题
    /// </summary>
    /// <param name="suggestList"></param>
    public void G2C_LoadItems(List<Opinion> suggestList)
    {
        UIUtils.DestroyChildren(itemParent);
        suggestList.Sort((a, b) =>
            {
                return b.datetime.CompareTo(a.datetime);
            });
        for (int i = 0; i < suggestList.Count; i++)
        {
            QuestionItem item = Instantiate(questionPrefab, itemParent).GetComponent<QuestionItem>();
            item.Init(suggestList[i]);
        }
    }

    
}

