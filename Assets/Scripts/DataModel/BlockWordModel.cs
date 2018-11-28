using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class BlockWordModel
{

    static string[] blockWordArr;

    static string blockConfigPath = "BlockConfig/BlockConfig";

    static BlockWordModel()
    {
        Load();
    }

    static void Load()
    {
        TextAsset text = Resources.Load<TextAsset>(blockConfigPath);
        blockWordArr = text.text.Split('|');
    }

    /// <summary>
    /// 判断文字是否违规
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool CheckIsBlock(string value)
    {
        value = value.Trim();
        if (!string.IsNullOrEmpty(value))
        {
            for (int i = 0; i < blockWordArr.Length; i++)
            {
                if (value.Contains(blockWordArr[i]) || blockWordArr[i].Contains(value))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
