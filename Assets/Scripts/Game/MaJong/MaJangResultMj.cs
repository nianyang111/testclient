using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaJangResultMj : MonoBehaviour {
    public Image icon;

    public void Init(string mjNo)
    {
        gameObject.SetActive(true);
        icon.sprite = BundleManager.Instance.GetSprite(mjNo, MaJangPage.Instance.majangBundle);
    }
}
