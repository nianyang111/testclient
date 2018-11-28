using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardAnimationTest : MonoBehaviour
{

    void OnGUI()
    {
        if(GUILayout.Button("转圈"))
        {
            Play();
        }
    }

    public int cardType = 3;

    void Play()
    {
        Rest();
        for (int i = 0; i < cardType; i++)
        {
            Transform chiledTrans = transform.GetChild(i);
            int index = Index(i + 1, cardType);
            chiledTrans.localPosition = new Vector3(0, chiledTrans.localPosition.y, 0);
            chiledTrans.localPosition += Vector3.up * i * 7;
            CommonAnimation chiledAni = chiledTrans.gameObject.AddComponent<CommonAnimation>();
            chiledAni.angleList.Add(chiledTrans.localEulerAngles);
            chiledAni.angleList.Add(Vector3.forward * 20 * index);
            chiledAni.angleDelayTime = 0.5f;
            chiledAni.time = 0.2f;
            chiledAni.Play();            
        }

        int posIndex = 1;
        for (int i = cardType; i < transform.childCount; i++)
        {            
            Transform chiledTrans = transform.GetChild(i);
            chiledTrans.localPosition += Vector3.right * (200 + posIndex++ * (chiledTrans as RectTransform).sizeDelta.x / 2);
        }
    }

    void Rest()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localEulerAngles = Vector3.zero;
        }
    }

    /// <summary>
    /// 得到相对于中间位置的索引
    /// </summary>
    int Index(int indexInParent, int allCount)
    {
        float midelle = allCount / 2f;
        if (midelle % 2 == 0)
        {//偶数
            return Index(indexInParent, allCount + 1);
        }
        else
        {//基数
            midelle = Mathf.Round(midelle);
            return (int)midelle - indexInParent;
        }
    }
}
