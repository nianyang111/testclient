﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UGUIEventListener : MonoBehaviour,
                                IPointerClickHandler,
                                IPointerDownHandler,
                                IPointerEnterHandler,
                                IPointerExitHandler,
                                IPointerUpHandler,
                                IBeginDragHandler,
                                IDragHandler,
                                IEndDragHandler
{
    public delegate void VoidDelegate(GameObject go);
    public delegate void VoidDelegateDrag(GameObject go, PointerEventData eventData);
    public AudioManager.AudioSoundType m_AudioType;

    public VoidDelegate onClick;
    public VoidDelegateDrag onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onDragStart;
    public VoidDelegateDrag onDrag;
    public VoidDelegate onDragEnd;
    public VoidDelegate onLongPress;
    //与长按配合使用
    bool isUp = true;
    float time = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            if (m_AudioType != AudioManager.AudioSoundType.None && GetComponent<Button>() != null && GetComponent<Button>().interactable)
                if (!AudioManager.Instance.IsSoundPlaying)
                    AudioManager.Instance.PlaySound(m_AudioType);
            onClick(gameObject);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject, eventData);
        if (isUp) time = Time.realtimeSinceStartup;
        isUp = false;
    }
    public void OnPointerEnter(PointerEventData eventData) { if (onEnter != null)  onEnter(gameObject); }
    public void OnPointerExit(PointerEventData eventData) { if (onExit != null) onExit(gameObject); }
    public void OnPointerUp(PointerEventData eventData) { if (onUp != null) onUp(gameObject); isUp = true; time = 0; }
    public void OnBeginDrag(PointerEventData eventData) { if (onDragStart != null) onDragStart(gameObject); }
    public void OnDrag(PointerEventData eventData) { if (onDrag != null) onDrag(gameObject, eventData); }
    public void OnEndDrag(PointerEventData eventData) { if (onDragEnd != null) onDragEnd(gameObject); }

    public static UGUIEventListener Get(GameObject go, AudioManager.AudioSoundType clickAudio = AudioManager.AudioSoundType.BtnClick)
    {
        UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
        if (listener == null) listener = go.AddComponent<UGUIEventListener>();
        listener.m_AudioType = clickAudio;
        return listener;
    }

    void Update()
    {
        //长按
        if (!isUp && onLongPress != null && Time.realtimeSinceStartup - time > 1)
        {
            onLongPress(gameObject);
            isUp = true;
        }
    }
}