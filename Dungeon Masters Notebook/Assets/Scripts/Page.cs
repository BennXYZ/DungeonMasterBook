using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Page
{
    public string name;
    public int id;
    public List<string> imagePaths;
    public List<string> tags;
    public List<string> texts;
    public List<int> links;
    public List<Note> notes;
    public PageTypes pageType;
    public bool favorite;

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
        for (int i = 0; i < GameManager.Instance.settingsList.GetSettings(pageType).numberOfTexts; i++)
        {
            texts.Add(string.Empty);
        }
        for (int i = 0; i < GameManager.Instance.settingsList.GetSettings(pageType).numberOfImages; i++)
        {
            imagePaths.Add(string.Empty);
            textures.Add(GameManager.Instance.defaultSprite);
        }
    }

    public Page(PageData data)
    {
        onPageChanged = new UnityEvent();
        id = data.id;
        name = data.name;
        favorite = data.favorite;
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
                    if(imagePaths[i].StartsWith("file:///"))
                    {
                        WWW www = new WWW(imagePaths[i]);
                        textures.Add(Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f));
                    }
                    else
                    {
                        textures.Add(GameManager.Instance.defaultSprite);
                        int currentImage = i;
                        GameManager.Instance.StartLoadingImageURL(this, currentImage);
                    }
                }
                catch
                {
                    Debug.LogWarning("Picture not Found. Maybe it was moved?");
                }
            }else
            {
                textures.Add(GameManager.Instance.defaultSprite);
            }
        }
        texts = data.texts.ToList();
        links = data.links.ToList();
    }
}

public class Note
{
    public string title;
    public string text;
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
    public bool favorite;

    public PageData(Page data)
    {
        tags = data.tags.ToArray();
        pageType = data.pageType.ToString();
        name = data.name;
        favorite = data.favorite;
        id = data.id;
        imagePaths = data.imagePaths.ToArray();
        texts = data.texts.ToArray();
        links = data.links.ToArray();
    }
}
