using LitJson;
using net_protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class ScannerPage : Page {

    public Text txt;
    public GameObject closeBtn;
    public RawImage cameraTexture;
    private WebCamTexture webCameraTexture;
    bool isScanner;
    public override void Init()
    {
        base.Init();
        UGUIEventListener.Get(closeBtn).onClick = delegate { PageManager.Instance.OpenLastPage(); };
        StartCoroutine(ScannerInit());

    }
    IEnumerator ScannerInit()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            try
            {
                WebCamDevice[] devices = WebCamTexture.devices;
                string devicename = devices[0].name;//0是背摄像头
                webCameraTexture = new WebCamTexture(devicename, 600, 500);
                cameraTexture.texture = webCameraTexture;
                webCameraTexture.Play();
                isScanner = true;
            }
            catch (System.Exception )
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "没有找到摄像机");
            }
        }

    }
   
    public override void Open()
    {
        base.Open();
        InvokeRepeating("ScannerCode", 0f, 0.5f);
    }
    /// <summary>
    /// 检测二维码
    /// </summary>
    void ScannerCode()
    {
        ScreenChange();
        if (!isScanner) return;
        StartCoroutine(ScanQRcode());
    }
    /// <summary>
    /// 屏幕横竖屏切换
    /// </summary>
    void ScreenChange()
    {
        if (webCameraTexture != null)
        {
            if (webCameraTexture.videoRotationAngle == 0)
                cameraTexture.transform.localEulerAngles = Vector3.zero;
            if (webCameraTexture.videoRotationAngle == 180)
                cameraTexture.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
    }
    string scaning = "扫描中";
    IEnumerator ScanQRcode()
    {
        string result = QRCode.DecodeColData(webCameraTexture.GetPixels32(),
                                             webCameraTexture.width,
                                             webCameraTexture.height);

        scaning += ".";
        if (scaning == "扫描中....")
            scaning = "扫描中";
        txt.text = scaning;
        if (result != "")
        {
            webCameraTexture.Stop();
            isScanner = false;
            txt.text = "扫描成功";
            LoadingNode.OpenLoadingNode(LoadingType.Common);
            try
            {
                JsonData jd = JsonMapper.ToObject(result);
                switch (jd.TryGetString("id"))
                {
                    case "1":
                        SocialModel.Instance.AddFriend(int.Parse(jd.TryGetString("content")));
                        TipManager.Instance.OpenTip(TipType.SimpleTip, "发送成功，等待通过", 1f);
                        break;
                    case "2":
                        SocketClient.Instance.AddSendMessageQueue(new C2GMessage{msgid = MessageId.C2G_QueryTableInfo,
                         queryTableInfo = new QueryTableInfo(){tableId = jd.TryGetString("content")}});
                        break;
                    case "3":
                        Application.OpenURL(jd.TryGetString("content"));
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception )
            {
                TipManager.Instance.OpenTip(TipType.SimpleTip, "未识别的二维码");
            }
            PageManager.Instance.OpenLastPage();
        }
        yield return new WaitForEndOfFrame();
    }

    public override void Close()
    {
        base.Close();
        Destroy(webCameraTexture);
        webCameraTexture = null;
    }
}
