using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagNode : Node
{
    public Button shoreBtn, competeBtn, callBtn;
    public Tab goodsBtn, redeemBtn;
    public BagGoodsPanel goodsPanel;
    public BagRedeemPanel redeemPanel;
    public GameObject hintPanel;
    public Text hintText;
    public List<BagIconData> iconList = new List<BagIconData>();
    public override void Init()
    {
        base.Init();
        goodsPanel.Init(this);
        redeemPanel.Init(this);
        hintPanel.SetActive(false);
        string textAsset = BundleManager.Instance.GetJson("GoodsConfig");
        JsonData jd = JsonMapper.ToObject(textAsset);
        for (int i = 0; i < jd.Count; i++)
        {
            BagIconData data = JsonMapper.ToObject<BagIconData>(JsonMapper.ToJson(jd[i]));
            iconList.Add(data);
        }
        UGUIEventListener.Get(callBtn.gameObject).onClick = (g) => { OpenCall(); };//拨打电话
        UGUIEventListener.Get(competeBtn.gameObject).onClick = (g) => { OpenMatch(); };//比赛
        UGUIEventListener.Get(shoreBtn.gameObject).onClick = (g) => { OpenStore(); };//s商店
        UGUIEventListener.Get(goodsBtn.gameObject).onClick = (g) => { SelectPanel(0); };
        UGUIEventListener.Get(redeemBtn.gameObject).onClick = (g) => { SelectPanel(1); };

    }
    public override void Open()
    {
        base.Open();
        SelectPanel();
    }
    private void OpenCall()
    {
        SDKManager.Instance.CopyToClipboard(UserInfoModel.userInfo.phoneNum);
    }
    private void OpenMatch()
    {
        PageManager.Instance.OpenPage<MatchPage>();
    }
    private void OpenStore()
    {
        NodeManager.OpenNode<StoreNode>();
    }
    private void SelectPanel(int num = 0)//0是goods 1是redeem
    {
        if (num == 0)
        {
            goodsPanel.Open();
            redeemPanel.Close();
        }
        if (num == 1)
        {
            redeemPanel.Open();
            goodsPanel.Close();
        }
    }

    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
        goodsPanel.Close();
    }
}
