using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PagesPanel : MonoBehaviour
{
    public Transform pagesParent;
    public GameObject panelPrefab;
    public TMP_InputField nameSearch;

    List<PagePanelItem> items;

    private void Awake()
    {
        items = new List<PagePanelItem>();
    }

    public void LoadPages()
    {
        Clear();
        for (int i = 0; i < GameManager.CurrentCampaign.pages.Count; i++)
        {
            CreatePageItem(GameManager.CurrentCampaign.pages[i]);
        }
    }

    public void FilterPages()
    {
        bool badTagMatch = false;
        for (int i = 0; i < items.Count; i++)
        {
            if(!items[i].page.name.ToLower().Contains(nameSearch.text.ToLower()))
            {
                items[i].gameObject.SetActive(false);
                continue;
            }
            badTagMatch = false;
            for (int j = 0; j < GameManager.CurrentCampaign.tags.Count; j++)
            {
                if(GameManager.CurrentCampaign.tags[j].active && !items[i].page.tags.Contains(GameManager.CurrentCampaign.tags[j].name))
                {
                    badTagMatch = true;
                    break;
                }
            }
            items[i].gameObject.SetActive(!badTagMatch);
        }
    }

    public void CreatePageItem(Page page)
    {
        PagePanelItem newItem = Instantiate(panelPrefab, pagesParent).GetComponent<PagePanelItem>();
        if (newItem)
        {
            newItem.Init(page);
            newItem.onOpenPage.AddListener(delegate { GameManager.Instance.OpenPage(newItem.page.id); });
            newItem.onRemove.AddListener(delegate { RemoveItem(newItem); });
            items.Add(newItem);
        }
    }

    private void RemoveItem(PagePanelItem item)
    {
        for (int i = 0; i < GameManager.CurrentCampaign.pages.Count; i++)
        {
            GameManager.CurrentCampaign.pages[i].links.Remove(item.page.id);
        }
        if (GameManager.Instance.mainPanel.currentPageId == item.page.id)
        {
            GameManager.Instance.mainPanel.CloseCurrentPage();
            GameManager.Instance.linksPanel.Clear();
        }
        else
        {
            GameManager.Instance.linksPanel.SetLinksToCurrentPage();
        }
        GameManager.CurrentCampaign.pages.Remove(item.page);
        items.Remove(item);
        Destroy(item.gameObject);
    }

    public void Clear()
    {
        foreach(Transform child in pagesParent)
        {
            Destroy(child.gameObject);
        }
        if(items != null)
        {
            items.Clear();
        }
    }

    public void UpdateTags()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].UpdateTagText();
        }
    }
}
