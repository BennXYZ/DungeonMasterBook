using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagetagSelection : MonoBehaviour
{
    public Transform tagsParent;

    public GameObject tagsPrefab;

    public MainPanelBehaviour mainPanel;

    List<TagItem> items;

    bool initialized = false;

    private void Awake()
    {
        if(!initialized)
        {
            items = new List<TagItem>();
            Clear();
            initialized = true;
        }
    }

    public void Initialize()
    {
        if (!initialized)
        {
            items = new List<TagItem>();
            Clear();
            initialized = true;
        }
    }

    public void Clear()
    {
        foreach(Transform child in tagsParent)
        {
            Destroy(child.gameObject);
        }
        if(items != null)
        {
            items.Clear();
        }
    }

    public void SetPageTags()
    {
        bool tagFound = false;
        if (items != null)
            for (int i = 0; i < items.Count; i++)
            {
                tagFound = false;

                if (GameManager.CurrentCampaign.GetPageById(mainPanel.currentPageId) != null)
                {
                    for (int j = 0; j < GameManager.CurrentCampaign.GetPageById(mainPanel.currentPageId).tags.Count; j++)
                    {
                        if (items[i].nameDisplay.text == GameManager.CurrentCampaign.GetPageById(mainPanel.currentPageId).tags[j])
                        {
                            tagFound = true;
                            break;
                        }
                    }
                }

                items[i].toggleValue = tagFound;
                items[i].toggle.isOn = tagFound;
            }
    }

    public void UpdateTags()
    {
        Clear();
        for (int i = 0; i < GameManager.CurrentCampaign.tags.Count; i++)
        {
            AddTag(GameManager.CurrentCampaign.tags[i].name, false);
        }
        SetPageTags();
    }

    public void AddTag(string name, bool startValue)
    {
        TagItem newItem = Instantiate(tagsPrefab, tagsParent).GetComponent<TagItem>();
        if(newItem)
        {
            newItem.nameDisplay.text = name;
            newItem.toggleValue = startValue;
            newItem.toggle.isOn = startValue;
            newItem.onToggle.AddListener(delegate { ToggleTag(newItem.nameDisplay.text, newItem.toggleValue); });
            if(items == null)
            {
                items = new List<TagItem>();
            }
            items.Add(newItem);
        }
    }

    public void ToggleTag(string name, bool value)
    {
        if(value)
        {
            if(!GameManager.CurrentCampaign.GetPageById(mainPanel.currentPageId).tags.Contains(name))
                GameManager.CurrentCampaign.GetPageById(mainPanel.currentPageId).tags.Add(name);
        }
        else
        {
            GameManager.CurrentCampaign.GetPageById(mainPanel.currentPageId).tags.Remove(name);
        }
        GameManager.Instance.pagesPanel.UpdateTags();
        GameManager.Instance.pagesPanel.FilterPages();
    }
}