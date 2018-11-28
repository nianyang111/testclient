using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreAgItem : MonoBehaviour {
    public Text itemName, rmbNum, replaceNum;
    public Image itemIcon;
    public Button itemBg;
    public StoreAgPanel _panel;
    StoreAgData _data;
    public void Init(StoreAgData data)
    {
        _data = data;
        itemName.text = _data.itemName;
        rmbNum.text = string.Format(_data.rmbNum + "元购买");
        replaceNum.text = _data.replaceNum.ToString();
        itemIcon.sprite = BundleManager.Instance.GetSprite("Store/"+_data.itemIcon);
        itemIcon.SetNativeSize();
        itemBg.onClick.AddListener(delegate {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            _panel.rechargePanel.Open(_data); 
        });
    }
}
public class StoreAgData
{
    public string itemName;
    public string itemIcon;
    public int itemNum;
    public int ratioNum;
    public int rmbNum;
    public int replaceNum;
    public int id;
}
