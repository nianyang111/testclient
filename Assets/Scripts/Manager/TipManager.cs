using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public enum TipType
{
    ChooseTip,
    AlertTip,
    SimpleTip
}

public class TipManager : MonoBehaviour
{
    private static TipManager instance;
    public static TipManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                RectTransform rt = go.AddComponent<RectTransform>();
                RectTransform rtParent = (RectTransform)PageManager.Instance.transform;
                go.transform.SetParent(PageManager.Instance.transform);
                go.transform.localPosition = Vector3.zero;
                rt.sizeDelta = new Vector2(rtParent.sizeDelta.x, rtParent.sizeDelta.y);
                rt.localScale = Vector3.one;
                instance = go.AddComponent<TipManager>();
                go.name = instance.GetType().ToString();
            }
            return instance;
        }
        set { instance = value; }
    }

    /// <summary>
    /// 根据tip类型打开指定tip窗口
    /// </summary>
    /// <param name="tipType">tip类型（枚举）</param>
    /// <param name="describe">文本描述</param>
    /// <param name="waitTime">等待时间，结束后关闭tip窗口,SimpleTip默认该值为3</param>
    /// <param name="sureAction">点击确定执行的函数</param>
    /// <param name="cancelAction">点击取消执行的函数（可以为null）</param>
    public void OpenTip(TipType tipType, string describe, float waitTime = 0, UnityAction sureAction = null, UnityAction cancelAction = null)
    {
        StartCoroutine(OpenTipAc(tipType, describe, waitTime, sureAction, cancelAction));
    }

    IEnumerator OpenTipAc(TipType tipType, string describe, float waitTime, UnityAction sureAction, UnityAction cancelAction)
    {
        Instance.gameObject.transform.SetAsLastSibling();
        Transform tf = GetTip(tipType);
        tf.Find("Describe").GetComponent<Text>().text = describe;
        if (tipType != TipType.SimpleTip)
        {
            UIUtils.RegisterButton("BtnSure", () => { ButtonCallBack(tf.gameObject, sureAction); }, tf);
            UIUtils.RegisterButton("BtnClose", () => { tf.gameObject.SetActive(false); }, tf);
            if (tipType == TipType.ChooseTip)
                UIUtils.RegisterButton("BtnCancel", () => { ButtonCallBack(tf.gameObject, cancelAction); }, tf);
        }
        else
        {
            if (waitTime == 0) waitTime = 2;
            UIUtils.RegisterButton("Background", () => { Destroy(tf.gameObject); }, tf);
        }
        tf.gameObject.SetActive(true);
        SetCanvasRarcaster();
        if (LoadingNode.instance != null)
            LoadingNode.instance.Close();
        Transform timerTf = tf.Find("Timer");
        if (waitTime != 0)
        {
            if (timerTf)
            {
                Timer timer = timerTf.GetComponent<Timer>();
                timer.allLength = waitTime;
                timer.ResetTimer(true);
            }
            yield return new WaitForSecondsRealtime(waitTime);
            if (tf && tf.gameObject.activeSelf)
            {
                if (cancelAction != null)
                    cancelAction();
                if (tf)
                    Destroy(tf.gameObject);
            }
        }
        else
            timerTf.gameObject.SetActive(false);
    }

    Transform GetTip(TipType tipType)
    {
        RectTransform tf = transform.Find(tipType.ToString()) as RectTransform;
        if (!tf || tipType == TipType.SimpleTip)
        {
            tf = BundleManager.Instance.GetGameObject(GetTipPath(tipType)).transform as RectTransform;
            tf.SetParent(transform);
            tf.anchoredPosition = Vector3.zero;
            tf.sizeDelta = Vector3.zero;
            tf.localScale = Vector3.one;
        }
        tf.gameObject.SetActive(false);
        return tf;
    }

    void ButtonCallBack(GameObject go, UnityAction ua)
    {
        go.SetActive(false);
        if (ua != null)
            ua();
        SetCanvasRarcaster();
    }

    string GetTipPath(TipType tipType)
    {
        return "tips/" + tipType.ToString();
    }

    void SetCanvasRarcaster()
    {
        bool isEnable = false;
        foreach (Transform t in transform)
            if (t.gameObject.activeSelf)
            {
                isEnable = true;
                break;
            }
        UIUtils.SetCanvasRarcaster(transform, isEnable);
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
