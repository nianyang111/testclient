using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreVipItem : MonoBehaviour {
    public Button itemBg;
    public Text itemName, rmbNum, hintText;
    public Image itemIcon;
    public StoreVipPanel _panel;
    StoreVipData _data;
    public void Init(StoreVipData data)
    {
        _data = data;
        itemName.text = _data.itemName;
        itemIcon.sprite = BundleManager.Instance.GetSprite("Store/"+_data.itemIcon);
        hintText.text = _data.itemHint;
        rmbNum.text = string.Format(_data.rmbNum + "元");
        itemBg.onClick.AddListener( delegate{
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            _panel.rechargePanel.Open(_data);
        });
    }
}
public class StoreVipData
{
    public string itemName;
    public string itemIcon;
    public string itemHint;
    public int vipDay;
    public int rmbNum;
    public int goldNum;
    public int id;
}
