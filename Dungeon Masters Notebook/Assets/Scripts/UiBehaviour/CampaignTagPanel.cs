using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignTagPanel : MonoBehaviour
{
    public Transform tagsParent;

    public GameObject tagPrefab;

    public PagetagSelection pagetags;

    public List<TagItem> items;

    private void Awake()
    {
        items = new List<TagItem>();
        Clear();
    }

    public void LoadTags()
    {
        Clear();
        for (int i = 0; i < GameManager.CurrentCampaign.tags.Count; i++)
        {
            CreateTag(GameManager.CurrentCampaign.tags[i].name);
        }
    }

    public void Clear()
    {
        foreach(Transform child in tagsParent)
        {
            Destroy(child.gameObject);
        }
        if(items != null)
            items.Clear();
    }

    public void CreateTag(string name)
    {
        TagItem newItem = Instantiate(tagPrefab, tagsParent).GetComponent<TagItem>();
        newItem.transform.SetAsLastSibling();
        newItem.nameInput.text = name;
        newItem.memorizedName = name;
        newItem.toggleValue = false;
        newItem.toggle.isOn = false;
        newItem.onToggle.AddListener(delegate { OnToggle(newItem); }) ;
        newItem.onNameEdit.AddListener(delegate { OnTagEdit(newItem); });
        newItem.onRemove.AddListener(delegate { OnTagRemove(newItem); });
        items.Add(newItem);
    }

    public void OnTagRemove(TagItem item)
    {
        for (int i = 0; i < GameManager.CurrentCampaign.tags.Count; i++)
        {
            if(GameManager.CurrentCampaign.tags[i].name == item.nameInput.text)
            {
                GameManager.CurrentCampaign.tags.RemoveAt(i);
                break;
            }
        }
        items.Remove(item);
        Destroy(item.gameObject);
        pagetags.UpdateTags();
    }

    public void OnTagEdit(TagItem item)
    {
        pagetags.UpdateTags();
    }

    public void AddTag()
    {
        GameManager.CurrentCampaign.tags.Add(new Tag("New Tag"));
        CreateTag("New Tag");
        pagetags.UpdateTags();
        items[items.Count - 1].nameInput.Select();
    }

    public void OnToggle(TagItem item)
    {
        for (int i = 0; i < GameManager.CurrentCampaign.tags.Count; i++)
        {
            if(GameManager.CurrentCampaign.tags[i].name == item.nameInput.text)
            {
                GameManager.CurrentCampaign.tags[i].active = item.toggleValue;
            }
        }
        GameManager.Instance.pagesPanel.FilterPages();
    }
}
