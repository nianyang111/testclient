using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatBallNode : Node {

    public UnityEngine.UI.Button dradPanel;
    public FloatBallWheel wheel;
    public FloatBall ball;

    void Start()
    {
        ball.Init(this);
        wheel.Init(this);
        SelectShow();
        dradPanel.onClick.AddListener(delegate {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            SelectShow(true); 
        });
    }

    public void SelectShow(bool isShow=true)
    {
        dradPanel.gameObject.SetActive(!isShow);
        ball.gameObject.SetActive(isShow);
    }
}
