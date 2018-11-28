using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGameNode : Node
{
    public Slider soundSlider, muiceSlider;
    public RadioButton offAudioBtn, readBtn, chatBtn, shockBtn;
    public Button soundMiliBtn, soundMaxBtn, muiceMiliBtn, muiceMaxBtn;
    public Image readAoudioImage;
    public List<Image> readImageList;
    public override void Init()
    {
        base.Init();
        UGUIEventListener.Get(soundMiliBtn.gameObject).onClick = delegate { SetSoundSlider(soundSlider.minValue); };
        UGUIEventListener.Get(soundMaxBtn.gameObject).onClick = delegate { SetSoundSlider(soundSlider.maxValue); };
        UGUIEventListener.Get(muiceMiliBtn.gameObject).onClick = delegate { SetMuiceSlider(muiceSlider.minValue); };
        UGUIEventListener.Get(muiceMaxBtn.gameObject).onClick = delegate { SetMuiceSlider(muiceSlider.maxValue); };
        offAudioBtn.onValueChange = delegate { SetNode.off = offAudioBtn.isTrue ? 1 : 0; OffAudio(); };
        readBtn.onValueChange = delegate { SetNode.read = readBtn.isTrue ? 1 : 0; };//读牌
        chatBtn.onValueChange = delegate { SetNode.chat = chatBtn.isTrue ? 1 : 0; };//聊天
        shockBtn.onValueChange = delegate { SetNode.shock = shockBtn.isTrue ? 1 : 0; };//震动
        soundSlider.onValueChanged.AddListener(num => SetSoundSlider(num));
        muiceSlider.onValueChanged.AddListener(num => SetMuiceSlider(num));
    }

    public override void Open()
    {
        base.Open();
        SetNode.ExpireSet();
        AudioManager.Instance.MusicValue = muiceSlider.value = SetNode.musicSource;
        AudioManager.Instance.SoundValue = soundSlider.value = SetNode.soundSource;
        offAudioBtn.SetValue(SetNode.off == 1);
        readBtn.SetValue(SetNode.read == 1);
        chatBtn.SetValue(SetNode.chat == 1);
        shockBtn.SetValue(SetNode.shock == 1);
        OffAudio();
    }

    private void OffAudio()
    {
        soundMiliBtn.interactable = soundMaxBtn.interactable = !offAudioBtn.isTrue;
        muiceMiliBtn.interactable = muiceMaxBtn.interactable = !offAudioBtn.isTrue;
        readBtn.interactable = soundSlider.interactable = muiceSlider.interactable = readAoudioImage.raycastTarget = !offAudioBtn.isTrue;//

        for (int i = 0; i < readImageList.Count; i++)
        {
            readImageList[i].color = !offAudioBtn.isTrue ? Color.white : new Color(148 / 255.0f, 148 / 255.0f, 148 / 255.0f);
        }
        SetNode.SetAudio();
    }
    /// <summary>设置音效最大最小 </summary>
    private void SetSoundSlider(float num)
    {
        if (!offAudioBtn.isTrue)
        {
            soundSlider.onValueChanged.RemoveAllListeners();
            soundSlider.value = num;
            soundSlider.onValueChanged.AddListener(value => SetSoundSlider(value));
            AudioManager.Instance.SoundValue = SetNode.soundSource = num;
        }
    }
    /// <summary>设置音乐最大最小 </summary>
    private void SetMuiceSlider(float num)
    {
        if (!offAudioBtn.isTrue)
        {
            muiceSlider.onValueChanged.RemoveAllListeners();
            muiceSlider.value = num;
            muiceSlider.onValueChanged.AddListener(value => SetMuiceSlider(value));
            AudioManager.Instance.MusicValue = SetNode.musicSource = num;
        }
    }

    public override void Close(bool isOpenLast = true)
    {
        base.Close(isOpenLast);

        SetNode.SaveSet();
    }
}
