using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class VersionNode : Node {

    public Text prefab;

    public Transform parent;

    public GameObject updateBtn;

    VersionModel _model;
    public override void Open()
    {
        base.Open();
        UGUIEventListener.Get(updateBtn).onClick = delegate { RefreshGame(); };
    }

    public void Inits(VersionModel model)
    {
        _model = model;
        UIUtils.DestroyChildren(parent);
        for (int i = 0; i < model.contents.Count; i++)
        {
            Text text = Instantiate(prefab, parent).GetComponent<Text>();
            text.text = model.contents[i].connect;
            text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight);
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    void RefreshGame()
    {
        if (File.Exists(Application.persistentDataPath + "/APK" + "/version.apk"))
        {
            SDKManager.Instance.Install(Application.persistentDataPath + "/APK" + "/version.apk");
        }
        else
        {
#if UNITY_ANDROID
            StartCoroutine(VersionManager.Instance.DownApk(_model.android_apkUrl));

#elif UNITY_IPHONE
			//IOSCall.Instance.OpenUrl (url);
#endif
        }
    }
}
