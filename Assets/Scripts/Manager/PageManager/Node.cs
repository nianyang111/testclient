using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 窗口关闭方式
/// </summary>
public enum NodeCloseMode
{
    Hide,
    Destroy
}

public class Node : MonoBehaviour
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public GameObject btnClose;
    /// <summary>
    /// 蒙层
    /// </summary>
    public GameObject maskLayer;
    public NodeCloseMode windowCloseMode;
    /// <summary>
    /// 点击蒙层是否关闭窗口
    /// </summary>
    public bool isClickMaskClose = false;
    [HideInInspector]
    public List<string> lastNodeList = new List<string>();
    [HideInInspector]
    public string nodeName;

    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Init()
    {
        if (isClickMaskClose)
            UGUIEventListener.Get(maskLayer).onClick += delegate { Close(); };
        if (btnClose)
            UGUIEventListener.Get(btnClose).onClick += delegate { Close(); };
        gameObject.SetActive(false);
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close(bool isOpenLast = true)
    {
        if (gameObject != null)
        {
            if (isOpenLast)
            {
                if (lastNodeList.Count != 0)
                {
                    Node node = NodeManager.OpenNode(lastNodeList[lastNodeList.Count - 1], null, null, false, false);
                    lastNodeList.RemoveAt(lastNodeList.Count - 1);
                    NodeManager.currentNodeName = node.name;
                }
                else
                    NodeManager.currentNodeName = null;
            }
            if (windowCloseMode == NodeCloseMode.Hide)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
    }
}
