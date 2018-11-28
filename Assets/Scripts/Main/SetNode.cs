using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 设置
/// </summary>
public class SetNode : Node
{
    [HideInInspector]
    public Image soundMiliImage, soundMaxImage, muiceMiliImage, muiceMaxImage, readAoudioImage;
    [HideInInspector]
    public Button aboutBtn, soundMiliBtn, soundMaxBtn, muiceMiliBtn, muiceMaxBtn, helpBtn, serviceBtn, refreshGameBtn;
    public RadioButton offAudioBtn, readBtn, chatBtn, shockBtn, fuDongBtn;
    public Slider soundSlider, muiceSlider;
    public List<Image> readImageList;
    public static int off=0, read=1, chat=1, shock=1, fudong=1;
    public static float musicSource=0.5f,soundSource=0.5f;
    public Text release;

    VersionModel model;
    public override void Init()
    {
        base.Init();
        UGUIEventListener.Get(aboutBtn.gameObject).onClick = (g) => { NodeManager.OpenNode<AboutNode>(); };
        UGUIEventListener.Get(helpBtn.gameObject).onClick = (g) => { NodeManager.OpenNode<HelpNode>(); };
        UGUIEventListener.Get(serviceBtn.gameObject).onClick = (g) => { NodeManager.OpenNode<FeedbackNode>(); };
        UGUIEventListener.Get(refreshGameBtn.gameObject).onClick = (g) => { if (model != null) NodeManager.OpenNode<VersionNode>(null, null, false).Inits(model); };
        UGUIEventListener.Get(soundMiliBtn.gameObject).onClick = (g) => { SetSoundSlider(soundSlider.minValue); };
        UGUIEventListener.Get(soundMaxBtn.gameObject).onClick = (g) => { SetSoundSlider(soundSlider.maxValue); };
        UGUIEventListener.Get(muiceMiliBtn.gameObject).onClick = (g) => { SetMuiceSlider(muiceSlider.minValue); };
        UGUIEventListener.Get(muiceMaxBtn.gameObject).onClick = (g) => { SetMuiceSlider(muiceSlider.maxValue); };

        offAudioBtn.onValueChange = delegate { off = offAudioBtn.isTrue ? 1 : 0; OffAudio(); };
        readBtn.onValueChange = delegate { read = readBtn.isTrue ? 1 : 0; };//读牌
        chatBtn.onValueChange = delegate { chat = chatBtn.isTrue ? 1 : 0; };//聊天
        shockBtn.onValueChange = delegate { shock = shockBtn.isTrue ? 1 : 0; };//震动
        fuDongBtn.onValueChange = delegate { fudong = fuDongBtn.isTrue ? 1 : 0; FloatBall(); };//浮动球
        soundSlider.onValueChanged.AddListener(num => SetSoundSlider(num));
        muiceSlider.onValueChanged.AddListener(num => SetMuiceSlider(num));        

    }

    public override void Open()
    {
        base.Open();
        ExpireSet();
        ForAudio();
        offAudioBtn.SetValue(off == 1);
        readBtn.SetValue(read == 1);
        chatBtn.SetValue(chat==1);
        shockBtn.SetValue(shock == 1);
        fuDongBtn.SetValue(fudong == 1);
        soundSlider.value = AudioManager.Instance.SoundValue;
        muiceSlider.value = AudioManager.Instance.MusicValue;
        OffAudio();
        release.text = "当前版本号：" + Application.version;
        StartCoroutine(VersionManager.Instance.BigVersion(model =>
        {
            this.model = model;
#if UNITY_ANDROID
            refreshGameBtn.gameObject.SetActive(VersionManager.Instance.CompareVersion(model.android_severVersion));

#elif UNITY_IPHONE
   		        refreshGameBtn.gameObject.SetActive(VersionManager.Instance.CompareVersion(model.ios_severVersion));
#elif UNITY_EDITOR
                refreshGameBtn.gameObject.SetActive(false);
#endif
        }));
    }
    
    /// <summary>
    /// 音效管理
    /// </summary>
    public static void ForAudio()
    {
        AudioManager.Instance.MusicValue = musicSource;
        AudioManager.Instance.SoundValue = soundSource;
    }
    /// <summary>
    /// 开关音效
    /// </summary>
    public static void SetAudio()
    {
        AudioManager.Instance.MuteAllSound(off == 1);
    }
    /// <summary>
    /// 浮动球
    /// </summary>
    public static void FloatBall()
    {
        if (fudong==1)
            FloatBallManager.Instance.Show();
        else
            FloatBallManager.Instance.Hide();
    }
    public static void SaveSet()
    {
        StringBuilder sb = new StringBuilder("{");
        sb.Append("\"off\":\"" + off);
        sb.Append("\",\"read\":\"" + read);
        sb.Append("\",\"chat\":\"" + chat);
        sb.Append("\",\"shock\":\"" + shock);
        sb.Append("\",\"fudong\":\"" + fudong);
        sb.Append("\",\"muisc\":\"" + musicSource);
        sb.Append("\",\"effect\":\"" + soundSource + "\"}");
        MiscUtils.CreateTextFile(ConstantUtils.setConfigPath, sb.ToString());
    }
    public static void ExpireSet()
    {
        if (File.Exists(ConstantUtils.setConfigPath))
        {
            JsonData jd = JsonMapper.ToObject(File.ReadAllText(ConstantUtils.setConfigPath));
            if (jd != null)
            {
                musicSource = float.Parse(jd.TryGetString("muisc"));
                soundSource = float.Parse(jd.TryGetString("effect"));
                off = int.Parse(jd.TryGetString("off"));
                read = int.Parse(jd.TryGetString("read"));
                chat = int.Parse(jd.TryGetString("chat"));
                shock = int.Parse(jd.TryGetString("shock"));
                fudong = int.Parse(jd.TryGetString("fudong"));
            }
        }
    }
    /// <summary>关闭音效 </summary>
    private void OffAudio()
    {
        soundMiliBtn.interactable = soundMiliImage.raycastTarget = soundMaxBtn.interactable = soundMaxImage.raycastTarget = !offAudioBtn.isTrue;
        muiceMiliBtn.interactable = muiceMiliImage.raycastTarget = muiceMaxBtn.interactable = muiceMaxImage.raycastTarget = !offAudioBtn.isTrue;
        readBtn.interactable = soundSlider.interactable = muiceSlider.interactable = readAoudioImage.raycastTarget = !offAudioBtn.isTrue;

        for (int i = 0; i < readImageList.Count; i++)
        {
            readImageList[i].color = !offAudioBtn.isTrue ? Color.white : new Color(148 / 255.0f, 148 / 255.0f, 148 / 255.0f);
        }
        SetAudio();
    }
    /// <summary>设置音效最大最小 </summary>
    private void SetSoundSlider(float num)
    {
        if (!offAudioBtn.isTrue)
        {
            soundSlider.onValueChanged.RemoveAllListeners();
            soundSlider.value = num;
            soundSlider.onValueChanged.AddListener(value => SetSoundSlider(value));
            AudioManager.Instance.SoundValue = soundSource = num;
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
            AudioManager.Instance.MusicValue = musicSource = num;
        }
    }

    public override void Close(bool isOpenLast = true)
    {
        base.Close(false);
        SaveSet();
    }
}
