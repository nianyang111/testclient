using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreGoldItem : MonoBehaviour {
    public Text itemName,rmbNum,replaceNum;
    public Image  itemIcon;
    public Button itemBg;
    public StoreGoldPanel _panel;
    StoreGoldData _data;
    public void Init(StoreGoldData data)
    {
        _data = data;
        itemName.text = string.Format(_data.itemNum + "金条");
        rmbNum.text = string.Format(_data.rmbNum + "元购买"); 
        replaceNum.text = _data.replaceNum.ToString();
        itemIcon.sprite = BundleManager.Instance.GetSprite("Store/"+_data.itemIcon);
        itemIcon.SetNativeSize();
        itemBg.onClick.AddListener( delegate {
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            _panel.rechargePanel.Open(_data); 
        });
    }

}
public class StoreGoldData
{
    public string itemIcon;
    public int itemNum;
    public int ratioNum;
    public int rmbNum;
    public int  replaceNum;
    public int id;
}
