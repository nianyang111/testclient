using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class InputUtils : MonoBehaviour
{
    public static bool OnHeld(int index = 0)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WebGLPlayer:
                return Input.GetMouseButton(index);
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                return Input.touchCount > index && (Input.GetTouch(index).phase == TouchPhase.Moved || Input.GetTouch(index).phase == TouchPhase.Stationary);
        }

        return false;
    }

    public static bool OnPressed(int index = 0)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WebGLPlayer:
                return Input.GetMouseButtonDown(index);
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                return Input.touchCount > index && Input.GetTouch(index).phase == TouchPhase.Began;
        }

        return false;
    }

    public static bool OnReleased(int index = 0)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WebGLPlayer:
                return Input.GetMouseButtonUp(index);
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                return Input.touchCount > index && (Input.GetTouch(index).phase == TouchPhase.Ended || Input.GetTouch(index).phase == TouchPhase.Canceled);
        }

        return false;
    }

    public static Vector2 GetTouchPosition(int index = 0)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WebGLPlayer:
                return Input.mousePosition;
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                if (index < Input.touchCount)
                    return Input.GetTouch(index).position;
                break;
        }

        return Vector2.zero;
    }

    public static Vector2 GetTouchDeltaPosition(int index = 0)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WebGLPlayer:
                return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                if (index < Input.touchCount)
                    return Input.GetTouch(index).deltaPosition;
                return Vector2.zero;
        }

        return Vector2.zero;
    }

    public static void GetTouchEscape()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                if (Input.GetKey(KeyCode.Escape))
                    TipManager.Instance.OpenTip(TipType.ChooseTip, "确定要退出游戏吗？", 0, () => { Application.Quit(); });
                    break;
        }
    }

    /// <summary>
    /// 是否触摸在UI上
    /// </summary>
    public static bool IsPointerOverUIObject()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    /// <param name="fingerID">该参数即使触摸手势的 id</param>
    public static bool IsPointerOverUIObject(int fingerID)
    {
        return EventSystem.current.IsPointerOverGameObject(fingerID);
    }


    /// <param name="fingerID">通过UI事件发射射线 是 2D UI 的位置，非 3D 位置</param>
    /// <returns></returns>
    public static bool IsPointerOverUIObject(Vector2 screenPosition)
    {
        //实例化点击事件
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        //将点击位置的屏幕坐标赋值给点击事件
        eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        //向点击处发射射线
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    /// <summary>
    /// 是否触摸在UI上
    /// </summary>
    /// <param name="fingerID">通过画布上的 GraphicRaycaster 组件发射射线</param>
    /// <returns></returns>
    public static bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
    {
        //实例化点击事件
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        //将点击位置的屏幕坐标赋值给点击事件
        eventDataCurrentPosition.position = screenPosition;
        //获取画布上的 GraphicRaycaster 组件
        GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();

        List<RaycastResult> results = new List<RaycastResult>();
        // GraphicRaycaster 发射射线
        uiRaycaster.Raycast(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}
