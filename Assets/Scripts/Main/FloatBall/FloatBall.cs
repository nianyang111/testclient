using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FloatBall : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Button a;

    public void Init(FloatBallNode node)
    {
        a.onClick.AddListener(() => {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);    
            node.SelectShow(false); 
        });
        var mWidth = Screen.width / 2;
        RectTransform rect = transform.GetComponent<RectTransform>();
        Vector3 mPosition = Vector3.zero;
        mPosition = new Vector3(Random.Range(mWidth - 10, mWidth + 10), Random.Range(rect.rect.width / 4, Screen.height - (rect.rect.width / 4)));
        if (mPosition.x >= mWidth)
            mPosition.x = Screen.width - (rect.rect.height / 4);
        if (mPosition.x < mWidth)
            mPosition.x = (rect.rect.height / 4);
        if (mPosition.y > Screen.height - (rect.rect.width / 4))
            mPosition.y = Screen.height - (rect.rect.width / 4);
        if (mPosition.y < (rect.rect.width / 4))
            mPosition.y = (rect.rect.width / 4);
        transform.position = mPosition;

    }
    
    public void OnDrag(PointerEventData eventData)
    {
        GameObject pointerDrag = eventData.pointerDrag;
        RectTransform rect = pointerDrag.GetComponent<RectTransform>();
        Vector3 globalMousePosition;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out globalMousePosition))
        {
            if (globalMousePosition.y > Screen.height - (rect.rect.width / 4))
                globalMousePosition.y = Screen.height - (rect.rect.width / 4);
            if (globalMousePosition.y < (rect.rect.width / 4))
                globalMousePosition.y = (rect.rect.width / 4);
            if (globalMousePosition.x > Screen.width - (rect.rect.height / 4))
                globalMousePosition.x = Screen.width - (rect.rect.height / 4);
            if (globalMousePosition.x < (rect.rect.height / 4))
                globalMousePosition.x = (rect.rect.height / 4);
            pointerDrag.transform.position = globalMousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject pointerDrag = eventData.pointerDrag;
        RectTransform rect = pointerDrag.GetComponent<RectTransform>();
        Vector3 globalMousePosition;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out globalMousePosition))
        {
            if(globalMousePosition.x > Screen.width / 2)
                globalMousePosition.x = Screen.width - (rect.rect.height / 4);
            if(globalMousePosition.x < Screen.width / 2)
                globalMousePosition.x = (rect.rect.height / 4);
            if (globalMousePosition.y > Screen.height - (rect.rect.width / 4))
                globalMousePosition.y = Screen.height - (rect.rect.width / 4);
            if (globalMousePosition.y < (rect.rect.width / 4))
                globalMousePosition.y = (rect.rect.width / 4);
            pointerDrag.transform.position = globalMousePosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }
}