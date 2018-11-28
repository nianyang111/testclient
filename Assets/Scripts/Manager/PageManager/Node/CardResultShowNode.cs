using net_protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardResultShowNode : Node
{

    public Text gameTypeLb;

    public Text roomInfoLb;

    public Image erweimaIcon;

    public Text timeLb;

    public GameObject screenBtn;

    public GameObject weixinBtn;

    public Transform vertical_parent;
    public HorizontalLayoutGroup horizontal_parent;

    public GameObject vertical_prefab;
    public GameObject horizontal_prefab;

    List<ShareItemInfo> shareInfos = new List<ShareItemInfo>();

    List<YuepaiLogPlayerInfo> playerInfos = new List<YuepaiLogPlayerInfo>();
    YuePaiTable curYuepaiTable;


    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(screenBtn).onClick = delegate { SDKManager.Instance.ScrrenShoot("MyScreenshot", "MyApp", true); };
        UGUIEventListener.Get(weixinBtn).onClick = delegate { StartCoroutine(WeixinBtn()); };
    }

    public static void G2c_Init(YuePaiTable info)
    {
        CardResultShowNode node = NodeManager.GetNode<CardResultShowNode>();
        if(node)
        {
            node.Inits(info);
        }
    }

    public void Inits(YuePaiTable info)
    {
        if (info == null)
        {
            //请求约牌记录
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_YuePaiTable
            });
            return;
        }
        curYuepaiTable = info;
        gameTypeLb.text = info.type;
        roomInfoLb.text = "房卡房<color=#01AEFA>" + info.yuePaiLog[info.yuePaiLog.Count - 1].curr_round + "</color>" + "次对局";

        GetCodeTextrue();

        DateTime dataTime = MiscUtils.GetDateTimeByTimeStamp(info.playat / 1000);
        timeLb.text = string.Format("{0}月{1}日{2}：{3}", dataTime.Month.ToString("D2"), dataTime.Day.ToString("D2"), dataTime.Hour.ToString("D2"), dataTime.Minute.ToString("D2"));
        //加载总体
        UIUtils.DestroyChildren(vertical_parent);        
        for (int i = 0; i < info.yuePaiLog[0].YuePaiOther.Count; i++)
        {
            YuePaiOther playerInfo = info.yuePaiLog[0].YuePaiOther[i];
            int allRessult = 0;
            for (int j = 0; j < info.yuePaiLog.Count; j++)
            {
                for (int k = 0; k < info.yuePaiLog[j].YuePaiOther.Count; k++)
                {
                    if (info.yuePaiLog[j].YuePaiOther[k].userId == playerInfo.userId)
                        allRessult += info.yuePaiLog[j].YuePaiOther[k].score;
                }

            }
            YuepaiLogPlayerInfo logInfo = new YuepaiLogPlayerInfo();
            logInfo.userId = playerInfo.userId;
            logInfo.nickname = playerInfo.userName;
            logInfo.allResult = allRessult; print(playerInfo.icon);
            logInfo.headIcon = playerInfo.icon;
            playerInfos.Add(logInfo);
        }
        YuepaiLogPlayerInfo maxInfos = ArrayHelper.Max<YuepaiLogPlayerInfo, int>(playerInfos.ToArray(), p => p.allResult);
        maxInfos.isMax = true;
        for (int i = 0; i < playerInfos.Count; i++)
        {
            LoadVerticalResultItem(playerInfos[i]);
        }

        info.yuePaiLog.Sort((a, b) =>
            {
                return a.curr_round.CompareTo(b.curr_round);
            });
        //加载详情
        for (int i = 0; i < info.yuePaiLog.Count; i++)
        {
            YuePaiLog result = info.yuePaiLog[i];
            LoadHorizontalResultItem(result);
        }
        horizontal_parent.spacing = 427 / horizontal_parent.transform.childCount;
        
    }

    /// <summary>
    /// 获取二维码图片
    /// </summary>
    private void GetCodeTextrue()
    {
        Texture2D textrue = QRCode.GetQRTexture(QRCode.GetQRString(3, UserInfoModel.userInfo.downUrl));
        erweimaIcon.sprite = MiscUtils.TextureToSprite(textrue);
    }

    /// <summary>
    /// 加载分享信息
    /// </summary>
    /// <param name="playerInfos"></param>
    /// <param name="info"></param>
    void LoadShareInfos()
    {
        shareInfos.Clear();
        //1.名次排序
        playerInfos.Sort((a, b) =>
            {
                return -a.allResult.CompareTo(b.allResult);
            });

        //2.加载基础信息 name headIcon score rank
        for (int i = 0; i < playerInfos.Count; i++)
        {
            ShareItemInfo itemInfo = new ShareItemInfo();
            itemInfo.name = playerInfos[i].nickname;
            itemInfo.headIcon = playerInfos[i].headIconSpr;
            itemInfo.score = playerInfos[i].allResult;
            itemInfo.rank = i + 1;

            itemInfo.winCount = 0;
            //3.加载赢的局数
            for (int j = 0; j < curYuepaiTable.yuePaiLog.Count; j++)
            {
                YuePaiOther other = curYuepaiTable.yuePaiLog[j].YuePaiOther.Find(p => p.userId == playerInfos[i].userId);
                if (other.score > 0)
                    itemInfo.winCount++;
            }

            shareInfos.Add(itemInfo);
        }

        


    }


    void LoadVerticalResultItem(YuepaiLogPlayerInfo info)
    {
        Instantiate(vertical_prefab, vertical_parent).GetComponent<YuepaiResultItem>().Inits(info);
    }

    void LoadHorizontalResultItem(YuePaiLog resultInfo)
    {
        Instantiate(horizontal_prefab, horizontal_parent.transform).GetComponent<YuepaiHorizontalItem>().Init(resultInfo);
    }
    public Image image;

    /// <summary>
    /// 微信按钮
    /// </summary>
    IEnumerator WeixinBtn()
    {
        LoadShareInfos();
        ShareInfo info = new ShareInfo();
        info.playerInfos = shareInfos;
        info.gameType = curYuepaiTable.type;
        info.allJs = curYuepaiTable.yuePaiLog[curYuepaiTable.yuePaiLog.Count - 1].curr_round;
        info.gameTime = MiscUtils.GetDateTimeByTimeStamp(curYuepaiTable.playat / 1000);
        //判断是否为Android平台  
        if (Application.platform == RuntimePlatform.Android)
        {
            ShareNode node = NodeManager.OpenNode<ShareNode>(null, null, false, false);
            node.Inits(info);

            //等待所有的摄像机和GUI被渲染完成。
            yield return new WaitForEndOfFrame();
            //创建一个空纹理（图片大小为屏幕的宽高）
            Texture2D tex = new Texture2D(Screen.width, Screen.height);
            //只能在帧渲染完毕之后调用（从屏幕左下角开始绘制，绘制大小为屏幕的宽高，宽高的偏移量都为0）
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            //图片应用（此时图片已经绘制完成）
            tex.Apply();

            node.Close();
            tex = MiscUtils.RotateTexture(tex, -90);
            yield return new WaitForSecondsRealtime(0.1f);
            if (tex)
                SDKManager.Instance.ShareImage(SDKManager.WechatShareScene.WXSceneSession, tex.EncodeToJPG(), MiscUtils.SizeTextureBilinear(tex, Vector2.one * 150).EncodeToJPG());
        }
        else
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "当前平台不正确");
        }
    }



    public override void Close(bool isOpenLast = true)
    {
        base.Close(isOpenLast);
        LandlordsNet.LinshiRemoveTable();
        //if (PageManager.Instance.CurrentPage is LandlordsPage)
        PageManager.Instance.OpenPage<MainPage>();
    }

    public class YuepaiLogPlayerInfo
    {
        public int userId;
        public string nickname;
        public string headIcon;
        public Sprite headIconSpr;
        public int allResult;
        public bool isMax = false;
    }
}
