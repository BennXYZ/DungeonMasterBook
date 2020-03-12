using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelBehaviour : MonoBehaviour
{
    public int currentPageId;
    public TMP_InputField inputField;
    public ImageLoader mainImage;
    public PagetagSelection pagetagSelection;
    public Toggle favoriteToggle;
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
            favoriteToggle.isOn = GameManager.CurrentCampaign.GetPageById(currentPageId).favorite;
            inputField.text = GameManager.CurrentCampaign.GetPageById(currentPageId).name;
            if(GameManager.CurrentCampaign.GetPageById(currentPageId).textures.Count > 0)
            {
                mainImage.image.sprite = GameManager.CurrentCampaign.GetPageById(currentPageId).textures[0];
            }
            else if (GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths.Count > 0)
            {
                mainImage.path = GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths[0];
                mainImage.LoadImageChecked();
            }
            else
            {
                mainImage.ClearImage();
            }
            pagetagSelection.SetPageTags();
            GameManager.Instance.linksPanel.SetLinksToCurrentPage();
        }
    }

    public void OnToggleFavorite(bool val)
    {
        GameManager.CurrentCampaign.GetPageById(currentPageId).favorite = val;
        GameManager.CurrentCampaign.GetPageById(currentPageId).onPageChanged.Invoke();
        GameManager.Instance.pagesPanel.FilterPages();
    }

    public void OnNameEdit()
    {
        if (GameManager.CurrentCampaign.GetPageById(currentPageId) != null)
        {
            GameManager.CurrentCampaign.GetPageById(currentPageId).name = !string.IsNullOrEmpty(inputField.text) ? inputField.text : "New Page" ;
            PageChanged();
        }
    }

    public void OnMainImageEdit()
    {
        if (GameManager.CurrentCampaign.GetPageById(currentPageId) != null)
        {
            if(GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths.Count > 0)
            {
                GameManager.CurrentCampaign.GetPageById(currentPageId).textures[0] = mainImage.image.sprite;
                GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths[0] = mainImage.path;
            }
            else
            {
                GameManager.CurrentCampaign.GetPageById(currentPageId).textures.Add(mainImage.image.sprite);
                GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths.Add(mainImage.path);
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
        currentPageId = page.id;
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
                mainImage.path = GameManager.CurrentCampaign.GetPageById(id).imagePaths[0];
        }
    }
    
    public void ActivateAllPages(bool val, PageTypes exception = PageTypes.Default)
    {
        if(exception == PageTypes.Default)
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
