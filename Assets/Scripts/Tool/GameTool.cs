using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// 开发辅助工具类
/// </summary>
public class GameTool
{
    public static T Find<T>(Transform parent, string path) where T : Component
    {
        if (parent != null)
            return parent.Find(path).GetComponent<T>();
        return null;
    }

    public static T AddScript<T>(GameObject go) where T : Component
    {
        return AddScript(go, typeof(T).ToString()) as T;
    }

    public static Component AddScript(GameObject go, string scriptName)
    {
        Type type = Type.GetType(scriptName);
        if (type != null)
            return go.AddComponent(type);
        return null;
    }

 
    public static GameObject CreateGameObject(string name, Transform parent = null)
    {
        string _name = name;
        if (string.IsNullOrEmpty(_name))
        {
            _name = "new GameObjct";
        }
        GameObject go = new GameObject(_name);
        if (parent != null)
            SetParent(go, parent);
        return go;
    }
    public static void SetParent(GameObject obj, Transform parent)
    {
        SetParent(obj.transform, parent);
    }
    public static void SetParent(Transform obj, Transform parent)
    {
        obj.SetParent(parent);
    }
    public static void clearChild(Transform container)
    {
        foreach (Transform item in container)
        {
            if (container != item)
                MonoBehaviour.Destroy(item.gameObject);
        }
    }

    public static bool GetActive(Behaviour mb)
    {
        return mb && mb.enabled;
    }

    //public static string GetCoinIcon(CoinType _type)
    //{
    //    return _type.ToString();
    //}


    public static void SetLayer(GameObject g, string layerName)
    {
        g.layer = LayerMask.NameToLayer(layerName);
    }

    public static T StrToEnum<T>(string str)
    {
        return (T)Enum.Parse(typeof(T), str);
    }

    public static Vector3 StrToVector3(string str)
    {
        if (string.IsNullOrEmpty(str) || str == "0")
        {
            return Vector3.zero;
        }
        if(str.StartsWith("\""))
        {
            str = str.Replace("\"", "").Trim();
        }
        string[] arr = str.Split(',');
        return new Vector3(StrToFloat(arr[0]), arr.Length > 1 ? StrToFloat(arr[1]) : 0, arr.Length > 2 ? StrToFloat(arr[2]) : 0);
    }


    public static int StrToInt(string str)
    {
        return str == null || str.Trim() == "" ? 0 : int.Parse(str.Replace("_", "-"));
    }


    public static float StrToFloat(string str)
    {
        return str == null || str.Trim() == "" ? 0f : float.Parse(str.Replace("_", "-"));
    }

    /// <summary>
    /// 是否是数字
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool iNum(string value)
    {
        try
        {
            float.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// 从数组中删除指定位置的元素
    /// </summary>
    public static void arrayDeleteAt<T>(ref T[] source, int index, int count)
    {
        int offset = 0;
        for (int i = 0; i < source.Length - count; i++)
        {
            if (i >= index && i < count + index)
            {
                source[i] = source[source.Length - offset - 1];
                offset++;
            }
        }
        Array.Resize<T>(ref source, source.Length - count);
    }


    public static string NumToStr(long num,bool isQian=true)
    {
        float number=0;
        //亿
        number = num / 100000000f;
        if (number >= 1)
        {
            return number + "亿";
        }
        
        //万
        number = num / 10000f;
        if (number >= 1)
        {
            return number + "万";
        }

        //千
        number = num / 1000f;
        if (number >= 1)
        {
            if (isQian)
                return number + "千";
            else

                return num.ToString();
        }
        return num.ToString();
    }

    public static string KeepGoldNum(long num)
    {
        string str = num.ToString();
        string valus;
        if (str.Length >= 9)
        {
            str = str.Substring(0, str.Length - 7);
            valus = str;
            valus = valus.Substring(valus.Length - 1, 1);
            if (valus != "0")
                str = str.Insert(str.Length - 1, ".");
            else
                str = str.Substring(0, str.Length - 1);
            return str + "亿";
        }
        if (str.Length >= 5)
        {
            str = str.Substring(0, str.Length - 3);
            valus = str;
            valus = valus.Substring(valus.Length - 1, 1);
            if (valus != "0")
                str = str.Insert(str.Length - 1, ".");
            else
                str = str.Substring(0, str.Length - 1);
            return str + "万";
        }
        return str;
    }
}
