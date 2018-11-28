using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreCardItem : MonoBehaviour {
    public Text itemName, rmbNum;
    public Button itemBg;
    public List<GameObject> IconList;
    public StoreCardPanel _panel;
    StoreCardData _data;
    public void Init(StoreCardData data)
    {
        _data = data;
        itemName.text = _data.itemName;
        rmbNum.text =string .Format( _data.rmbNum+"元购买");
        for (int i = 0; i < IconList.Count; i++)
        {
            IconList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < _data.iconNum; i++)
        {
            IconList[i].gameObject.SetActive(true);
        }
        itemBg.onClick.AddListener(delegate{
            if (!AudioManager.Instance.IsSoundPlaying)
                AudioManager.Instance.PlaySound(AudioManager.AudioSoundType.BtnClick);
            _panel.rechargePanel.Open(_data); 
        });
    }
}
public class StoreCardData
{
    public string itemName;
    public int cardNum;
    public int rmbNum;
    public int iconNum;
    public int id;
}
