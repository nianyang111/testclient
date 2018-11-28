using LitJson;
using net_protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoginPage : Page
{
    public static bool isLoadAssets = false;
    public GameObject btnLogin;
    public InputField inputF;
    public Toggle mainToggle, isWechatLogin;
    public static string token;
    public List<Toggle> IPtoggles;
    public InputField ipInput;
    public GameObject textBtnLogin;
    public override void Init()
    {
        base.Init();
        PageManager.Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        StartCoroutine(VersionUpdate());
        ExportLoginData();
        SetNode.ExpireSet();
        SetNode.ForAudio();
        SetNode.SetAudio();
        FloatBallManager.Instance.Hide();
        UGUIEventListener.Get(btnLogin).onClick = delegate { Login(); };
        InitTextIpChoose();        
    }

    public void Login(string userInfo = null)
    {
        if (string.IsNullOrEmpty(userInfo))
        {
            if (!mainToggle.isOn)
            {
                TipManager.Instance.OpenTip(TipType.ChooseTip, "请勾选同意用户协议，才可进入游戏");
                return;
            }
#if UNITY_ANDROID
            if (!VersionManager.Instance.MastUpdate())
            {
                AndroidSDKManager.Instance.WechatLogin();
            }
            else
            {
                TipManager.Instance.OpenTip(TipType.AlertTip, "检测到版本更新,请更新", 0, () =>
                {
                    StartCoroutine(VersionManager.Instance.BigVersionUpdate(false));
                });


            }
#elif UNITY_IPHONE
	       if (!VersionManager.Instance.MastUpdate()) 
           {
               SDKManager.Instance.WechatLogin();
	       }
	       else
	       {
		TipManager.Instance.OpenTip(TipType.AlertTip, "检测到版本跟新，请前往网官网下载更新", 0, () =>
		   {
				//IOSCall.Instance.OpenUrl(iosLoadUrl);
			});
		
	       }
#endif

        }
        else
        {
            JsonData jd = JsonMapper.ToObject(userInfo);
            SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
            {
                msgid = MessageId.C2G_Login,
                login = new Login()
                {
                    channel = 1,
                    name = jd.TryGetString("nickname"),
                    uuid = jd.TryGetString("openid"),
                    icon = jd.TryGetString("headimgurl"),
                    gender = int.Parse(jd.TryGetString("sex"))
                }
            }, true);
        }
    }

    /// <summary>
    /// 存账号信息
    /// </summary>
    /// <param name="account"></param>
    void SaveLoginData(string account)
    {
        StringBuilder sb = new StringBuilder("{");
        sb.Append("\"account\":\"" + account + "\"}");
        MiscUtils.CreateTextFile(ConstantUtils.loginConfigPath, sb.ToString());
    }

    /// <summary>
    /// 取账号信息
    /// </summary>
    void ExportLoginData()
    {
        if (File.Exists(ConstantUtils.loginConfigPath))
        {
            JsonData json = MiscUtils.GetJsonFromPath(ConstantUtils.loginConfigPath);
            inputF.text = json["account"].ToString();
        }
    }

    /// <summary>
    /// 登录回调
    /// </summary>
    /// <param name="token"></param>
    public static void LoginResult(string token)
    {
        SaveToken(token);
        LoginPage.token = token;
        PageManager.Instance.OpenPage<MainPage>();
    }

    /// <summary>
    /// 保存token信息到本地
    /// </summary>
    static void SaveToken(string token)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{\"token\":\"" + token);
        sb.Append("\",\"time\":\"" + DateTime.Now);
        sb.Append("\"}");
        MiscUtils.CreateTextFile(ConstantUtils.tonkenConfigPath, sb.ToString());

    }
    /// <summary>
    /// 获取登录Token文件是否存在
    /// </summary>
    /// <returns></returns>
    public static bool TokenExist()
    {
        if (File.Exists(ConstantUtils.tonkenConfigPath))
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "正在重新登陆");
            JsonData jd = JsonMapper.ToObject(File.ReadAllText(ConstantUtils.tonkenConfigPath));
            token = jd.TryGetString("token");
            DateTime tokenRecordTime = DateTime.Parse(jd.TryGetString("time"));
            return (DateTime.Now - tokenRecordTime).TotalSeconds > 60 * 60 * 24 * 3 - 3600;//超过2天23小时即过期
        }
        return !string.IsNullOrEmpty(token);
    }
    /// <summary>
    /// 清除登录信息
    /// </summary>
    public static void DeleteLoginInfo()
    {
        LoginPage.token = null;
        UserInfoModel.userInfo = null;
        if (File.Exists(ConstantUtils.tonkenConfigPath))
            File.Delete(ConstantUtils.tonkenConfigPath);
    }
    /// <summary>
    /// 尝试token登录
    /// </summary>
    /// <returns></returns>
    public static IEnumerator TryTokenLogin()
    {
        yield return new WaitForEndOfFrame();
        if (!TokenExist())
        {
            if (token != null)
                SocketClient.Instance.AddSendMessageQueue(new C2GMessage
                {
                    login = new Login() { token = token },
                    msgid = MessageId.C2G_Login
                }, true);
        }
        else
        {
            LoadingNode.CloseLoadingNode();
        }
    }

    /// <summary>
    /// 版本/文件更新
    /// </summary>
    /// <returns></returns>
    IEnumerator VersionUpdate()
    {
        WWW www = new WWW(ConstantUtils.bundleTipsUrl);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            List<BundleManager.BundleInfo> bmbis = new List<BundleManager.BundleInfo>();
            JsonData jds = JsonMapper.ToObject(www.text);
            string md5, file, path;
            for (int i = 0; i < jds.Count; i++)
            {
                JsonData jd = jds[i];
                file = jd.TryGetString("file");
                path = ConstantUtils.AssetBundleFolderPath + file;
                md5 = MiscUtils.GetMD5HashFromFile(path);
                if (string.IsNullOrEmpty(md5) || md5 != jd.TryGetString("md5"))
                {
                    bmbis.Add(new BundleManager.BundleInfo()
                    {
                        _url = ConstantUtils.bundleDownLoadUrl + file,
                        _path = path
                    });
                }
            }
            if (bmbis.Count > 0)
            {
                StartCoroutine(BundleManager.Instance.DownloadBundleFiles(bmbis,
                    (progress) =>
                    {
                        LoadingNode.OpenLoadingNode(LoadingType.Progress, "自动更新中...", progress);
                    },
                    (isFinish) =>
                    {
                        if (isFinish)
                        {
                            OnUpdateFileComplete();
                        }
                        else
                        {
                            TipManager.Instance.OpenTip(TipType.SimpleTip, "部分文件更新失败，正在重试...");
                            StartCoroutine(VersionUpdate());
                        }
                    }));
            }
            else
            {
                OnUpdateFileComplete();
            }
        }
        else
        {
            UIUtils.Log(www.error);
            VersionManager.Instance.CheckVersion(OnUpdateBigFileComplete);
        }
    }
    
    //小版本更新结束
    void OnUpdateFileComplete()
    {
        LoadNecessaryBundle();
        VersionManager.Instance.CheckVersion(OnUpdateBigFileComplete);
    }

    void LoadNecessaryBundle()
    {
        if (!isLoadAssets)
        {
            isLoadAssets = true;
            PageManager.Instance.gamecommonBundle = BundleManager.Instance.GetSpriteBundle("gamecommon");
            //BundleManager.Instance.GetBundle("sprite/common");
            AssetBundle ab = BundleManager.Instance.GetBundle("shader/imagetogrey");
            Material mat = ab.LoadAsset<Material>("imagetogrey");
            mat.shader = Shader.Find(mat.shader.name);
        }
    }

    #region 大版本更新     

    //完成全部更新  走到这说明版本更新完毕并且当前是最新的版本
    void OnUpdateBigFileComplete()
    {
        Debug.LogWarning("所有版本检测完毕");
        //if (!PageManager.Instance.isDebugLog)
        //StartCoroutine(TryTokenLogin());
    }

    #endregion

    /// <summary>
    /// 被顶号
    /// </summary>
    public static void TakeLogin()
    {
        DeleteLoginInfo();
        PageManager.Instance.OpenPage<LoginPage>();
        TipManager.Instance.OpenTip(TipType.SimpleTip, "你的账号在其他设备上登录，请重新登录");
    }

    void InitTextIpChoose()
    {
        inputF.gameObject.SetActive(PageManager.Instance.isDebugLog);
        ipInput.gameObject.SetActive(PageManager.Instance.isDebugLog);
        textBtnLogin.SetActive(PageManager.Instance.isDebugLog);
        for (int i = 0; i < IPtoggles.Count; i++)
            IPtoggles[i].gameObject.SetActive(PageManager.Instance.isDebugLog);

        if (PageManager.Instance.isDebugLog)
        {
            UGUIEventListener.Get(textBtnLogin).onClick = delegate
            {
                if (!VersionManager.Instance.MastUpdate())
                {
                    SetTestIp();

                    SocketClient.Instance.AddSendMessageQueue(new C2GMessage()
                    {
                        msgid = MessageId.C2G_Login,
                        login = new Login()
                        {
                            channel = 1,
                            name = inputF.text,
                            uuid = inputF.text,
                            icon = "http://www.qq22.com.cn/uploads/allimg/201608181022/iapjaicgs3b94560.jpg",
                            gender = 1
                        }
                    }, true);
                    SaveLoginData(inputF.text);
                }
                else
                {
                    TipManager.Instance.OpenTip(TipType.AlertTip, "检测到版本更新,请更新", 0, () =>
                    {
                        StartCoroutine(VersionManager.Instance.BigVersionUpdate(false));
                    });
                }
            };
            for (int i = 0; i < IPtoggles.Count; i++)
            {
                if (IPtoggles[i].name.CompareTo(PageManager.Instance.ipIndex.ToString()) == 0)
                {
                    IPtoggles[i].isOn = true;
                    break;
                }
            }
        }
    }

    void SetTestIp()
    {
        if (PageManager.Instance.isDebugLog)
        {
            if (string.IsNullOrEmpty(ipInput.text))
            {
                int index = int.Parse(IPtoggles.Find(p => p.isOn).name);
                PageManager.Instance.ipIndex = index;
            }
            else
            {
                PageManager.Instance.ips.Add(ipInput.text);
                PageManager.Instance.ipIndex = PageManager.Instance.ips.Count - 1;
            }
        }
    }
}
