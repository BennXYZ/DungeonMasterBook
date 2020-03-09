using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Page
{
    public string name;
    public int id;
    public List<string> imagePaths;
    public List<string> tags;
    public List<string> texts;
    public List<int> links;
    public PageTypes pageType;

    public UnityEvent onPageChanged;
    public UnityEvent onPageDeleted;

    public List<Sprite> textures;

    public Page(int id, List<Tag> currentTags, PageTypes type)
    {
        onPageChanged = new UnityEvent();
        this.id = id;
        pageType = type;
        name = "New Page";
        tags = new List<string>();
        textures = new List<Sprite>();
        for (int i = 0; i < currentTags.Count; i++)
        {
            tags.Add(currentTags[i].name);
        }
        imagePaths = new List<string>();
        texts = new List<string>();
        links = new List<int>();

        SetStartList();
    }

    private void SetStartList()
    {
        int numberOfTexts = 0;
        int numberOfImages = 0;
        switch (pageType)
        {
            case PageTypes.Blank:
                numberOfTexts = 1;
                numberOfImages = 1;
                break;
            case PageTypes.Character:
                numberOfTexts = 6;
                numberOfImages = 1;
                break;
        }
        for (int i = 0; i < numberOfTexts; i++)
        {
            texts.Add(string.Empty);
        }
        for (int i = 0; i < numberOfImages; i++)
        {
            imagePaths.Add(string.Empty);
            textures.Add(null);
        }
    }

    public Page(PageData data)
    {
        onPageChanged = new UnityEvent();
        id = data.id;
        name = data.name;
        pageType = (PageTypes)Enum.Parse(typeof(PageTypes), data.pageType);
        tags = data.tags.ToList();
        imagePaths = data.imagePaths.ToList();
        textures = new List<Sprite>();
        for (int i = 0; i < imagePaths.Count; i++)
        {
            if(!string.IsNullOrEmpty(imagePaths[i]))
            {
                try
                {
                    WWW www = new WWW("file:///" + imagePaths[i]);
                    textures.Add(Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f));
                }
                catch
                {
                    Debug.LogWarning("Picture not Found. Maybe it was moved?");
                }
            }else
            {
                textures.Add(null);
            }
        }
        texts = data.texts.ToList();
        links = data.links.ToList();
    }
}

[System.Serializable]
public class PageData
{
    public string[] tags;
    public string name;
    public string pageType;
    public int id;
    public string[] imagePaths;
    public string[] texts;
    public int[] links;

    public PageData(Page data)
    {
        tags = data.tags.ToArray();
        pageType = data.pageType.ToString();
        name = data.name;
        id = data.id;
        imagePaths = data.imagePaths.ToArray();
        texts = data.texts.ToArray();
        links = data.links.ToArray();
    }
}
