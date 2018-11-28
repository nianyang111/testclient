using net_protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKManager
{

    #region 单例
    static SDKManager _instance;

    public static SDKManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SDKManager();
                ScreenshotManager.ScreenshotFinishedSaving += delegate { TipManager.Instance.OpenTip(TipType.SimpleTip, "截图成功,请到相册查看"); };
            }
            return _instance;
        }
    }

    #endregion

    /// <summary>
    /// 注册微信
    /// </summary>
    public void RegisterAppWechat()
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.RegisterAppWechat();
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.RegisterAppWechat();
#endif
    }

    /// <summary> 是否安装了微信 </summary>
    public bool IsWechatInstalled()
    {
#if UNITY_IPHONE
        return IOSSDKManager.Instance.IsWechatInstalled();
#elif UNITY_ANDROID
        return AndroidSDKManager.Instance.IsWechatInstalled();
#endif
    }

    #region ...登录
    /// <summary> 微信登录 </summary>
    public void WechatLogin()
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.WechatLogin();
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.WechatLogin();
#endif
    }


    #region ...支付

    /// <summary> 创建新的支付请求 </summary>
    public void CreateNewWechatPayOrder(int goodsId, int count = 1)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_UnifiedOrder,
            unifiedOrderReq = new UnifiedOrderReq()
            {
                goods_id = goodsId,
                count = count,
                payType = 2 //1-支付宝，2-微信
            }
        });
    }

    /// <summary> 发送支付请求 </summary>
    public void SendWechatPay(UnifiedOrderResp ufo)
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.SendWechatPay(ufo);
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.SendWechatPay(ufo);
#endif
    }

    #endregion

    #region ...alipay
    public void CreateNewAliPayOrder(int goodsId, int count = 1)
    {
        SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
        {
            msgid = MessageId.C2G_UnifiedOrder,
            unifiedOrderReq = new UnifiedOrderReq()
            {
                goods_id = goodsId,
                count = count,
                payType = 1 //1-支付宝，2-微信
            }
        });
    }

    public void SendAliPay(AliOrderResp aop)
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.SendAliPay(aop);
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.SendAliPay(aop);
#endif
    }

    #endregion
    #endregion

    #region 分享
    public enum WechatShareScene
    {
        // 好友
        WXSceneSession = 0,
        // 朋友圈
        WXSceneTimeline = 1,
        // 收藏
        WXSceneFavorite = 2,
    }
    /// <summary>
    /// 分享图片
    /// </summary>
    /// <param name="scene">分享的场景</param>
    /// <param name="data">分享的图片，jpg格式</param>
    /// <param name="dataThumb">图片缩略图，jpg格式，需先压缩至150及以下，质量低于50最佳</param>
    public void ShareImage(WechatShareScene scene, byte[] data, byte[] dataThumb)
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.ShareImage(scene, data, dataThumb);
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.ShareImage(scene, data, dataThumb);
#endif
    }

    /// <summary>
    /// 分享文本
    /// </summary>
    /// <param name="scene">分享的场景</param>
    /// <param name="content">分享的文本内容</param>
    public void ShareText(WechatShareScene scene, string content)
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.ShareText(scene, content);
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.ShareText(scene, content);
#endif
    }

    /// <summary>
    /// 分享链接
    /// </summary>
    /// <param name="scene">分享的场景</param>
    /// <param name="url">分享的链接地址</param>
    /// <param name="title">分享链接的标题</param>
    /// <param name="content">分享链接的文本描述</param>
    /// <param name="thumb">缩略图，jpg格式，需先压缩至150及以下，质量低于50最佳</param>
    public void ShareWebPage(WechatShareScene scene, string url, string title, string content, byte[] thumb)
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.ShareWebPage(scene, url, title, content, thumb);
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.ShareWebPage(scene, url, title, content, thumb);
#endif
    }
    /// <summary>
    /// 打开微信
    /// </summary>
    public void OpenWeChat()
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.OpenWeChat();
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.OpenWeChat(); 
#endif
    }
    #endregion

    #region 其他

    /// <summary> 复制到剪贴板 </summary>
    public void CopyToClipboard(string input)
    {
#if UNITY_ANDROID
        AndroidSDKManager.Instance.CopyToClipboard(input);
#elif UNITY_IPHONE
        IOSSDKManager.Instance.CopyToClipboard(input);
#elif UNITY_EDITOR  
        TextEditor t = new TextEditor();
        t.text = input;  
        t.OnFocus();  
        t.Copy();  
#endif
    }

    /// <summary>取出剪贴板内容</summary>
    public string GetFromClipboard()
    {
        string clipBoardStr = string.Empty;
#if UNITY_IPHONE
        clipBoardStr = IOSSDKManager.Instance.GetFromClipboard();
#elif UNITY_ANDROID
        clipBoardStr = AndroidSDKManager.Instance.GetFromClipboard();
#elif UNITY_EDITOR
        clipBoardStr = GUIUtility.systemCopyBuffer;        
#endif
        return clipBoardStr;
    }

    /// <summary>得到电量</summary>
    public int GetBattery()
    {
#if UNITY_IPHONE
        return IOSSDKManager.Instance.GetBattery();
#elif UNITY_ANDROID
        return AndroidSDKManager.Instance.GetBattery();
#else
        return 0;
#endif
    }

    /// <summary> 截屏保存到相册 </summary>
    public void ScrrenShoot(string fileName, string albumName = "MyScreenshots", bool callback = false)
    {
        PageManager.Instance.StartCoroutine(ScreenshotManager.Save(fileName, albumName, callback));
    }

    // 打开相册 选择图片   图片最终会在 "file://" + Application.persistentDataPath + "/" + name
    public void OpenPhoto(string iconName, CallBack onPhotoSuccessCall = null)
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.OpenPhoto(iconName, onPhotoSuccessCall);
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.OpenPhoto(iconName, onPhotoSuccessCall);
#endif
    }

    // 安装APK
    public void Install(string file)
    {
#if UNITY_IPHONE
        IOSSDKManager.Instance.OpenUrl(file);
#elif UNITY_ANDROID
        AndroidSDKManager.Instance.InstallAPK(file);
#endif
    }


    #endregion

}
