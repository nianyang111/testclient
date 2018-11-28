using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
public class FindFontError : Editor
{
#if UNITY_EDITOR
    static string fontName = "Arial";
    [MenuItem("自定义工具/字体/字体检查 %Q")]
    static void CheckFont()
    {
        Transform[] transs = Selection.transforms;
        for (int i = 0; i < transs.Length; i++)
        {
            Text[] texts = transs[i].GetComponentsInChildren<Text>(true);
            for (int j = 0; j < texts.Length; j++)
            {
                if (texts[j].font.name == fontName)
                {
                    Debug.LogWarning(texts[j].name);
                }
            }
        }
    }
#endif
}
