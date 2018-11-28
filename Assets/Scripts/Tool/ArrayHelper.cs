using System;
using System.Collections.Generic;


/// <summary>
/// 通用的数组助手类
/// </summary>
public class ArrayHelper
{

    /// <summary>
    /// 查找时用于比较的委托
    /// </summary>
    /// <param name="item">比较的对象</param>
    /// <returns>比较的结果</returns>
    public delegate bool FindHandler<T>(T item);

    /// <summary>
    /// 从对象中提取或生成关键字可用于排序
    /// </summary>
    /// <param name="sourceObj">源对象</param>
    /// <returns>关键字</returns>
    public delegate TKey SelectHandler<T, TKey>(T sourceObj);

    /// <summary>
    /// 查找满足条件的单个对象
    /// </summary>
    /// <param name="array">源数组</param>
    /// <param name="handler">查找条件</param>
    /// <returns>找到的单个对象</returns>
    public static T Find<T>(T[] array, FindHandler<T> handler)
    {
        foreach (var item in array)
        {
            if (handler(item))
                return item;
        }
        return default(T);
    }

    /// <summary>
    /// 查找满足条件的多个对象
    /// </summary>
    /// <param name="array">源数组</param>
    /// <param name="handler">查找条件</param>
    /// <returns>找到的多个对象</returns>
    public static T[] FindAll<T>(T[] array, FindHandler<T> handler)         
    {
        List<T> tempList = new List<T>();
        foreach (var item in array)
        {
            if (handler(item))
                tempList.Add(item);
        }
        return tempList.Count > 0 ? tempList.ToArray() : null;
    }

    /// <summary>
    /// 对数组做升序排列
    /// </summary> 
    /// <param name="array">源数组</param>
    public static void OrderBy<T, TKey>(T[] array, SelectHandler<T, TKey> handler)
       where TKey : IComparable, IComparable<TKey>
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
        }
    }


    /// <summary>
    /// 对数组做降序排列
    /// </summary> 
    /// <param name="array">源数组</param>
    public static void OrderByDescending<T, TKey>(T[] array, SelectHandler<T, TKey> handler)
       where TKey : IComparable, IComparable<TKey>
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                if (handler(array[i]).CompareTo(handler(array[j])) < 0)
                {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
        }
    }

    /// <summary>
    /// 找出数据中最大的元素
    /// </summary>
    /// <param name="array">源数组</param>
    /// <param name="handler">比较时所用的关键字</param>
    /// <returns>最大的元素</returns>
    public static T Max<T, TKey>(T[] array, SelectHandler<T, TKey> handler)
       where TKey : IComparable, IComparable<TKey>
    {
        T max = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (handler(array[i]).CompareTo(handler(max)) > 0)
                max = array[i];
        }
        return max;
    }

    /// <summary>
    /// 找出数据中最小的元素
    /// </summary>
    /// <param name="array">源数组</param>
    /// <param name="handler">比较时所用的关键字</param>
    /// <returns>最小的元素</returns>
    public static T Min<T, TKey>(T[] array, SelectHandler<T, TKey> handler)
              where TKey : IComparable, IComparable<TKey>
    {
        T min = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (handler(array[i]).CompareTo(handler(min)) < 0)
                min = array[i];
        }
        return min;
    }

    /// <summary>
    /// 从数组每个元素中提取部分内容组成一个新的数组
    /// </summary>
    /// <param name="array">源数组</param>
    /// <param name="handler">提取算法</param>
    /// <returns>组成的新数组</returns>
    public static TKey[] Select<T, TKey>(T[] array, SelectHandler<T, TKey> handler)
    {
        TKey[] arr = new TKey[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            arr[i] = handler(array[i]);
        }
        return arr;
    }


}

