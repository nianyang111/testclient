using UnityEngine;
using System.Collections.Generic;
using System.Collections;




public class SetTimeout : MonoBehaviour
{

    public static SetTimeout inst;

    private void Start()
    {
        inst = this;
    }
    public struct CallbackInfo
    {
        public CallBack call;
        public float dely;
        public float endTime;
        public bool once;
    }


    private static List<CallbackInfo> mCallbackList = new List<CallbackInfo>();



    public static void add(float delay, CallBack handler, bool once = true)
    {
        if (!contains(handler))
        {
            mCallbackList.Add(new CallbackInfo() { call = handler, dely = delay, endTime = Time.realtimeSinceStartup + delay, once = once });
        }
    }

    public static bool remove(CallBack handler)
    {
        for (int i = mCallbackList.Count - 1; i >= 0; i--)
        {
            if (mCallbackList[i].call == handler)
            {
                mCallbackList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public static void RemoveAll()
    {
        mCallbackList.Clear();
    }

    public void add2(float delay, CallBack handler)
    {
        StartCoroutine(Add2(delay, handler));
    }

    IEnumerator Add2(float delay, CallBack handler)
    {
        yield return new WaitForSeconds(delay);
        handler();
    }

    public static bool contains(CallBack handler)
    {
        for (int i = 0; i < mCallbackList.Count; i++)
        {
            if (mCallbackList[i].call == handler)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        for (int i = 0; i < mCallbackList.Count; i++)
        {
            if (mCallbackList[i].call == null)
            {
                mCallbackList.RemoveAt(i);
                i--;
            }
            else if (Time.realtimeSinceStartup >= mCallbackList[i].endTime)
            {
                CallbackInfo info = mCallbackList[i];
                if (!mCallbackList[i].once)
                {
                    info.endTime += info.dely;
                    mCallbackList[i] = info;
                }
                else
                    mCallbackList.RemoveAt(i);
                info.call();
                i--;
            }
        }
    }


}



