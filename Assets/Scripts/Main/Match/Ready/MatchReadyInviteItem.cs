using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MatchReadyInviteItem : MonoBehaviour,IPointerClickHandler {
    public Text userName;
    public GameObject clickBg;
    public GameObject selectIcon;
    public MatchInvitePanel panel;
    public int userId;
    public void Init(net_protocol.InviteUser data)
    {
        userId = data.userId;
        userName.text = data.userName;
        OnSelect();
    }
    public void Select()
    {
        selectIcon.SetActive(true);
    }
    public void OnSelect()
    {
        selectIcon.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!AudioManager.Instance.IsSoundPlaying)
            AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
        panel.CurItem = this;
    }

}