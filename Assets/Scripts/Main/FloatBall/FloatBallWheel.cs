using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatBallWheel : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public FloatBallItem[] itemArryer;

    private FloatBallItem curItem;
	public void Init(FloatBallNode node){
        try
        {
            string assetsText = BundleManager.Instance.GetJson(ConstantUtils.floatBallConfig);
            LitJson.JsonData jd = LitJson.JsonMapper.ToObject(assetsText);
            for (int i = 0; i < jd.Count; i++)
            {
                FloatBallData data = LitJson.JsonMapper.ToObject<FloatBallData>(LitJson.JsonMapper.ToJson(jd[i]));
                itemArryer[i]._node = node;
                itemArryer[i].Init(data);
            }
        }
        catch (Exception)
        {
            TipManager.Instance.OpenTip(TipType.SimpleTip, "配置读取失败");
        }
        
	}
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 curScreenPosition = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, transform.position);
        Vector2 directionTo = curScreenPosition - eventData.position;
        Vector2 directionFrom = directionTo - eventData.delta*3;
        transform.rotation *= Quaternion.FromToRotation(directionTo, directionFrom);
        for(int i = 0; i < itemArryer.Length; i++)
        {
            itemArryer[i].SetRotation(transform.localEulerAngles.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float angle = transform.eulerAngles.z;
        if (angle>315&&angle <360)
            angle = 0;
        curItem = Min<FloatBallItem, float>(itemArryer, p => Mathf.Abs(p._data.activityAngle - angle));
        StartCoroutine(PlayAnim());
    }

    IEnumerator PlayAnim()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        float speed = 0;
        float progress = 0;
        speed =0.1f;
        float angle = curItem._data.activityAngle;
        if (transform.localEulerAngles.z > 315 && angle == 0)
            angle = 360;
        while (progress<1)
        {
            progress += speed;
            if (progress > 1)
                progress = 1;
            yield return new WaitForSeconds(0.01f);
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0, 0, angle), progress);
            for (int i = 0; i < itemArryer.Length; i++)
            {
                itemArryer[i].SetRotation(transform.localEulerAngles.z);
            }
        }
    }
    /// <summary>
    /// 从对象中提取或生成关键字可用于排序
    /// </summary>
    /// <param name="sourceObj">源对象</param>
    /// <returns>关键字</returns>
    public delegate TKey SelectHandler<T, TKey>(T sourceObj);
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
}
