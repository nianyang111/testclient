using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tab : Selectable
{
    public bool isOn;
    public GameObject targetPage;
    public TabPageGroup tabPageGroup;
    public GameObject selectBackground;

    protected override void Start()
    {
        Init();
        UGUIEventListener.Get(gameObject).onClick += delegate
        {
            if (!this.Equals(tabPageGroup.currentSelectTab))
            {
                isOn = true;
                if (targetPage)
                    targetPage.SetActive(true);
                if (selectBackground)
                    selectBackground.SetActive(true);

                if (tabPageGroup.currentSelectTab)
                {
                    tabPageGroup.currentSelectTab.isOn = false;
                    tabPageGroup.currentSelectTab.selectBackground.SetActive(false);
                }
                if (tabPageGroup.currentDisplayPage)
                    tabPageGroup.currentDisplayPage.SetActive(false);

                tabPageGroup.currentSelectTab = this;
                tabPageGroup.currentDisplayPage = targetPage;
            }
        };
    }

    void Init()
    {
        if (isOn)
        {
            if (targetPage)
                targetPage.SetActive(true);
            if (selectBackground)
                selectBackground.SetActive(true);
            if (tabPageGroup)
            {
                tabPageGroup.currentSelectTab = this;
                tabPageGroup.currentDisplayPage = targetPage;
            }
        }
        else
        {
            if (targetPage)
                targetPage.SetActive(false);
            if (selectBackground)
                selectBackground.SetActive(false);
        }
    }
}
