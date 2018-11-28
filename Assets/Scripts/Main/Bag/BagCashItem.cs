using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagCashItem : MonoBehaviour {
    public Image cashIcon;
    public int cashNum;
    public string cashName;
    public BagCashPanel panel;
    void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            panel.CurItem = this;
            panel.selectPanel.SetActive(false);
        });
    }
}
