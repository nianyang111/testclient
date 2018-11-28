using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class SequenceAnimation : MonoBehaviour
{
    private Image ImageSource;
    private int mCurFrame = 0;
    private float mDelta = 0;

    public float FPS = 5;
    public List<Sprite> SpriteFrames;
    public bool IsPlaying = false;
    public bool Foward = true;
    public bool AutoPlay = false;
    public bool PingPong = false;
    public bool Loop = false;
    public bool isSetNativeSize = true;
    public UnityAction onPlayEndCall;//播放结束的回调
    Dictionary<int, UnityAction> diu = new Dictionary<int, UnityAction>();


    public int FrameCount
    {
        get
        {
            return SpriteFrames.Count;
        }
    }

    void Awake()
    {
        ImageSource = GetComponent<Image>();
    }

    void OnEnable()
    {
        if (AutoPlay)
            Play();
    }

    private void SetSprite(int idx)
    {
        ImageSource.sprite = SpriteFrames[idx];
        if (isSetNativeSize)
            ImageSource.SetNativeSize();
    }

    public void Play()
    {
        IsPlaying = true;
        Foward = true;
        StopAllCoroutines();
        StartCoroutine(PlayAnimation());
    }

    public void PlayReverse()
    {
        IsPlaying = true;
        Foward = false;
    }


    IEnumerator PlayAnimation()
    {
        mCurFrame = 0;
        while (true)
        {
            if (!IsPlaying || 0 == FrameCount)
                break;
            yield return new WaitForEndOfFrame();
            mDelta += Time.deltaTime;
            if (mDelta > 1 / FPS)
            {
                mDelta = 0;
                if (Foward)
                    mCurFrame++;
                else
                    mCurFrame--;

                if (mCurFrame >= FrameCount)
                {
                    if (Loop)
                    {
                        if (PingPong)
                        {
                            Foward = false;
                            mCurFrame = FrameCount - 1;
                        }
                        else
                            mCurFrame = 0;
                    }
                    else
                    {
                        IsPlaying = false;
                        if (onPlayEndCall != null)
                            onPlayEndCall();
                        break;
                    }
                }
                else if (mCurFrame < 0)
                {
                    if (Loop)
                    {
                        if (PingPong)
                        {
                            Foward = true;
                            mCurFrame = 0;
                        }
                        else
                            mCurFrame = FrameCount - 1;
                    }
                    else
                    {
                        IsPlaying = false;
                        if (onPlayEndCall != null)
                            onPlayEndCall();
                        break;
                    }
                }
                SetSprite(mCurFrame);
                if (diu.ContainsKey(mCurFrame))
                    diu[mCurFrame]();
            }
        }
    }

    public void Pause()
    {
        IsPlaying = false;
    }

    public void Continue()
    {
        if (!IsPlaying)
            IsPlaying = true;
    }

    public void Stop()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        IsPlaying = false;
    }

    public void Rewind()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        Play();
    }

    public void AddActionAtFrame(int frame, UnityAction ua)
    {
        diu.Add(frame, ua);
    }
}
