using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum LoadingType
{
    Common,
    Progress
}

public class LoadingNode : Node
{
    public static LoadingNode instance;
    public GameObject loadingImage;
    public Text progress, describe;

    /// <summary>
    /// 打开一个LoadingNode
    /// </summary>
    /// <param name="type">Loading类型</param>
    /// <param name="describe">描述</param>
    /// <param name="progress">加载进度</param>
    public static void OpenLoadingNode(LoadingType type, string describe = null, float progress = 0)
    {
        if (instance == null)
            instance = NodeManager.OpenNode<LoadingNode>(null, null, false, false);
        switch (type)
        {
            case LoadingType.Common:
                instance.progress.gameObject.SetActive(false);
                instance.StartCoroutine(instance.SetTimer());
                break;
            case LoadingType.Progress:
                instance.progress.gameObject.SetActive(true);
                instance.progress.text = (progress * 100).ToString("F1");
                break;
        }
        if (!string.IsNullOrEmpty(describe))
            instance.describe.text = describe;
    }
    public int mTimer = 3;
    IEnumerator SetTimer()
    {
        while (mTimer>0)
        {
            yield return new WaitForSeconds(1f);
            mTimer--;
        }
        TipManager.Instance.OpenTip(TipType.SimpleTip, "请求超时...");
        CloseLoadingNode();
    }
    public static void CloseLoadingNode()
    {
        if (instance != null)
            instance.Close();
    }

    void Update()
    {
        loadingImage.transform.Rotate(0, 0, 300 * Time.deltaTime);
    }

    void OnDestroy()
    {
        instance = null;
    }
}

