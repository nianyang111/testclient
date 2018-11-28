using net_protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidSDKManager : MonoBehaviour
{
    public bool isRegisterToWechat = false;
    static AndroidSDKManager instance;
    AndroidJavaObject currentActivity;

    public static AndroidSDKManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<AndroidSDKManager>();
                go.name = instance.GetType().ToString();
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                instance.currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                instance.RegisterAppWechat();
            }
            return instance;
        }
    }

    #region ...wechat
    const string WXAppID = "wxedd32cd0cd48e37a";
    //const string WXPaySecret = "6310aa9a9fa596d60c7a11478c45d32f";
    //const string WXShopId = "1488194022";

    /// <summary> 注册微信 </summary>
    public void RegisterAppWechat()
    {
        if (!isRegisterToWechat)
        {
            AndroidJavaClass utils = new AndroidJavaClass("com.xueyaokeji.xyms.wechat.WechatTool");
            utils.CallStatic("RegisterToWechat", currentActivity);
            isRegisterToWechat = true;
        }
    }

    /// <summary> 是否安装了微信 </summary>
    public bool IsWechatInstalled()
    {
        AndroidJavaClass utils = new AndroidJavaClass("com.xueyaokeji.xyms.wechat.WechatTool");
        return utils.CallStatic<bool>("IsWechatInstalled");
    }

    #region ...登录
    /// <summary> 微信登录 </summary>
    public void WechatLogin()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass utils = new AndroidJavaClass("com.xueyaokeji.xyms.wechat.WechatLogin");
        utils.CallStatic("LoginWeChat", "app_wechat");//后期改为随机数加session来校验
    }

    /// <summary> 微信登录回调 </summary>
    public void LoginCallBack(string userInfo)
    {
        //openid	 普通用户的标识，对当前开发者帐号唯一
        //nickname	 普通用户昵称
        //sex	     普通用户性别，1为男性，2为女性
        //province	 普通用户个人资料填写的省份
        //city	     普通用户个人资料填写的城市
        //country	 国家，如中国为CN
        //headimgurl 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        //privilege	 用户特权信息，json数组，如微信沃卡用户为（chinaunicom）
        //unionid	 用户统一标识。针对一个微信开放平台帐号下的应用，同一用户的unionid是唯一的。多app数据互通保存该值
        if (userInfo.IndexOf("errcode") < 0)
        {
            LoginPage lp = PageManager.Instance.GetPage<LoginPage>();
            if (lp)
                lp.Login(userInfo);
        }
        else
            TipManager.Instance.OpenTip(TipType.SimpleTip, "授权失败,请重试");
    }
    #endregion

    #region ...支付

    /// <summary> 发送支付请求 </summary>
    public void SendWechatPay(UnifiedOrderResp ufo)
    {
        AndroidJavaClass utils = new AndroidJavaClass("com.xueyaokeji.xyms.wechat.WechatPay");
        utils.CallStatic("SendPay", ufo.appId, ufo.partnerid, ufo.prepayid, ufo.noncestr, ufo.timestamp, ufo.package, ufo.sign);
    }

    /// <summary> 支付回调 </summary>
    public void WechatPayCallback(string retCode)
    {
        switch (int.Parse(retCode))
        {
            case -2:
                TipManager.Instance.OpenTip(TipType.SimpleTip, "支付取消");
                break;
            case -1:
                TipManager.Instance.OpenTip(TipType.SimpleTip, "支付失败");
                break;
            case 0:
                TipManager.Instance.OpenTip(TipType.SimpleTip, "支付成功");
                break;
        }
    }
    #endregion

    #endregion

    #region ...alipay

    public void SendAliPay(AliOrderResp aop)
    {
        AndroidJavaObject utils = new AndroidJavaObject("com.xueyaokeji.xyms.alipay.AliPay");
        utils.CallStatic("SendPay", aop.info, currentActivity);
    }

    /// <summary> 支付回调 </summary>
    public void AliPayCallback(string palySuccess)
    {
        TipManager.Instance.OpenTip(TipType.SimpleTip, bool.Parse(palySuccess) ? "支付成功" : "支付失败");
    }
    #endregion

    #region 文本

    const string Utils = "com.xueyaokeji.xyms.utils.Utils";

    /// <summary> 复制到剪贴板 </summary>
    public void CopyToClipboard(string input)
    {
        AndroidJavaObject androidObject = new AndroidJavaObject(Utils);
        AndroidJavaObject cur_activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        androidObject.CallStatic("CopyTextToClipboard", cur_activity, input);
    }

    /// <summary>取出剪贴板内容</summary>
    public string GetFromClipboard()
    {
        AndroidJavaObject androidObject = new AndroidJavaObject(Utils);
        AndroidJavaObject cur_activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        string clipBoardStr = androidObject.CallStatic<string>("GetTextFromClipboard", cur_activity);
        return clipBoardStr;
    }
    #endregion

    #region 其他功能

    /// <summary>得到电量</summary>
    public int GetBattery()
    {
        AndroidJavaObject androidObject = new AndroidJavaObject(Utils);
        AndroidJavaObject cur_activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        return androidObject.CallStatic<int>("GetBattery", cur_activity);
    }


    /// <summary>截屏</summary>
    public Texture2D ScrrenShoot(string photoName)
    {
        //获取系统时间并命名相片名  
        System.DateTime now = System.DateTime.Now;
        string times = now.ToString();
        times = times.Trim();
        times = times.Replace("/", "-");
        string filename = photoName + times + ".png";
        //判断是否为Android平台  
        if (Application.platform == RuntimePlatform.Android)
        {
            Texture2D t2d = UIUtils.PrintScreen(new Rect(0, 0, Screen.width, Screen.height), Vector2.zero);
            string destination = "/storage/emulated/0/DCIM/Burst";
            //判断目录是否存在，不存在则会创建目录  
            if (!System.IO.Directory.Exists(destination))
            {
                System.IO.Directory.CreateDirectory(destination);
            }
            string Path_save = destination + "/" + filename;
            //存图片  
            System.IO.File.WriteAllBytes(Path_save, t2d.EncodeToPNG());
            AndroidJavaObject cur = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject jo = new AndroidJavaObject("com.xueyaokeji.xyms.photo.Photo");
            jo.CallStatic("UpdateAlbum", cur);
            TipManager.Instance.OpenTip(TipType.SimpleTip, "截图成功");
            return t2d;
        }
        else
        {
            Texture2D t2d = UIUtils.PrintScreen(new Rect(0, 0, Screen.width, Screen.height), Vector2.zero);
            return t2d;
        }
        //return null;
    }

    public void ScrrenShoot(string fileName, string albumName = "MyScreenshots", bool callback = false)
    {
        StartCoroutine(ScreenshotManager.Save(fileName, albumName, callback));
    }

    CallBack _onPhotoSuccessCall;    
    public void OpenPhoto(string iconName, CallBack onPhotoSuccessCall = null)
    {
        _onPhotoSuccessCall = onPhotoSuccessCall;
        AndroidJavaObject cur = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject jo = new AndroidJavaObject("com.xueyaokeji.xyms.photo.Photo");
        jo.CallStatic("SettingAvaterFormMobile", cur, name, "OpenPhotoResult", iconName);
    }

    // android系统相册回调
    public void OpenPhotoResult(string result)
    {
        Debug.Log("返回:" + result);
        if (_onPhotoSuccessCall != null)
        {
            _onPhotoSuccessCall();
            _onPhotoSuccessCall = null;
        }
    }

    public void InstallAPK(string file)
    {
        LoadingNode.OpenLoadingNode(LoadingType.Progress, "正在安装新版本中...");      
        AndroidJavaObject jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("installApk", file);
    }

    #endregion

    #region 分享    

    const string WeChatShareUtils = "com.xueyaokeji.xyms.wechat.ShareUtils";

    public void ShareImage(SDKManager.WechatShareScene scene, byte[] data, byte[] dataThumb)
    {
        AndroidJavaClass utils = new AndroidJavaClass(WeChatShareUtils);
        utils.CallStatic("ShareImage", (int)scene, data, dataThumb);
    }

    public void ShareText(SDKManager.WechatShareScene scene, string content)
    {
        AndroidJavaClass utils = new AndroidJavaClass(WeChatShareUtils);
        utils.CallStatic("ShareText", (int)scene, content);
    }

    public void ShareWebPage(SDKManager.WechatShareScene scene, string url, string title, string content, byte[] thumb)
    {
        AndroidJavaClass utils = new AndroidJavaClass(WeChatShareUtils);
        utils.CallStatic("ShareWebPage", (int)scene, url, title, content, thumb);
    }
    public void OpenWeChat()
    {
        AndroidJavaClass utils = new AndroidJavaClass(WeChatShareUtils);
        utils.CallStatic("OpenWeCaht");
    }
    public void WechatCallBack(string errCode)
    {
        WechatErrCode code = (WechatErrCode)int.Parse(errCode);
        switch (code)
        {
            case WechatErrCode.Success:
                TipManager.Instance.OpenTip(TipType.SimpleTip, "分享成功");
                SocketClient.Instance.AddSendMessageQueue(new net_protocol.C2GMessage()
                {
                    Complete1Task = new net_protocol.Complete1Task()
                    {
                        taskType = 3
                    },
                    msgid = MessageId.C2G_Complete1Task
                });
                TaskNode.ReqTasks();
                break;
            default:
                TipManager.Instance.OpenTip(TipType.SimpleTip, "分享失败");
                break;
        }
    }


    #endregion

}

public enum WechatErrCode
{
    Success = 0,
    ErrorCommon = -1,
    ErrorUserCancel = -2,
    ErrorSentFail = -3,
    ErrorAuthDenied = -4,
    ErrorUnsupport = -5,
    ErrorBan = -6,
}
