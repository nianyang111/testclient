using net_protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class IOSSDKManager : MonoBehaviour
{
    static IOSSDKManager instance;

    public static IOSSDKManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<IOSSDKManager>();
                go.name = instance.GetType().ToString();
            }
            return instance;
        }
    }


    #region ...wechat
    const string WXAppID = "wxedd32cd0cd48e37a";
    public void RegisterAppWechat()
    {
        //_RegisterApp (WechatAppId);
    }

    public bool IsWechatInstalled()
    {
        return false; //_IsWechatInstalled ();
    }

    #region ...登录
    public void WechatLogin()
    {
    }

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

    public void SendWechatPay(UnifiedOrderResp ufo)
    {

    }

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

    #region 支付宝

    public void SendAliPay(AliOrderResp aop)
    {
    }

    public void AliPayCallback(string palySuccess)
    {
        TipManager.Instance.OpenTip(TipType.SimpleTip, bool.Parse(palySuccess) ? "支付成功" : "支付失败");
    }

    #endregion

    #region 分享

    #endregion 

    #region 其他
    [DllImport("__Internal")]
    private static extern void _copyTextToClipboard(string text);
    [DllImport("__Internal")]
    private static extern float getiOSBatteryLevel();
    [DllImport("__Internal")]
    private static extern void openUrl(string text);

    public void CopyToClipboard(string input)
    {
        _copyTextToClipboard(input);  
    }

    public string GetFromClipboard()
    {
        string clipBoardStr = "";
        //clipBoardStr = _copyTextToClipboard(input);  
        return clipBoardStr;
    }

    public int GetBattery()
    {
        return (int)getiOSBatteryLevel();
    }
    CallBack _onPhotoSuccessCall;  
    public void OpenPhoto(string iconName, CallBack onPhotoSuccessCall = null)
    {
        _onPhotoSuccessCall = onPhotoSuccessCall;
    }

    public void OpenPhotoResult(string result)
    {
        Debug.Log("返回:" + result);
        if (_onPhotoSuccessCall != null)
        {
            _onPhotoSuccessCall();
            _onPhotoSuccessCall = null;
        }
    }

    public void OpenUrl(string url)
    {
        openUrl(url);
    }

    #region 分享

    [DllImport("__Internal")]
    static extern void _RegisterApp(string appId);
    [DllImport("__Internal")]
    static extern void _ShareImageWechat(int scene, IntPtr ptr, int size, IntPtr ptrThumb, int sizeThumb);
    [DllImport("__Internal")]
    static extern bool _IsWechatInstalled();
    [DllImport("__Internal")]
    static extern bool _IsWechatAppSupportApi();


    public void ShareImage(SDKManager.WechatShareScene scene, byte[] data, byte[] dataThumb)
    {
        IntPtr array = Marshal.AllocHGlobal(data.Length);
        Marshal.Copy(data, 0, array, data.Length);
        IntPtr arrayThumb = Marshal.AllocHGlobal(dataThumb.Length);
        Marshal.Copy(dataThumb, 0, arrayThumb, dataThumb.Length);
        _ShareImageWechat((int)scene, array, data.Length, arrayThumb, dataThumb.Length);
    }


    public void ShareText(SDKManager.WechatShareScene scene, string content)
    {
        
    }

    public void ShareWebPage(SDKManager.WechatShareScene scene, string url, string title, string content, byte[] thumb)
    {
    }
    public void OpenWeChat()
    {

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
    #endregion
}
