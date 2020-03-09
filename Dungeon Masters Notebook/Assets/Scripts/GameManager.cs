using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public Campaign currentCampaign;

    public static void CreateNewCampaign(string name)
    {
        instance.currentCampaign = new Campaign();
        instance.currentCampaign.name = name;
        instance.AddPage(0);
        SaveSystem.SaveCampaign(CurrentCampaign);
    }

    public UnityEvent onTagsChanged;
    private static GameManager instance;

    public static bool isSelectingLink = false;

    public PagesPanel pagesPanel;

    public CampaignTagPanel tagsPanel;

    public MainPanelBehaviour mainPanel;

    public LinksPanel linksPanel;

    public CampaignSelection campaignSelection;

    Dictionary<int, PageTypes> typesByInt = new Dictionary<int, PageTypes>
    {
        {0, PageTypes.Blank },
        {1, PageTypes.Character }
    };

    public static GameManager Instance { get => instance; }
    public static Campaign CurrentCampaign { get => instance.currentCampaign; }

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
        mainPanel.pagetagSelection.Initialize();
        //currentCampaign = new Campaign();
    }

    public void AddPage(int type)
    {
        if(type >= 0 && type < typesByInt.Count)
        {
            currentCampaign.pages.Add(new Page(currentCampaign.FirstAvailableId(), CurrentCampaign.tags.Where(t => t.active).ToList(), typesByInt[type]));
            pagesPanel.CreatePageItem(currentCampaign.pages[currentCampaign.pages.Count - 1]);
            OpenPage(currentCampaign.pages[currentCampaign.pages.Count - 1].id);
        }

    }

    public static void OpenCampaign(string title)
    {
        instance.Load(title);
    }

    public void SelectPage(int id)
    {
        linksPanel.AddLink(id);
    }

    public void Load(string title)
    {
        currentCampaign = new Campaign(SaveSystem.LoadCampaign(title));
        pagesPanel.LoadPages();
        tagsPanel.LoadTags();
        mainPanel.OpenPage(0);
        tagsPanel.pagetags.UpdateTags();
    }

    public void OpenPage(int id)
    {
        mainPanel.OpenPage(id);
    }

    public void Save()
    {
        SaveSystem.SaveCampaign(currentCampaign);
    }

}
