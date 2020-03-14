using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PagePanelItem : MonoBehaviour
{
    public Page page;
    public Image image;
    public TMP_Text name;
    public TMP_Text tags;
    public Image favorite;
    public Button button;

    public bool isLink = false;

    public UnityEvent onOpenPage;
    public UnityEvent onRemove;

    public PagePanelItem()
    {
        onOpenPage = new UnityEvent();
           onRemove = new UnityEvent();
    }

    public void UpdateData()
    {
        if(page.textures.Count > 0)
        {
            image.sprite = page.textures[0];
        }
        else if (page.imagePaths.Count > 0 && !string.IsNullOrEmpty(page.imagePaths[0]))
        {
            WWW www = new WWW("file:///" + page.imagePaths[0]);
            image.sprite = image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f);
        }
        if(favorite)
        {
            favorite.gameObject.SetActive(page.favorite);
        }
        name.text = page.name;
        UpdateTagText();
    }

    public void UpdateTagText()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < page.tags.Count; i++)
        {
            if (GameManager.CurrentCampaign.TagExists(page.tags[i]))
            {
                if (builder.Length > 0)
                    builder.Append(", ");
                builder.Append(page.tags[i]);
            }
        }
        tags.text = builder.ToString();
    }

    public void PagePanelClicked()
    {
        if(GameManager.isSelectingLink && !isLink)
        {
            GameManager.Instance.SelectPage(page.id);
        }
        else if(GameManager.isSelectingMapitem && !isLink)
        {
            if(GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).pageType == PageTypes.Map)
            {
                GameManager.Instance.mainPanel.AddItemToMap(page.id);
                GameManager.isSelectingMapitem = false;
            }
            else
            {
                GameManager.isSelectingMapitem = false;
            }
        }
        else
        {
            onOpenPage.Invoke();
        }
    }

    public void Init(Page page)
    {
        this.page = page;
        page.onPageChanged.AddListener(UpdateData);
        UpdateData();
    }

    public void OnRemove()
    {
        WarningNotification.OpenNotificationWindow("Do you really want to delete the page " + page.name + "?", delegate { onRemove.Invoke(); });   
    }
}
