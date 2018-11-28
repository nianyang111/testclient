using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ImageToGrey : Editor
{
    [MenuItem("自定义工具/置灰/添加置灰材质球")]
    static void AddImageToGrey()
    {
        GameObject[] gos = Selection.gameObjects;
        for (int i = 0; i < gos.Length; i++)
            gos[i].GetComponent<Image>().material = Resources.Load<Material>("Shader/ImageToGrey");
    }
}
