using UnityEngine;
using System.Collections.Specialized;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using net_protocol;

public class PageManager : MonoBehaviour
{
    public static PageManager Instance;
    public Canvas canvas;
    public List<string> ips = new List<string> { "192.168.10.101", "192.168.10.147", "192.168.10.148", "192.168.10.170", "192.168.10.104" };
    public int ipIndex;
    public int testLoginIds = 1;
    public bool isDebugLog;
    public bool isCanSend = true;
    public bool isLocalVersion;

    public static float canvasScale;
    public static int pixelWidth;
    public static int pixelHeight;
    // 当前Page
    public Page CurrentPage { get; set; }
    public static List<AssetBundle> spriteABList = new List<AssetBundle>();
    // Page历史，用于返回时检索
    static List<string> _pageHistory = new List<string>();
    public AssetBundle gamecommonBundle;

    void Awake()
    {
        Vector2 vec2 = GetComponent<CanvasScaler>().referenceResolution;
        pixelWidth = (int)vec2.x;
        pixelHeight = (int)vec2.y;
        canvasScale = Screen.width * 1f / pixelWidth;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 30;
        Application.runInBackground = true;
        if (Instance == null)
            Instance = this;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            GVoice.Instance.Click_btnReqAuthKey();//注册语音
        Screen.orientation = ScreenOrientation.Landscape;
    }

    void Start()
    {
        UIUtils.Log(Application.persistentDataPath);
        //SetNode.ExpireSet();
        Loom.Initialize();
        OpenPage<LoginPage>();
        //OpenPage<MainPage>();
        //LoadingNode.OpenLoadingNode(LoadingType.Common);        
    }
    void Update()
    {
#if UNITY_ANDROID
        InputUtils.GetTouchEscape();
#endif
    }

    void OnDestroy()
    {
        Instance = null;
        gamecommonBundle.Unload(false);
    }

    /// <summary>
    /// 打开特定类型的Page
    /// </summary>
    /// <param name="pagePath"></param>
    /// <param name="finishCallback"></param>
    Page OpenPageAc(string pagePath, Action finishCallback = null)
    {
        Page page = CreatePage(pagePath);
        if (page != null)
        {
            DestroyCurrentPage(page.name);
            CurrentPage = page;
            page.Open();

            if (finishCallback != null)
                finishCallback();
        }
        StartCoroutine(DelayLoadMusic(pagePath));
        return page;
    }

    IEnumerator DelayLoadMusic(string pagePath)
    {
        AudioManager.Instance.PauseAllSound();
        yield return new WaitForSecondsRealtime(0.5f);
        AudioManager.Instance.PlayMusic(MiscUtils.GetFileName(pagePath).ToLower(), MiscUtils.GetFileName(pagePath));
        AudioManager.Instance.ResumeAllSound();
    }

    /// <summary>
    /// 销毁当前的Page并刷新历史
    /// </summary>
    /// <param name="pageName"></param>
    void DestroyCurrentPage(string pageName)
    {
        if (CurrentPage != null)
            DestroyPage(CurrentPage);
        RefreshHistory(pageName);
    }

    /// <summary>
    /// 销毁指定Page
    /// </summary>
    /// <param name="page"></param>
    void DestroyPage(Page page)
    {
        page.Close();
        if (CurrentPage.GetSpriteAB() != null)
        {
            PageManager.spriteABList.Remove(CurrentPage.GetSpriteAB());
            CurrentPage.GetSpriteAB().Unload(true);
        }
        AudioManager.Instance.ClearAllTempAudio();
        Destroy(page.gameObject);
        GC.Collect();
    }

    /// <summary>
    /// 创建一个新的特定类型的Page
    /// </summary>
    /// <param name="pageName"></param>
    /// <returns></returns>
    Page CreatePage(string pageName)
    {
        if (string.IsNullOrEmpty(pageName))
            return null;
        if (CurrentPage != null && pageName == CurrentPage.name)
            return null;

        Page page = GetComponentInChildren<Page>(true);
        if (page == null || page.name != pageName)
        {
            AssetBundle ab = BundleManager.Instance.GetSpriteBundle("mainpic", MiscUtils.GetFileName(pageName));
            if (ab != null)
                spriteABList.Add(ab);
            string path = "pages/" + pageName;
            GameObject go = BundleManager.Instance.GetGameObject(path, MiscUtils.GetFileName(pageName));
            if (go)
            {
                UIUtils.AttachAndReset(go, transform);
                page = go.GetComponent<Page>();
                page.InitData(ab);
            }
            else
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "加载界面失败");
                return null;
            }
        }
        page.transform.SetAsFirstSibling();
        return page;
    }

    /// <summary>
    /// 刷新Page历史
    /// </summary>
    /// <param name="type"></param>
    void RefreshHistory(string type)
    {
        int index = _pageHistory.IndexOf(type);
        if (index >= 0)
            _pageHistory.RemoveRange(index, _pageHistory.Count - index);
        else
            if (CurrentPage)
                _pageHistory.Add(CurrentPage.name);
    }

    /// <summary>
    /// 根据Page子类类型打开特定的Page
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public T OpenPage<T>(Action finishCallback = null) where T : Page
    {
        return (T)OpenPageAc(GetPageType<T>(), finishCallback);
    }

    public Page OpenPage(string pageName, Action finishCallback = null)
    {
        return OpenPageAc(pageName, finishCallback);
    }

    public T GetPage<T>() where T : Page
    {
        T page = GetComponentInChildren<T>();
        return page;
    }

    string GetPageType<T>() where T : Page
    {
        return typeof(T).ToString();
    }

    // 返回上一个Page
    public void OpenLastPage()
    {
        if (_pageHistory.Count > 0)
        {
            string page = _pageHistory[_pageHistory.Count - 1].ToString();
            OpenPageAc(page.ToString());
        }
    }


    //bool isHavaFocus = false;
    void OnApplicationFocus()
    {
        //isHavaFocus = !isHavaFocus;
        //if(isHavaFocus)
        //{
        JoinGameRoonNode.JoinGameOnFocus();
        //}       
    }
}
