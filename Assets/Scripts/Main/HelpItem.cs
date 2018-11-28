using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpItem : MonoBehaviour
{
    public CommonAnimation anim;
    public RectTransform contentRect;
    public Transform arrow;
    public Text title, content;
    bool isPlay = false;

    public void Init(HelpData data)
    {
        title.text = data.questionType;
        content.text = data.answerType;
        UGUIEventListener.Get(arrow.gameObject).onClick = (g) => { PlayAnim(); };
    }

    private void PlayAnim()
    {
        arrow.rotation *= Quaternion.Euler(new Vector3(0,0,180));
        if (!isPlay)
        {
            isPlay = true;
            
            anim.sizeList.Clear();
            anim.sizeList.Add(new Vector2(1240, 100));
            anim.sizeList.Add(new Vector2(1240, contentRect.rect.height+200));
            anim.Play();
        }
        else
        {
            isPlay = false;
            anim.sizeList.Clear();
            anim.sizeList.Add(new Vector2(1240, contentRect.rect.height+200));
            anim.sizeList.Add(new Vector2(1240, 100));
            anim.Play();
        }
    }
}