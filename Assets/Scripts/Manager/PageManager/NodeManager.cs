using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NodeManager
{
    public static string currentNodeName;

    public static Node OpenNode(string nodeName = null, string folderName = null, Action finishCallback = null, bool isCloseLastNode = true, bool isAddToRecord = true)
    {
        nodeName = (string.IsNullOrEmpty(folderName) ? "" : (folderName + "/")) + nodeName;
        return OpenNodeAc(nodeName, finishCallback, isCloseLastNode, isAddToRecord);
    }

    public static T OpenNode<T>(string folderName = null, Action finishCallback = null, bool isCloseLastNode = true, bool isAddToRecord = true) where T : Node
    {
        string fileName = (string.IsNullOrEmpty(folderName) ? "" : (folderName + "/")) + GetNodeType<T>();
        return (T)OpenNodeAc(fileName, finishCallback, isCloseLastNode, isAddToRecord);
    }

    /// <summary>
    /// 打开指定类型Node
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="folderName"></param>
    /// <param name="finishCallback"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static Node OpenNodeAc(string fileName = null, Action finishCallback = null, bool isCloseLastNode = true, bool isAddToRecord = true)
    {
        Node node = CreateNode(fileName, finishCallback);
        if (node != null)
        {
            Node lastNode = GetNode(currentNodeName);
            if (isCloseLastNode && lastNode && lastNode != node)
            {
                node.lastNodeList.Add(lastNode.nodeName);
                lastNode.Close(false);
            }
            if (isAddToRecord)
                currentNodeName = node.name;
        }
        else
            UIUtils.Log("打开失败!");
        return node;
    }

    static Node CreateNode(string nodeName, Action finishCallback = null)
    {
        if (string.IsNullOrEmpty(nodeName))
            return null;

        Node node = GetNode(nodeName);
        if (node)
        {
            node.transform.SetAsLastSibling();
            node.Open();
        }
        else
        {
            string path = "nodes/" + nodeName;
            GameObject go = BundleManager.Instance.GetGameObject(path, PageManager.Instance.CurrentPage.name);
            if (go)
            {
                node = go.GetComponent<Node>();
                node.nodeName = nodeName;
                node.Init();
                UIUtils.AttachAndReset(go, PageManager.Instance.CurrentPage.transform);
                go.transform.SetAsLastSibling();
                node.Open();
            }
            else
                Debug.LogError("错误! 找不到路径: " + path);
        }
        if (finishCallback != null)
            finishCallback();
        return node;
    }

    /// <summary>
    /// 获取指定Node(已打开),不存在则返回null
    /// </summary>
    /// <param name="nodeName"></param>
    /// <returns></returns>
    public static Node GetNode(string nodeName)
    {
        if (!string.IsNullOrEmpty(nodeName))
        {
            Node[] nodes = PageManager.Instance.CurrentPage.GetComponentsInChildren<Node>(true);
            foreach (Node n in nodes)
                if (nodeName.IndexOf(n.name) >= 0)
                    return n;
        }
        return null;
    }

    /// <summary>
    /// 获取指定Node(已打开),不存在则返回null
    /// </summary>
    /// <typeparam name="T">Node</typeparam>
    /// <returns></returns>
    public static T GetNode<T>() where T : Node
    {
        return (T)GetNode(typeof(T).ToString());
    }

    static string GetNodeType<T>() where T : Node
    {
        return typeof(T).ToString();
    }

    public static void CloseTargetNode<T>() where T : Node
    {
        T t = GetNode<T>();
        if (t)
            t.Close();
        else
            UIUtils.Log("指定Node:" + t + "不存在");
    }

    public static void CloseTargetNode(string nodeName)
    {
        Node node = GetNode(nodeName);
        if (node)
            node.Close();
        else
            UIUtils.Log("指定Node:" + nodeName + "不存在");
    }
}