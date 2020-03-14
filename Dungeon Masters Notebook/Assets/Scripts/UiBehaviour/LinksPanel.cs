using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinksPanel : MonoBehaviour
{
    public Transform linksParent;

    public GameObject linksPrefab;

    public GameObject linksNotification;

    List<PagePanelItem> items;

    public GameObject windowBlocker;

    int currentId;

    private void Awake()
    {
        Clear();
    }

    public void Clear()
    {
        foreach(Transform child in linksParent)
        {
            Destroy(child.gameObject);
        }
        if (items != null)
            items.Clear();
    }

    public void SetLinksToCurrentPage()
    {
        Clear();
        if(GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId) != null)
        {
            for (int i = 0; i < GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).links.Count; i++)
            {
                CreateLink(GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).links[i]);
            }
        }
    }

    public LinksPanel()
    {
        items = new List<PagePanelItem>();
    }

    public void SelectLink()
    {
        GameManager.isSelectingLink = !GameManager.isSelectingLink;
        linksNotification.gameObject.SetActive(GameManager.isSelectingLink);
    }

    public void CreateLink(int id)
    {
        PagePanelItem newItem = Instantiate(linksPrefab, linksParent).GetComponent<PagePanelItem>();
        if (newItem)
        {
            newItem.Init(GameManager.CurrentCampaign.GetPageById(id));
            newItem.onOpenPage.AddListener(delegate { GameManager.Instance.OpenPage(newItem.page.id); });
            newItem.onRemove.AddListener(delegate { RemoveItem(newItem); });
            items.Add(newItem);
        }
    }

    private void RemoveItem(PagePanelItem newItem)
    {
        if(newItem.page.pageType == PageTypes.Map)
        {
            for (int i = 1; i < newItem.page.texts.Count; i++)
            {
                if(MapItemDataConverter.GetId(newItem.page.texts[i]) == GameManager.Instance.mainPanel.currentPageId)
                {
                    newItem.page.texts.RemoveAt(i);
                    break;
                }
            }
        }
        else
        {
            GameManager.Instance.currentCampaign.GetPageById(newItem.page.id).links.Remove(GameManager.Instance.mainPanel.currentPageId);
        }
        GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).links.Remove(newItem.page.id);
        items.Remove(newItem);
        Destroy(newItem.gameObject);
    }

    public void AddLink(int id)
    {
        if(!GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).links.Contains(id))
        {
            GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).links.Add(id);
            CreateLink(id);
        }

        GameManager.isSelectingLink = false;
    }

    public void CreateOneWayLink()
    {
        if (!GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).links.Contains(currentId))
        {
            GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).links.Add(currentId);
            CreateLink(currentId);
        }

        GameManager.isSelectingLink = false;
    }

    public void CreateTwoWayLink()
    {
        if(!GameManager.CurrentCampaign.GetPageById(currentId).links.Contains(GameManager.Instance.mainPanel.currentPageId))
        {
            GameManager.CurrentCampaign.GetPageById(currentId).links.Add(GameManager.Instance.mainPanel.currentPageId);
        }

        if (!GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).links.Contains(currentId))
        {
            GameManager.CurrentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).links.Add(currentId);
            CreateLink(currentId);
        }

        GameManager.isSelectingLink = false;
    }

    internal void SelectId(int id)
    {
        currentId = id;
        //windowBlocker.SetActive(true);
        GameManager.isSelectingLink = !GameManager.isSelectingLink;
        CreateTwoWayLink();
        linksNotification.gameObject.SetActive(GameManager.isSelectingLink);
        //acceptPanel.SetActive(true);
    }
}
