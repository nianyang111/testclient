using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VersionManager : MonoBehaviour {

    static VersionManager instance;
    public static VersionManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<VersionManager>();
                go.name = instance.GetType().ToString();
            }
            return instance;
        }
    }

    #region 登录的时候用
    CallBack _onVersionUpdateFinish;
    public void CheckVersion(CallBack onVersionUpdateFinish)
    {
        _onVersionUpdateFinish = onVersionUpdateFinish;
        LoadingNode.OpenLoadingNode(LoadingType.Progress, "正在检测是否有新版本...", 0);
#if UNITY_EDITOR
        LoadingNode.OpenLoadingNode(LoadingType.Progress, "正在检测是否有新版本...", 1);
        SetTimeout.add(1, () =>
            {
                LoadingNode.CloseLoadingNode();
                if (_onVersionUpdateFinish != null)
                {
                    _onVersionUpdateFinish();
                    _onVersionUpdateFinish = null;
                }
            });
        //StartCoroutine(BigVersionUpdate(true));
#elif UNITY_ANDROID
            StartCoroutine(BigVersionUpdate(true));
#elif UNITY_IPHONE
        onVersionUpdateFinish();
		//StartCoroutine(BigVersionUpdate());
#endif
    }

    string serverVersion;
    public bool MastUpdate()
    {
#if UNITY_EDITOR
        return false;
#else
        bool status = false;
        Debug.Log("serverVersion = " + serverVersion);
        Debug.Log("Application.version = " + Application.version);
        string[] severList = serverVersion.Split('.');
        string[] now = Application.version.Split('.');
        if (serverVersion != Application.version)
        {
            if (int.Parse(severList[0]) > int.Parse(now[0]))
            {
                status = true;
            }
            else if (int.Parse(severList[0]) == int.Parse(now[0]))
            {
                if (int.Parse(severList[1]) > int.Parse(now[1]))
                {
                    status = true;
                }
                else if (int.Parse(severList[1]) == int.Parse(now[1]))
                {
                    if (int.Parse(severList[2]) > int.Parse(now[2]))
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }

                }
                else
                {
                    status = false;
                }
            }
            else
            {
                status = false;
            }
        }
        else
        {
            status = false;
        }
        return status;
#endif
    }

    public IEnumerator BigVersionUpdate(bool isTip)
    {
        WWW www = new WWW(ConstantUtils.urlVersionConfigPath);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            JsonData jds = JsonMapper.ToObject(www.text.Trim());
            string apkUrl = "";

#if UNITY_ANDROID
            serverVersion = jds["android_severVersion"].ToString();
            apkUrl = jds["android_apkUrl"].ToString();
            UserInfoModel.userInfo.downUrl = jds["android_webDownApkUrl"].ToString();
#elif UNITY_IPHONE
   		    serverVersion = jds["ios_curVersion"].ToString();
            apkUrl = jds["ios_apkUrl"].ToString();
            UserInfoModel.userInfo.downUrl = jds["ios_webDownApkUrl"].ToString();
#elif UNITY_EDITOR
		    serverVersion = jds["android_curVersion"].ToString();
            apkUrl = jds["android_apkUrl"].ToString();
            UserInfoModel.userInfo.downUrl = jds["android_webDownApkUrl"].ToString();
#endif
            if (VersionManager.Instance.MastUpdate())
            {
                CallBack call = () =>
                {
#if UNITY_ANDROID
                    StartCoroutine(DownloadAPK(apkUrl));
#elif UNITY_IPHONE
                    Application.OpenURL(apkUrl);
					//IOSCall.Instance.OpenUrl (url);
#endif
                };
                if (isTip)
                    TipManager.Instance.OpenTip(TipType.AlertTip, "检测到版本有更新，请更新！", 0, () =>
                    {
                        call();
                    });
                else
                    call();

            }
            else
            {//如果不需要更新          
                if (File.Exists(Application.persistentDataPath + "/APK" + "/version.apk"))
                {
                    File.Delete(Application.persistentDataPath + "/APK" + "/version.apk");
                }

                SetTimeout.add(2f, () =>
                {
                    LoadingNode.CloseLoadingNode();
                });
            }
        }
    }

    public IEnumerator DownloadAPK(string url)
    {
        WWW www = new WWW(url);
        do
        {
            yield return null;
            LoadingNode.OpenLoadingNode(LoadingType.Progress, "正在下载最新资源包...", www.progress);

        } while (!www.isDone);

        if (www.size > 0)
        {
            string filePath = Application.persistentDataPath + "/APK";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            File.WriteAllBytes(filePath + "/version.apk", www.bytes);
            SDKManager.Instance.Install(filePath + "/version.apk");

        }
        else
        {
            SetTimeout.add(2f, () =>
            {
                LoadingNode.CloseLoadingNode();
                if (_onVersionUpdateFinish!=null)
                {
                    _onVersionUpdateFinish();
                    _onVersionUpdateFinish = null;
                }
            });
        }
    }

    #endregion

    #region 设置界面用
    public IEnumerator BigVersion(CallBack<VersionModel> versionModel)
    {
        WWW www = new WWW(ConstantUtils.urlVersionConfigPath);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            VersionModel jds = JsonMapper.ToObject<VersionModel>(www.text.Trim());
            versionModel(jds);
        }
    }

    public bool CompareVersion(string _severVersion)
    {
        bool status = false;
        Debug.Log("_severVersion = " + _severVersion);
        Debug.Log("Application.version = " + Application.version);
        string[] severList = _severVersion.Split('.');
        string[] now = Application.version.Split('.');
        if (_severVersion != Application.version)
        {
            if (int.Parse(severList[0]) > int.Parse(now[0]))
            {
                status = true;
            }
            else if (int.Parse(severList[0]) == int.Parse(now[0]))
            {
                if (int.Parse(severList[1]) > int.Parse(now[1]))
                {
                    status = true;
                }
                else if (int.Parse(severList[1]) == int.Parse(now[1]))
                {
                    if (int.Parse(severList[2]) > int.Parse(now[2]))
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }

                }
                else
                {
                    status = false;
                }
            }
            else
            {
                status = false;
            }
        }
        else
        {
            status = false;
        }
        return status;
    }

    public IEnumerator DownApk(string url)
    {        
        WWW www = new WWW(url);
        do
        {
            yield return null;
            LoadingNode.OpenLoadingNode(LoadingType.Progress, "正在下载最新资源包...", www.progress);

        } while (!www.isDone);

        if (www.size > 0)
        {
            string filePath = Application.persistentDataPath + "/APK";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            File.WriteAllBytes(filePath + "/version.apk", www.bytes);
            SDKManager.Instance.Install(filePath + "/version.apk");

        }
        else
        {
            SetTimeout.add(2f, () =>
            {
                LoadingNode.CloseLoadingNode();
            });
        }
    }
    #endregion

}

public class VersionModel
{
    public string android_severVersion;
    public string android_apkUrl;
    public string ios_severVersion;
    public string ios_apkUrl;
    public List<VersionConnect> contents;
}

public class VersionConnect
{
    public string connect;
}
