using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatBallManager : MonoBehaviour {

    private static FloatBallManager instance;
    public static FloatBallManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<FloatBallManager>();
                go.name = instance.GetType().ToString();
                instance.Init();
            }
            return instance;
        }
        set { instance = value; }
    }
    private FloatBallNode _node;
    public void Init()
    {
        if(_node==null)
        StartCoroutine(_Init());
    }

    IEnumerator _Init()
    {
        Instance.gameObject.transform.SetAsLastSibling();
        var trans= BundleManager.Instance.GetGameObject("nodes/floatballnode").transform as RectTransform;
        trans.SetParent(PageManager.Instance.transform);
        trans.anchoredPosition = Vector3.zero;
        trans.sizeDelta = Vector3.zero;
        trans.localScale = Vector3.one;
        _node = trans.GetComponent<FloatBallNode>();
        Hide();
        yield return null;
    }

    public void Show()
    {
        if (_node)
            _node.gameObject.SetActive(true);
    }
    public void Hide()
    {
        if (_node)
            _node.gameObject.SetActive(false);
    }
    public void Clear()
    {
        instance = null;
        Destroy(_node.gameObject);
        Destroy(gameObject); 
    }
}
