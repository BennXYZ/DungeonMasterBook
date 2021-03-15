using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Campaign
{
    public string name;
    public string path;
    public List<Page> pages;
    public List<Tag> tags;

    public bool TagExists(string tag)
    {
        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i].name == tag)
                return true;
        }
        return false;
    }

    public List<Page> GetPagesByCurrentTags()
    {
        List<Page> taggedPages = new List<Page>();
        bool containsAllTags = true;
        for (int i = 0; i < pages.Count; i++)
        {
            containsAllTags = true;
            for (int j = 0; j < tags.Count(t => t.active); j++)
            {
                if(!pages[i].tags.Contains(tags.Where(t => t.active).ToList()[j].name))
                {
                    containsAllTags = false;
                }
            }
            if(containsAllTags)
            {
                taggedPages.Add(pages[i]);
            }
        }
        return taggedPages;
    }

    public Page GetPageById(int id)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i].id == id)
                return pages[i];
        }
        return null;
    }

    public int FirstAvailableId()
    {
        int lowestId = 0;
        bool idIsNew = true;
        do
        {
            idIsNew = true;
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].id == lowestId)
                {
                    idIsNew = false;
                    lowestId++;
                }
            }

        } while (!idIsNew);
        return lowestId;
    }

    public Campaign()
    {
        name = "New Campaign";
        pages = new List<Page>();
        tags = new List<Tag>();
    }

    public Campaign(CampaignData data)
    {
        name = data.name;
        path = data.path;
        pages = new List<Page>();
        tags = new List<Tag>();
        int totalCount = data.tags.Length + data.pages.Length;

        for (int i = 0; i < data.tags.Length; i++)
        {
            tags.Add(new Tag(data.tags[i]));
        }
        for (int i = 0; i < data.pages.Length; i++)
        {
            pages.Add(new Page(data.pages[i]));
        }
    }
}


[System.Serializable]
public class CampaignData
{
    public string name;
    public string path;
    public string[] tags;
    public PageData[] pages;

    public CampaignData()
    {
    }

    public CampaignData(Campaign campaign)
    {
        name = campaign.name;
        path = campaign.path;
        tags = new string[campaign.tags.Count];
        for (int i = 0; i < campaign.tags.Count; i++)
        {
            tags[i] = campaign.tags[i].name;
        }
        pages = new PageData[campaign.pages.Count];
        for (int i = 0; i < campaign.pages.Count; i++)
        {
            pages[i] = new PageData(campaign.pages[i]);
        }
    }
}