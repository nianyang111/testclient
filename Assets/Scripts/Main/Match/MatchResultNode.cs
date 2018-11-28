using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchResultNode : Node {
    public Text content,rank,reward,date;
    public Image rewardIcon;
    public GameObject haveReward;
    public GameObject noHaveReward;

    public GameObject sharePanel;
    public GameObject backBtn,shareBtn,nextBtn;

    public GameObject weChatBtn, circleBtn;
    public override void Init()
    {
        base.Init();
        UGUIEventListener.Get(backBtn).onClick = delegate { PageManager.Instance.OpenPage<MainPage>(); };
        UGUIEventListener.Get(shareBtn).onClick = delegate { sharePanel.SetActive(!sharePanel.activeInHierarchy); };
        UGUIEventListener.Get(nextBtn).onClick = delegate { PageManager.Instance.OpenPage<MatchPage>(); };
        UGUIEventListener.Get(weChatBtn).onClick = delegate { StartCoroutine(MyCaptureScreen(SDKManager.WechatShareScene.WXSceneSession)); };
        UGUIEventListener.Get(circleBtn).onClick = delegate { StartCoroutine(MyCaptureScreen(SDKManager.WechatShareScene.WXSceneTimeline)); };
    }
    /// <summary>
    /// 分享
    /// </summary>
    /// <param name="shareType"></param>
    IEnumerator MyCaptureScreen(SDKManager.WechatShareScene type)
    {
        //等待所有的摄像机和GUI被渲染完成。
        yield return new WaitForEndOfFrame();
        //创建一个空纹理（图片大小为屏幕的宽高）
        Texture2D tex = new Texture2D(Screen.width, Screen.height);
        //只能在帧渲染完毕之后调用（从屏幕左下角开始绘制，绘制大小为屏幕的宽高，宽高的偏移量都为0）
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //图片应用（此时图片已经绘制完成）
        tex.Apply();
        //将图片装换成jpg的二进制格式，保存在byte数组中（计算机是以二进制的方式存储数据）
        byte[] result = tex.EncodeToJPG();

        SDKManager.Instance.ShareImage(type, result, MiscUtils.SizeTextureBilinear(tex, Vector2.one * 150).EncodeToJPG());
        //文件保存，创建一个新文件，在其中写入指定的字节数组（要写入的文件的路径，要写入文件的字节。）
        //System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/1.JPG", result);

    }

    public override void Open()
    {
        base.Open();
        sharePanel.SetActive(false);
    }
    /// <summary>
    /// 淘汰
    /// </summary>
    public void DieMatcherFinish(net_protocol.DieMatcherPlayerResp resp)//kind 比赛类型 1麻将 2斗地主
    {
        haveReward.SetActive(false);
        noHaveReward.SetActive(true);
        SetResult(resp.rankNum, resp.kind, resp.matcherName, resp.dieNum, resp.time);
    }
    /// <summary>
    /// 成绩
    /// </summary>
    public void MedalFinish(net_protocol.MedalResp resp)
    {
        haveReward.SetActive(true);
        noHaveReward.SetActive(false);
        reward.text = resp.reward[0].name;
        SetResult(resp.rank, resp.kind, resp.matcherName, resp.dieNum, resp.time); 
        StartCoroutine(MiscUtils.DownloadImage(resp.reward[0].icon, (spr) => { if (spr != null) rewardIcon.sprite = spr; }));
    }
    private void SetResult(int rankNum, int kind, string matcherName, int dieNum, long time)
    {
        rank.text = "第" + rankNum + "名";
        string str = kind == 1 ? "麻将" : "斗地主";
        content.text = string.Format("恭喜" + UserInfoModel.userInfo.nickName + "在" + str + matcherName + "中击败" + dieNum + "名对手荣获：");
        System.DateTime dataTime = MiscUtils.GetDateTimeByTimeStamp(time / 1000);
        date.text = string.Format(dataTime.Year + "." + dataTime.Month + "." + dataTime.Day + ".   " + dataTime.Hour + ":" + dataTime.Minute);
    }
}
