using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatBallItem : MonoBehaviour
{
    public Button btn;
    public Image icon;
    public Text title;
    public FloatBallNode _node;
    public FloatBallData _data;
    public void Init(FloatBallData data)
    {
        _data = data;
        title.text = _data.activityType;
        icon.sprite = BundleManager.Instance.GetSprite(_data.activityIcon);
        btn.onClick.AddListener(() =>
        {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            OnClickBtn();
        });
    }
    private void OnClickBtn()
    {

        if (_data.special == "Page")
            PageManager.Instance.OpenPage(_data.activityLink);
        if (_data.special == "Node")
        {
            if (_data.activityLink == "StoreNode")
                NodeManager.OpenNode(_data.activityLink, null, () => NodeManager.GetNode<StoreNode>().vipBtn.isOn = true);
            else
                NodeManager.OpenNode(_data.activityLink);
        }
        _node.SelectShow();
    }
    public void SetRotation(float a)
    {
        transform.localEulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z - a);
    }
}
public class FloatBallData
{
    public string activityIcon;
    public string activityLink;
    public string activityType;
    public int activityAngle;
    public string special;
}