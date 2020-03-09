using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainPanelBehaviour : MonoBehaviour
{
    public int currentPageId;
    public TMP_InputField inputField;
    public ImageLoader mainImage;
    public PagetagSelection pagetagSelection;
    public GameObject header;


    [Space(20)]

    public BlankPageBehaviour blankPage;
    public CharacterPageBehaviour characterpage;

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
            inputField.text = GameManager.CurrentCampaign.GetPageById(currentPageId).name;
            if(GameManager.CurrentCampaign.GetPageById(currentPageId).textures.Count > 0)
            {
                mainImage.image.sprite = GameManager.CurrentCampaign.GetPageById(currentPageId).textures[0];
            }
            else if (GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths.Count > 0)
            {
                mainImage.path = GameManager.CurrentCampaign.GetPageById(currentPageId).imagePaths[0];
                mainImage.LoadImage();
            }
            else
            {
                mainImage.ClearImage();
            }
            pagetagSelection.SetPageTags();
            GameManager.Instance.linksPanel.SetLinksToCurrentPage();
        }
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

                    blankPage.SetText(GameManager.CurrentCampaign.GetPageById(id).texts[0]);
                    break;
                case PageTypes.Character:
                    blankPage.gameObject.SetActive(false);
                    characterpage.gameObject.SetActive(true);

                    characterpage.SetText(GameManager.CurrentCampaign.GetPageById(id).texts);
                    break;
            }
            if(GameManager.CurrentCampaign.GetPageById(id).imagePaths.Count > 0)
                mainImage.path = GameManager.CurrentCampaign.GetPageById(id).imagePaths[0];
        }
    }
}
