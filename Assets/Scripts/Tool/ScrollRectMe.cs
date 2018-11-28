using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ScrollRectMe : ScrollRect
{
    [SerializeField]
    public float inertiaMaxTime = 0f;//限制惯性持续时间

    public Action<GameObject> stopScrollCallback = null;//滑动结束的回调

    private float _scrolledTime = 0f;

    private Action<GameObject> _stopScrollCallback = null;

    private Vector2 _lastPostion = Vector2.zero;

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        _stopScrollCallback = stopScrollCallback;
        _scrolledTime = 0f;
        _lastPostion = Vector2.zero;
        _stopScrollCallback(transform.gameObject);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        _scrolledTime = 0f;
        _lastPostion = Vector2.zero;
    }

    public override void StopMovement()
    {
        base.StopMovement();
    }

    protected override void LateUpdate()
    {
        // base.LateUpdate();
    }

    private void Update()
    {
        base.LateUpdate();
    }

    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        //2017-6-27 修改补充条件
        if (_scrolledTime >= inertiaMaxTime || (position.ToString("0.0").CompareTo(_lastPostion.ToString("0.0"))) == 0)
        {
            if (_stopScrollCallback != null)
            {
                _stopScrollCallback(transform.gameObject);
                _stopScrollCallback = null;
            }
            _scrolledTime = inertiaMaxTime;
            return;
        }

        base.SetContentAnchoredPosition(position);

        _scrolledTime += Time.unscaledDeltaTime;
        _lastPostion = position;
    }
}