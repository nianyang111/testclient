using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class UIUtils
{
    /// <summary> 封装系统Debug.Log </summary>
    public static void Log(object message)
    {
        if (PageManager.Instance.isDebugLog)
            Debug.Log(message);
    }

    /// <summary> 创建指定属性的物体 </summary>
    public static GameObject CreateGameObject(Transform parent, string name = null, Vector3 position = default(Vector3), Vector3 angle = default(Vector3))
    {
        GameObject go = new GameObject();
        if (name == null)
            go.name = typeof(GameObject).ToString();
        else
            go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localEulerAngles = angle;
        return go;
    }

    /// <summary> 创建指定属性和组件的物体 </summary>
    public static T CreateGameObject<T>(Transform parent, string name = null, Vector3 position = default(Vector3), Vector3 angle = default(Vector3)) where T : MonoBehaviour
    {
        T t = new GameObject().AddComponent<T>();
        if (name == null)
            t.name = typeof(T).ToString();
        else
            t.name = name;
        t.transform.SetParent(parent, false);
        t.transform.localPosition = position;
        t.transform.localEulerAngles = angle;
        return t;
    }

    /// <summary> 初始化目标的UI属性 </summary>
    public static void AttachAndReset(GameObject go, Transform parent, GameObject prefab = null)
    {
        RectTransform rectTrans = go.transform as RectTransform;
        if (rectTrans)
        {
            rectTrans.SetParent(parent);
            rectTrans.localPosition = Vector3.zero;
            rectTrans.localScale = Vector3.one;
            if (prefab == null)
            {
                rectTrans.sizeDelta = Vector2.zero;
                rectTrans.localPosition = Vector2.zero;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
            }
            else
            {
                RectTransform prefabRectTrans = prefab.transform as RectTransform;
                if (prefabRectTrans)
                {
                    rectTrans.sizeDelta = prefabRectTrans.sizeDelta;
                    rectTrans.localPosition = prefabRectTrans.localPosition;
                    rectTrans.offsetMax = prefabRectTrans.offsetMax;
                    rectTrans.offsetMin = prefabRectTrans.offsetMin;
                }
            }
        }
    }

    /// <summary> 给指定物体指定路径下的按钮添加点击事件 </summary>
    public static void RegisterButton(string buttonPath, UnityAction action, Transform parent)
    {
        Transform child = parent.Find(buttonPath);
        if (child)
            UGUIEventListener.Get(child.gameObject).onClick = delegate { action(); };
    }

    public static Vector2 GetCenterPosInCanvas(Canvas canvas, RectTransform rectTrans)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                         rectTrans.position, canvas.worldCamera, out pos))
        {
            var delta = new Vector2((.5f - rectTrans.pivot.x) * rectTrans.rect.width, (.5f - rectTrans.pivot.y) * rectTrans.rect.height);
            return pos + delta;
        }

        throw new System.Exception("Error! Get RectTransform center position in canvas fail.");
    }

    public static Vector2 GetPositionForNewCanvas(Canvas srcCanvas, Canvas dstCanvas, Vector2 pos)
    {
        pos.x *= (dstCanvas.transform as RectTransform).sizeDelta.x / (srcCanvas.transform as RectTransform).sizeDelta.x;
        pos.y *= (dstCanvas.transform as RectTransform).sizeDelta.y / (srcCanvas.transform as RectTransform).sizeDelta.y;
        return pos;
    }

    /// <summary> 得到UI相对屏幕左下角的位置(屏幕坐标) </summary>
    public static Vector2 GetUIScreenPos(Canvas canvas, RectTransform rectTrans)
    {
        Vector2 ve1 = GetCenterPosInCanvas(canvas, rectTrans);
        Vector2 ve2 = new Vector2(Screen.width / -2, Screen.height / -2);
        Vector2 screenPos = ve1 - ve2;
        return screenPos;
    }

    /// <summary> 得到鼠标相对Canvas中心的位置 </summary>
    public static Vector2 GetMouseCenterPosInCanvas()
    {
        Vector2 mousePosition = Input.mousePosition;

        Vector2 middlePos = new Vector2(Screen.width / 2, Screen.height / 2);

        Vector2 endPos = middlePos - mousePosition;//最终位置
        return endPos = new Vector2(-1 * endPos.x, -1 * endPos.y);
    }

    public static void SetScreenPosition(ref RectTransform target, Canvas canvas, Vector2 dstCenter)
    {
        Vector2 center = UIUtils.GetCenterPosInCanvas(canvas, target);
        target.anchoredPosition += dstCenter - center;
    }

    /// <summary> 获取当前Canvas的Rect值,通过屏幕坐标转化 </summary>
    public static Rect GetRectInCanvas(Canvas canvas, RectTransform rectTrans)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                         rectTrans.position, canvas.worldCamera, out pos))
        {
            var rect = new Rect(new Vector2(pos.x - rectTrans.pivot.x * rectTrans.rect.width, pos.y - rectTrans.pivot.y * rectTrans.rect.height), rectTrans.rect.size);
            return rect;
        }

        throw new System.Exception("Error! Get RectTransform rect in canvas fail.");
    }

    /// <summary> 销毁目标物体的所有子物体 </summary>
    public static void DestroyChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            GameObject.Destroy(parent.GetChild(i).gameObject);
    }

    /// <summary> 设置目标物体下所有子物体的显隐状态 </summary>
    public static void SetAllChildrenActive(Transform trans, bool active)
    {
        for (int i = 0; i < trans.childCount; ++i)
            trans.GetChild(i).gameObject.SetActive(active);
    }

    /// <summary> 设置目标物体和目标物体下所有子物体的layer </summary>
    public static void SetAllChildrenLayer(Transform tf, int layer)
    {
        tf.gameObject.layer = layer;
        for (int i = 0; i < tf.childCount; i++)
            tf.GetChild(i).gameObject.layer = layer;
    }

    /// <summary> 设置Canvas的GraphicRaycaster的状态 </summary>
    public static void SetCanvasRarcaster(Transform canvasTransform, bool isEnable)
    {
        GraphicRaycaster gr = canvasTransform.GetComponent<GraphicRaycaster>();
        if (gr)
            gr.enabled = isEnable;
    }

    /// <summary> 在指定物体上添加指定图片 </summary>
    public static Image AddImage(GameObject target, Sprite sprite)
    {
        target.SetActive(false);
        Image image = target.GetComponent<Image>();
        if (!image)
            image = target.AddComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
        target.SetActive(true);
        return image;
    }

    /// <summary> 角度转向量 </summary>
    public static Vector2 AngleToVector2D(float angle)
    {
        float radian = Mathf.Deg2Rad * angle;
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
    }

    /// <summary> 延迟销毁或隐藏目标物体 </summary>
    public static void DelayDesOrDisObject(GameObject target, float delayTime, bool isDestroy = true, bool isFadeOut = false, UnityAction ua = null)
    {
        PageManager.Instance.CurrentPage.StartCoroutine(DelayDesOrDisObjectAs(target, delayTime, isDestroy, isFadeOut, ua));
    }

    private static IEnumerator DelayDesOrDisObjectAs(GameObject target, float delayTime, bool isDestroy, bool isFadeOut, UnityAction ua)
    {
        bool isHadCG = true;
        if (isFadeOut)
        {
            CanvasGroup cg = target.GetComponent<CanvasGroup>();
            if (!cg)
            {
                cg = target.AddComponent<CanvasGroup>();
                isHadCG = false;
            }
            cg.blocksRaycasts = false;
            cg.interactable = false;
            float speed = 0.1f;
            while (cg && cg.alpha > 0)
            {
                cg.alpha -= speed;
                yield return new WaitForSecondsRealtime(delayTime * speed);
            }
        }
        else
            yield return new WaitForSecondsRealtime(delayTime);
        if (isDestroy)
        {
            if (target)
                GameObject.Destroy(target);
        }
        else
        {
            if (target)
            {
                target.SetActive(false);
                if (isFadeOut)
                {
                    CanvasGroup cg = target.GetComponent<CanvasGroup>();
                    cg.alpha = 1;
                    if (!isHadCG)
                        GameObject.Destroy(cg);
                }
            }
        }
        if (ua != null) ua();
    }

    /// <summary> 分秒 </summary>
    public static string GetTimeStrWithoutHour(int time)
    {
        int minute1 = time / 600;
        time -= minute1 * 600;

        int minute2 = time / 60;
        time -= minute2 * 60;

        int second1 = time / 10;
        time -= second1 * 10;

        return string.Format("{0}{1}:{2}{3}", minute1, minute2, second1, time);
    }

    /// <summary> 时分秒 </summary>
    public static string GetTimeStr(int time)
    {
        int hour1 = time / 36000;
        time -= hour1 * 36000;

        int hour2 = time / 3600;
        time -= hour2 * 3600;

        int minute1 = time / 600;
        time -= minute1 * 600;

        int minute2 = time / 60;
        time -= minute2 * 60;

        int second1 = time / 10;
        time -= second1 * 10;

        return string.Format("{0}{1}:{2}{3}:{4}{5}", hour1, hour2, minute1, minute2, second1, time);
    }

    /// <summary>
    /// 截屏，指定位置、尺寸、类型
    /// </summary>
    /// <param name="rect">截图的rect信息</param>
    /// <param name="ua">截图完毕后执行的函数</param>
    /// <param name="textureType">截图类型 1-jpg, 2-png, 其他-jpg</param>
    /// <param name="dest">截图的偏移量</param>
    public static void PrintScreenNextFrame(Rect rect, UnityAction<byte[]> ua = null, int textureType = 1, Vector2 dest = default(Vector2))
    {
        PageManager.Instance.CurrentPage.StartCoroutine(PrintScreenAc(rect, ua, textureType, dest));
    }

    private static IEnumerator PrintScreenAc(Rect rect, UnityAction<byte[]> ua, int textureType, Vector2 dest)
    {
        Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        yield return new WaitForEndOfFrame();
        texture.ReadPixels(rect, (int)dest.x, (int)dest.y);
        texture.Apply();

        byte[] bytes;
        if (textureType == 1)
            bytes = texture.EncodeToJPG();
        else if (textureType == 2)
            bytes = texture.EncodeToPNG();
        else
            bytes = texture.EncodeToJPG();
        if (ua != null)
            ua(bytes);
    }

    /// <summary>
    /// 截屏，指定位置、尺寸、类型
    /// </summary>
    /// <param name="rect">截图的rect信息</param>
    /// <param name="dest">截图的偏移量</param>
    public static Texture2D PrintScreen(Rect rect, Vector2 dest = default(Vector2))
    {
        Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        texture.ReadPixels(rect, (int)dest.x, (int)dest.y);
        texture.Apply();
        return texture;
    }

    /// <summary>
    /// 递归找子物体
    /// </summary>
    public static GameObject FindHideChildGameObject(GameObject parent, string childName)
    {
        if (parent.name == childName)
        {
            return parent;
        }
        if (parent.transform.childCount < 1)
        {
            return null;
        }
        GameObject obj = null;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject go = parent.transform.GetChild(i).gameObject;
            obj = FindHideChildGameObject(go, childName);
            if (obj != null)
            {
                break;
            }
        }
        return obj;
    }
    
    /// <summary>
    /// 递归找子物体
    /// </summary>
    public static T FindHideChildGameObject<T>(GameObject parent, string childName) where T:Component
    {
        return FindHideChildGameObject(parent, childName).GetComponent<T>();
    }  
}