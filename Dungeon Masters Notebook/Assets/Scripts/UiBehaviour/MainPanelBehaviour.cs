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

    public BlankPageBehaviour blankPage;
    public CharacterPageBehaviour characterpage;
    public ItemPageBehaviour itemPage;
    public LocationPageBehaviour locationPage;
    public QuestPageBehaviour questPage;
    public GroupPageBehaviour groupPage;

    private void Awake()
    {
        CloseCurrentPage();
    }

    public void CloseCurrentPage()
    {
        header.gameObject.SetActive(false);
        blankPage.gameObject.SetActive(false);
        characterpage.gameObject.SetActive(false);
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
            UpdateItems();
            switch(GameManager.CurrentCampaign.GetPageById(id).pageType)
            {
                case PageTypes.Blank:
                    blankPage.gameObject.SetActive(true);
                    characterpage.gameObject.SetActive(false);
                    itemPage.gameObject.SetActive(false);
                    locationPage.gameObject.SetActive(false);
                    questPage.gameObject.SetActive(false);
                    groupPage.gameObject.SetActive(false);

                    blankPage.SetText(GameManager.CurrentCampaign.GetPageById(id).texts[0]);
                    break;
                case PageTypes.Character:
                    blankPage.gameObject.SetActive(false);
                    characterpage.gameObject.SetActive(true);
                    itemPage.gameObject.SetActive(false);
                    locationPage.gameObject.SetActive(false);
                    questPage.gameObject.SetActive(false);
                    groupPage.gameObject.SetActive(false);

                    characterpage.SetText(GameManager.CurrentCampaign.GetPageById(id).texts);
                    break;
                case PageTypes.Item:
                    blankPage.gameObject.SetActive(false);
                    characterpage.gameObject.SetActive(false);
                    itemPage.gameObject.SetActive(true);
                    locationPage.gameObject.SetActive(false);
                    questPage.gameObject.SetActive(false);
                    groupPage.gameObject.SetActive(false);

                    itemPage.SetText(GameManager.CurrentCampaign.GetPageById(id).texts);
                    break;
                case PageTypes.Location:
                    blankPage.gameObject.SetActive(false);
                    characterpage.gameObject.SetActive(false);
                    itemPage.gameObject.SetActive(false);
                    locationPage.gameObject.SetActive(true);
                    questPage.gameObject.SetActive(false);
                    groupPage.gameObject.SetActive(false);

                    locationPage.SetText(GameManager.CurrentCampaign.GetPageById(id).texts);
                    break;
                case PageTypes.Quest:
                    blankPage.gameObject.SetActive(false);
                    characterpage.gameObject.SetActive(false);
                    itemPage.gameObject.SetActive(false);
                    locationPage.gameObject.SetActive(false);
                    questPage.gameObject.SetActive(true);
                    groupPage.gameObject.SetActive(false);

                    questPage.SetText(GameManager.CurrentCampaign.GetPageById(id).texts);
                    break;
                case PageTypes.Group:
                    blankPage.gameObject.SetActive(false);
                    characterpage.gameObject.SetActive(false);
                    itemPage.gameObject.SetActive(false);
                    locationPage.gameObject.SetActive(false);
                    questPage.gameObject.SetActive(false);
                    groupPage.gameObject.SetActive(true);

                    groupPage.SetText(GameManager.CurrentCampaign.GetPageById(id).texts);
                    break;
            }
            if(GameManager.CurrentCampaign.GetPageById(id).imagePaths.Count > 0)
                mainImage.path = GameManager.CurrentCampaign.GetPageById(id).imagePaths[0];
        }
    }
}
