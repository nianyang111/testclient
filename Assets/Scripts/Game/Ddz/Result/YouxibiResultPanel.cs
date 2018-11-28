using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YouxibiResultPanel : MonoBehaviour {

    public GameObject winObj;
    public GameObject loseObj;

    public GameObject changeBtn;

    public GameObject shareBtn;
    public GameObject shareObj;
    public GameObject circleOfFriendsBtn;//分享到朋友圈
    public GameObject firendBtn;//分享到微信好友

    public GameObject goOnBtn;
    public GameObject rechargeBtn;

    public GameObject closeBtn;
    public List<ResultItem> items = new List<ResultItem>();

    public Text beishuLb;
    
    public Transform desParent;
    public GameObject desPrefab;//文本item
    void Start()
    {
        UGUIEventListener.Get(closeBtn).onClick = delegate { gameObject.SetActive(false); };
        UGUIEventListener.Get(changeBtn).onClick = delegate { LandlordsNet.C2G_ChangeTabelReq(); };

        UGUIEventListener.Get(shareBtn).onClick = delegate { shareObj.SetActive(!shareObj.activeInHierarchy); };
        UGUIEventListener.Get(circleOfFriendsBtn).onClick = delegate { Share(SDKManager.WechatShareScene.WXSceneTimeline); };
        UGUIEventListener.Get(firendBtn).onClick = delegate { Share(SDKManager.WechatShareScene.WXSceneSession); };

        UGUIEventListener.Get(goOnBtn).onClick = delegate { Interaction.Instance.Zhunbei(); };
        UGUIEventListener.Get(rechargeBtn).onClick = delegate { gameObject.SetActive(false); NodeManager.OpenNode<StoreNode>(); };
    }

    public void Init()
    {
        gameObject.SetActive(true);
        bool isWin = LandlordsModel.Instance.CurWinerIds.Contains(UserInfoModel.userInfo.userId);
        winObj.SetActive(isWin);
        loseObj.SetActive(!isWin);
        List<DdzJSPlayerInfo> resultInfos = LandlordsModel.Instance.ResultModel.GetResultInfos();
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Init(resultInfos[i]);
        }
        LoadDes();
    }

    /// <summary>
    /// 加载倍数文本
    /// </summary>
    void LoadDes()
    {
        UIUtils.DestroyChildren( desParent);
        Dictionary<string, int> ratioDic = new Dictionary<string, int>();
        
        if (LandlordsModel.Instance.ResultModel.jdz != 0)
            ratioDic.Add("叫地主", LandlordsModel.Instance.ResultModel.jdz);
        if (LandlordsModel.Instance.ResultModel.zd != 0)
            ratioDic.Add("炸弹", LandlordsModel.Instance.ResultModel.zd);
        if (LandlordsModel.Instance.ResultModel.ct != 0)
            ratioDic.Add("春天", LandlordsModel.Instance.ResultModel.ct);
        if (LandlordsModel.Instance.ResultModel.fct != 0)
            ratioDic.Add("反春天", LandlordsModel.Instance.ResultModel.fct);

        foreach (var item in ratioDic)
        {
            GameObject ratioItem = Instantiate(desPrefab, desParent);
            ratioItem.GetComponent<Text>().text = item.Key;
            ratioItem.transform.Find("value").GetComponent<Text>().text = "x" + item.Value;
        }
        beishuLb.text = LandlordsPage.Instance.Multiples + "倍";
    }


    void Share(SDKManager.WechatShareScene scene)
    {
        string gameName="";//游戏名
        string roomType="";
        int income=0;//收入
        if(PageManager.Instance.CurrentPage is LandlordsPage)
        {
            gameName = "斗地主";            
            switch (LandlordsModel.Instance.RoomModel.CurRoomInfo.RoomType)
            {
                case RoomType.SilverCoin:
                    roomType = "银币场";
                    break;
                case RoomType.GoldBar:
                    roomType="金条场";
                    break;
            }
            income = LandlordsModel.Instance.ResultModel.GetResultInfos().Find(p => p.userId == UserInfoModel.userInfo.userId).income;
        }
        else if (PageManager.Instance.CurrentPage is MaJangPage)
        {
            gameName="麻将";
        }
        string des = string.Format("我在{0}{1}房间中{2}了{3},快来和我一起玩吧", gameName, roomType, income > 0 ? "赢" : "输", Mathf.Abs(income));
        Sprite icon = BundleManager.Instance.GetSprite("task/meirirenwu_pic_1");
        SDKManager.Instance.ShareWebPage(scene, UserInfoModel.userInfo.downUrl, "雪瑶明水棋牌", des, MiscUtils.SizeTextureBilinear(icon.texture, Vector2.one * 150).EncodeToJPG());
    }
}
