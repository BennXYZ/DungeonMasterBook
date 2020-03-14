using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public Campaign currentCampaign;

    public PageSettingsList settingsList;

    public UnityEvent onTagsChanged;
    private static GameManager instance;
    public bool favoriteFilter;

    public static bool isSelectingLink = false;
    public static bool isSelectingMapitem = false;
    public bool inMenu {
        get;
        set; }
    public bool prevMenu;

    public TMP_Text campaignTitle;

    public PagesPanel pagesPanel;

    public CampaignTagPanel tagsPanel;

    public MainPanelBehaviour mainPanel;

    public LinksPanel linksPanel;

    public CampaignSelection campaignSelection;

    public Dictionary<PageTypes, bool> pageFilters = new Dictionary<PageTypes, bool>
    {
        {PageTypes.Blank, false },
        {PageTypes.Item, false },
        {PageTypes.Character, false },
        {PageTypes.Location, false },
        {PageTypes.Quest, false },
        {PageTypes.Group, false },
        {PageTypes.Map, false}
    };

    public static GameManager Instance { get => instance; }
    public static Campaign CurrentCampaign { get => instance.currentCampaign; }

    private void LateUpdate()
    {
        prevMenu = inMenu;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        pagesPanel.Clear();
        campaignSelection.gameObject.SetActive(true);
        for (int i = 0; i < mainPanel.pagetagSelections.Length; i++)
        {
            mainPanel.pagetagSelections[i].Initialize();
        }
    }

    public void SetFavoriteFilter(bool val)
    {
        favoriteFilter = val;
        pagesPanel.FilterPages();
    }

    public void SetBlankFilter(bool val)
    {
        SetPageFilter(PageTypes.Blank, val);
    }
    public void SetCharacterFilter(bool val)
    {
        SetPageFilter(PageTypes.Character, val);
    }
    public void SetItemFilter(bool val)
    {
        SetPageFilter(PageTypes.Item, val);
    }
    public void SetLocationFilter(bool val)
    {
        SetPageFilter(PageTypes.Location, val);
    }
    public void SetQuestFilter(bool val)
    {
        SetPageFilter(PageTypes.Quest, val);
    }
    public void SetGroupFilter(bool val)
    {
        SetPageFilter(PageTypes.Group, val);
    }
    public void SetMapFilter(bool val)
    {

    }

    public void SetPageFilter(PageTypes pageType, bool val)
    {
        pageFilters[pageType] = val;
        pagesPanel.FilterPages();
    }

    public void AddPage(string name)
    {
        try
        {
            PageTypes type = (PageTypes)Enum.Parse(typeof(PageTypes), name);
            if(type != PageTypes.Default)
            {
                currentCampaign.pages.Add(new Page(currentCampaign.FirstAvailableId(), CurrentCampaign.tags.Where(t => t.active).ToList(), type));
                pagesPanel.CreatePageItem(currentCampaign.pages[currentCampaign.pages.Count - 1]);
                OpenPage(currentCampaign.pages[currentCampaign.pages.Count - 1].id);
            }
            else
            {
                Debug.LogError("There was an Error when choosing a Page-Type. Name: " + name);
            }
        }
        catch
        {
            Debug.LogError("There was an Error when choosing a Page-Type. Name: " + name);
        }
    }

    public static void OpenCampaign(string title)
    {
        instance.Load(title);
    }

    public void SelectPage(int id)
    {
        linksPanel.SelectId(id);
    }

    public void Load(string title)
    {
        currentCampaign = new Campaign(SaveSystem.LoadCampaign(title));
        campaignTitle.text = title;
        pagesPanel.LoadPages();
        tagsPanel.LoadTags();
        mainPanel.OpenPage(currentCampaign.pages[0]);
        tagsPanel.UpdatePageTags();
    }

    public void StartLoadingImageURL(Page page, int textureId)
    {
        StartCoroutine(LoadTexture(page, textureId));
    }

    public IEnumerator LoadTexture(Page page, int textureId)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(page.imagePaths[textureId]);
        UnityWebRequestAsyncOperation imageRequest = www.SendWebRequest();

        int timeOutCounter = 0;

        while (!imageRequest.isDone && timeOutCounter < 10)
        {
            timeOutCounter++;
            yield return new WaitForSeconds(2);
        }

        if (!imageRequest.isDone || www.isNetworkError || www.isHttpError)
        {
            page.textures[textureId] = null;
        }
        else
        {
            //return valid results:
            Texture2D result = DownloadHandlerTexture.GetContent(www);
            page.textures[textureId] = Sprite.Create(result, new Rect(0, 0, result.width, result.height), Vector2.one * 0.5f);
        }
        page.onPageChanged.Invoke();
        mainPanel.UpdateItems();
    }

    public void _CreateNewCampaign(string name)
    {
        currentCampaign = new Campaign();
        currentCampaign.name = name;
        pagesPanel.LoadPages();
        tagsPanel.LoadTags();
        AddPage("Blank");
        SaveSystem.SaveCampaign(CurrentCampaign);
        campaignTitle.text = name;
        tagsPanel.UpdatePageTags();
    }

    public static void CreateNewCampaign(string name)
    {
        instance._CreateNewCampaign(name);
    }

    public void OpenPage(int id)
    {
        mainPanel.OpenPage(id);
    }

    public void Save()
    {
        SaveSystem.SaveCampaign(currentCampaign);
    }

    [System.Serializable]
    public class PageSettingsList
    {
        public List<PageSettings> settings;
        public PageSettings GetSettings(PageTypes type)
        {
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].type == type)
                    return settings[i];
            }
            Debug.LogError("PageSettings for this Type not found");
            return null;
        }
    }
}
