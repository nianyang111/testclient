using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance = null;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject();
                _instance = go.AddComponent<AudioManager>();
                go.name = _instance.GetType().ToString();
                _instance.Init();
            }
            return _instance;
        }
    }

    AudioSource _soundSource;
    AudioSource _musicSource;
    Transform tempAudioParent;
    Dictionary<AudioSoundType, AudioClip> sound_clipDict = new Dictionary<AudioSoundType, AudioClip>();
    Dictionary<string, AudioClip> music_clipDict = new Dictionary<string, AudioClip>();

    float _music = 1;
    float _sound = 1;
     
    /// <summary>
    /// 设置音乐大小
    /// </summary>
    public float MusicValue
    {
        get
        {
            return _music;
        }
        set
        {
            _music = value;
            _instance._musicSource.volume = _music;
        }
    }

    /// <summary>
    /// 设置音效大小
    /// </summary>
    public float SoundValue
    {
        get
        {
            return _sound;
        }
        set
        {
            _sound = value;
            _instance._soundSource.volume = _sound;
        }
    }

    public void OnDestory()
    {
        _instance = null;
    }

    void Init()
    {
        GameObject bm = new GameObject("BackgroundMusic");
        bm.transform.SetParent(transform);
        tempAudioParent = new GameObject("AudioParent").transform;
        tempAudioParent.SetParent(transform);

        _musicSource = bm.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _soundSource = _instance.GetComponent<AudioSource>();
        _soundSource.playOnAwake = false;
    }

    AudioSource CreateSoundSource(AudioSoundType soundType)
    {
        GameObject go = new GameObject(soundType.ToString());
        go.transform.SetParent(tempAudioParent);
        AudioSource ase = go.AddComponent<AudioSource>();
        ase.playOnAwake = false;
        return ase;
    }

    public bool IsSoundPlaying
    {
        get
        {
            return _soundSource.isPlaying;
        }
    }

    /// <summary>
    /// 在指定物体上播放音效
    /// </summary>
    /// <param name="soundType"></param>
    /// <param name="pageName"></param>
    /// <param name="ob"></param>
    /// <param name="isLoop"></param>
    public AudioSource PlayTempSound(AudioSoundType soundType, string pageName = null, bool isLoop = false)
    {
        AudioSource ase;
        if (sound_clipDict.ContainsKey(soundType))
            ase = tempAudioParent.Find(soundType.ToString()).GetComponent<AudioSource>();
        else
            ase = CreateSoundSource(soundType);
        ase.clip = GetAudioByType(soundType, pageName);
        ase.loop = isLoop;
        ase.volume = _sound;
        ase.mute = _soundSource.mute;
        ase.Play();
        return ase;
    }

    public AudioSource PlaySound(AudioSoundType soundType, string pageName = null, bool isLoop = false)
    {
        _soundSource.clip = GetAudioByType(soundType, pageName);
        _soundSource.loop = isLoop;
        _soundSource.Play();
        return _soundSource;
    }

    public void StopSound()
    {
        _soundSource.Stop();
    }

    public void PlayMusic(string musicName, string pageName = null)
    {
        _musicSource.clip = GetAudioByType(musicName, pageName);
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void PauseAllSound()
    {
        _musicSource.Pause();
        _soundSource.Pause();
    }

    public void ResumeAllSound()
    {
        _musicSource.UnPause();
        _soundSource.UnPause();
    }

    public void StopAllSound()
    {
        _musicSource.Stop();
        _soundSource.Stop();
    }
    public void MuteAllSound(bool isMute)
    {
        _musicSource.mute=isMute;
        _soundSource.mute=isMute;
    }
    public void ClearAllTempAudio()
    {
        List<AudioSoundType> astTemp = new List<AudioSoundType>();
        foreach (AudioSoundType ast in sound_clipDict.Keys)
            foreach (Transform tf in tempAudioParent)
                if (tf.name == ast.ToString())
                {
                    astTemp.Add(ast);
                    Destroy(tf.gameObject);
                    break;
                }
        foreach (AudioSoundType ast in astTemp)
            sound_clipDict.Remove(ast);
    }

    /// <summary>
    /// 音乐开关
    /// </summary>
    public void SetMusic(int volume)
    {
        //SetNode.SetMusic(volume);
    }

    /// <summary>
    /// 音效开关
    /// </summary>
    public void SetSound(int volume)
    {
        //SetNode.SetSound(volume);
    }

    AudioClip GetAudioByType(string musicName, string pageName)
    {
        AudioClip ac = null;
        if (music_clipDict.ContainsKey(musicName))
            ac = music_clipDict[musicName];
        else
        {
            AssetBundle ab = BundleManager.Instance.GetBundle((string.IsNullOrEmpty(pageName) ? "" : pageName.ToLower() + "/") + "sound/music/" + musicName.ToLower());
            if (ab != null)
            {
                ac = ab.LoadAsset<AudioClip>(musicName.ToLower());
                ab.Unload(false);
            }
            else
                ac = Resources.Load<AudioClip>("Sound/Music/" + musicName);
            music_clipDict.Add(musicName, ac);
        }
        return ac;
    }

    AudioClip GetAudioByType(AudioSoundType soundType, string pageName)
    {
        AudioClip ac = null;
        if (sound_clipDict.ContainsKey(soundType))
            ac = sound_clipDict[soundType];
        else
        {
            AssetBundle ab = BundleManager.Instance.GetBundle((string.IsNullOrEmpty(pageName) ? "" : pageName.ToLower() + "/") + "sound/sound/" + soundType.ToString());
            if (ab != null)
            {
                ac = ab.LoadAsset<AudioClip>(soundType.ToString().ToLower());
                ab.Unload(false);
            }
            else
                ac = Resources.Load<AudioClip>("Sound/Sound/" + soundType.ToString());
            sound_clipDict.Add(soundType, ac);
        }
        return ac;
    }

    public enum AudioSoundType
    {
        None,
        BtnClick,
        BtnReward,
        BtnTizou,
        StartMatch,
        #region 游戏公共
        clock,
        //常用语
        boy_1,
        girl_1,
        boy_2,
        girl_2,
        boy_3,
        girl_3,
        boy_4,
        girl_4,
        boy_5,
        girl_5,
        boy_6,
        girl_6,
        boy_7,
        girl_7,
        boy_8,
        girl_8,
        boy_9,
        girl_9,
        boy_10,
        girl_10,
        boy_11,
        girl_11,
        boy_12,
        girl_12,
        boy_13,
        girl_13,
        boy_14,
        girl_14,
        boy_15,
        girl_15,
        boy_16,
        girl_16,
        #endregion

        #region 斗地主
        Boom,
        Fly,
        Straight,
        JokerBoom,
        ChunTian,
        FanChunTian,
        Lose,
        Win,

        dealCard,
        popCard,

        ToLandlords,

        warring,

        boy1fen,
        girl1fen,

        boy2fen,
        girl2fen,

        boy3fen,
        girl3fen,

        boyBoom,
        girlBoom,

        boyBuyao,
        girlBuyao,

        boyDoubleEight,
        girlDoubleEight,

        boyDoubleFive,
        girlDoubleFive,

        boyDoubleFour,
        girlDoubleFour,

        boyDoubleJack,
        girlDoubleJack,

        boyDoubleKing,
        girlDoubleKing,

        boyDoubleNine,
        girlDoubleNine,

        boyDoubleOne,
        girlDoubleOne,

        boyDoubleQueen,
        girlDoubleQueen,

        boyDoubleSeven,
        girlDoubleSeven,

        boyDoubleSix,
        girlDoubleSix,

        boyDoubleStraight,
        girlDoubleStraight,

        boyDoubleTen,
        girlDoubleTen,

        boyDoubleThree,
        girlDoubleThree,

        boyDoubleTwo,
        girlDoubleTwo,

        boyGuo,
        girlGuo,

        boyJiaoDizhu,
        girlJiaoDizhu,

        boyQiangDizhu,
        girlQiangdizhu,

        boyBuQiang,
        girlBuQiang,

        boyBuJiao,
        girlBuJiao,

        boyJokerBoom,
        girlJokerBoom,

        boyOnecard,
        girlOnecard,

        boyOnlyThree,
        girlOnlyThree,

        boyPASS,
        girlPASS,

        boySingleEight,
        girlSingleEight,

        boySingleFive,
        girlSingleFive,

        boySingleFour,
        girlSingleFour,

        boySingleJack,
        girlSingleJack,

        boySingleKing,
        girlSingleKing,

        boySingleLJoker,
        girlSingleLJoker,

        boySingleNine,
        girlSingleNine,

        boySingleOne,
        girlSingleOne,

        boySingleQueen,
        girlSingleQueen,

        boySingleSeven,
        girlSingleSeven,

        boySingleSix,
        girlSingleSix,

        boySingleTen,
        girlSingleTen,

        boySingleThree,
        girlSingleThree,

        boySingleTwo,
        girlSingleTwo,

        boySingleSJoker,
        girlSingleSJoker,

        boyStraight,
        girlStraight,

        boyThreeAndOne,
        girlThreeAndOne,

        boyThreeAndTwo,
        girlThreeAndTwo,

        boyTripleStraight,
        girlTripleStraight,

        boyTwocard,
        girlTwocard,

        boyYaobuqi,
        girlYaobuqi,
        #endregion
        #region 麻将
        #region  音效
        boychi,
        girlchi,

        boygang,
        girlgang,

        boyhu,
        girlhu,

        boypeng,
        girlpeng,

        boyting,
        girlting,

        boyzimo,
        girlzimo,
        #endregion
        #region  读牌
        girl01,
        girl02,
        girl03,
        girl04,
        girl05,
        girl06,
        girl07,
        girl08,
        girl09,
        girl11,
        girl12,
        girl13,
        girl14,
        girl15,
        girl16,
        girl17,
        girl18,
        girl19,
        girl21,
        girl22,
        girl23,
        girl24,
        girl25,
        girl26,
        girl27,
        girl28,
        girl29,
        girl31,
        boy01,
        boy02,
        boy03,
        boy04,
        boy05,
        boy06,
        boy07,
        boy08,
        boy09,
        boy11,
        boy12,
        boy13,
        boy14,
        boy15,
        boy16,
        boy17,
        boy18,
        boy19,
        boy21,
        boy22,
        boy23,
        boy24,
        boy25,
        boy26,
        boy27,
        boy28,
        boy29,
        boy31,
        #endregion
        #region 特效
        mjaudiohu,
        mjaudioliuju,
        mjaudiotouzi,
        mjaudiozimo,
        mj10,
        mj11,
        mj1,
        mj2,
        mj4,
        mj8,
        mj32,
        mj64,
        mj128,
        mj256,
        mj512,
        mj1024,
        mj2048,
        mj4096,
        mjresultlose,
        mjresultwin,
        mjgetcard,
        mjoutcard,
        mjonclick,
        #endregion
        #endregion
    }
}
