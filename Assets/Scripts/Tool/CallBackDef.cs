using UnityEngine;
using System.Collections;

public delegate void CallBack();

public delegate void CallBack<T>(T arg);

public delegate void CallBack<T,T1>(T arg0,T1 arg1);

public delegate void CallBack<T,T1,T2>(T arg0,T1 arg1,T2 arg2);

public delegate void CallBack<T,T1,T2,T3>(T arg0,T1 arg1,T2 arg2,T3 arg3);

public delegate bool BoolCallBack();