using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelBehaviour : MonoBehaviour
{
    public int currentPageId;
    public List<TMP_InputField> nameInputFields;
    public ImageLoader imageLoader;
    public PagetagSelection[] pagetagSelections;
    public Toggle[] favoriteToggles;
    public GameObject header;


    [Space(20)]

    public List<TypePage> pageBehaviours;

    private void Awake()
    {
        ActivateAllPages(false);
    }

    public void UpdateItems()
    {
        if (GameManager.CurrentCampaign.GetPageById(currentPageId) != null)
        {
            for (int i = 0; i < favoriteToggles.Length; i++)
            {
                favoriteToggles[i].isOn = GameManager.CurrentCampaign.GetPageById(currentPageId).favorite;
            }
            for (int i = 0; i < nameInputFields.Count; i++)
            {
                nameInputFields[i].text = GameManager.CurrentCampaign.GetPageById(currentPageId).name;
            }
            if (GameManager.CurrentCampaign.GetPageById(currentPageId).textures.Count > 0)
            {
                for (int i = 0; i < imageLoader.images.Length; i++)
                {
                    imageLoader.images[i].sprite = GameManager.CurrentCampaign.GetPageById(currentPageId).textures[0];
                }
            }
            else if (GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths.Count > 0)
            {
                imageLoader.path = GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths[0];
                imageLoader.LoadImageChecked();
            }
            else
            {
                imageLoader.ClearImage();
            }
            for (int i = 0; i < pagetagSelections.Length; i++)
            {
                pagetagSelections[i].SetPageTags();
            }
            GameManager.Instance.linksPanel.SetLinksToCurrentPage();
        }
    }

    public void OnToggleFavorite(bool val)
    {
        GameManager.CurrentCampaign.GetPageById(currentPageId).favorite = val;
        GameManager.CurrentCampaign.GetPageById(currentPageId).onPageChanged.Invoke();
        GameManager.Instance.pagesPanel.FilterPages();
    }

    public void ClosePage()
    {
        ActivateAllPages(false);
    }

    public void OnNameEdit(string newName)
    {
        if (GameManager.CurrentCampaign.GetPageById(currentPageId) != null)
        {
            GameManager.CurrentCampaign.GetPageById(currentPageId).name = !string.IsNullOrEmpty(newName) ? newName : "New Page" ;
            PageChanged();
        }
    }

    public void OnMainImageEdit()
    {
        if (GameManager.CurrentCampaign.GetPageById(currentPageId) != null)
        {
            if (GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths.Count > 0)
            {
                GameManager.CurrentCampaign.GetPageById(currentPageId).textures[0] = imageLoader.currentlyLoadedSprite;
                GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths[0] = imageLoader.path;
            }
            else
            {
                GameManager.CurrentCampaign.GetPageById(currentPageId).textures.Add(imageLoader.currentlyLoadedSprite);
                GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths.Add(imageLoader.path);
            }

            PageChanged();
        }
    }

    public void PageChanged()
    {
        if (GameManager.CurrentCampaign.GetPageById(currentPageId) != null)
        {
            GameManager.CurrentCampaign.GetPageById(currentPageId).onPageChanged.Invoke();
        }
    }

    public void OpenPage(Page page)
    {
        header.gameObject.SetActive(true);
        currentPageId = page.id;
        GameManager.Instance.linksPanel.gameObject.SetActive(true);
        UpdateItems();
        ActivateAllPages(false, page.pageType);
        GetPageByType(page.pageType).SetText(page.texts);
        if (page.imagePaths.Count > 0)
        {
            imageLoader.path = page.imagePaths[0];
        } 
    }

    public void OpenPage(int id)
    {
        if(GameManager.CurrentCampaign.GetPageById(id) != null)
        {
            header.gameObject.SetActive(true);
            currentPageId = id;
            GameManager.Instance.linksPanel.gameObject.SetActive(true);
            UpdateItems();
            ActivateAllPages(false, GameManager.CurrentCampaign.GetPageById(id).pageType);
            GetPageByType(GameManager.CurrentCampaign.GetPageById(id).pageType).SetText(GameManager.CurrentCampaign.GetPageById(id).texts);
            if (GameManager.CurrentCampaign.GetPageById(id).imagePaths.Count > 0)
            {
                imageLoader.path = GameManager.CurrentCampaign.GetPageById(id).imagePaths[0];
            }
        }
    }
    
    public void ActivateAllPages(bool val, PageTypes exception = PageTypes.Default)
    {
        if(exception == PageTypes.Default || exception == PageTypes.Map)
        {
            header.gameObject.SetActive(val);
            GameManager.Instance.linksPanel.gameObject.SetActive(val);
        }
        for (int i = 0; i < pageBehaviours.Count; i++)
        {
            if (pageBehaviours[i].page != null)
            {
                pageBehaviours[i].page.gameObject.SetActive(pageBehaviours[i].pageType != exception ? val : !val);
            }
        }
    }

    public PageBehaviour GetPageByType(PageTypes pageType)
    {
        for (int i = 0; i < pageBehaviours.Count; i++)
        {
            if (pageBehaviours[i].pageType == pageType)
                return pageBehaviours[i].page;
        }
        return null;
    }

    [System.Serializable]
    public class TypePage
    {
        public PageTypes pageType;
        public PageBehaviour page;
    }
}
