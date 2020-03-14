using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapPageBehaviour : PageBehaviour
{
    public List<MapItem> items;
    List<MapItemLink> linkItems;

    public TMP_Text panelTitle;

    public GameObject mapItemPrefab;
    public RectTransform mapItemsParent;

    public GameObject mapLinkPrefab;

    public float maxRangeTilDrag;

    public UnityEvent onItempressedEvent;
    public RectTransform mapItemContextMenu;
    public Slider sizeSlider;

    int currentLinkId = -1;
    bool isCreatingLink = true;

    int currentHoverItem = -1;

    bool isDrag = false;
    Vector2 previourMousePosition;
    Vector2 startMousePosition;

    Vector2 DeltaMousePosition
    {
        get
        {
            return (Vector2)Input.mousePosition - previourMousePosition;
        }
    }

    Vector2 DeltaWorldMousePosition
    {
        get
        {
            return (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)Camera.main.ScreenToWorldPoint(previourMousePosition);
        }
    }

    public override void Awake()
    {
        base.Awake();
        linkItems = new List<MapItemLink>();
        previourMousePosition = Input.mousePosition;
        startMousePosition = Input.mousePosition;
    }

    public override void TextChanged(string newText, int textId)
    {
        base.TextChanged(newText, textId);
    }

    public override void SetText(List<string> texts)
    {
        Clear();

        SetScale(texts[0]);
        for (int i = 1; i < texts.Count; i++)
        {
            SpawnItem(texts[i]);
        }
    }

    public void SetScale(string text)
    {
        float result;
        if(float.TryParse(text,out result))
        {
            sizeSlider.value = result;
            return;
        }
        sizeSlider.value = 0.5f;
    }

    public void AddPage(int id)
    {
        SpawnItem(id);
    }

    private void OnDisable()
    {
        currentLinkId = -1;
        Clear();
    }

    public void RemovePage(int id)
    {
        for (int i = 1; i < GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.Count; i++)
        {
            if(MapItemDataConverter.GetId(GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[i]) == id)
            {
                GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.RemoveAt(i);
                break;
            }
        }
        for (int i = 0; i < items.Count; i++)
        {
            if(items[i].page.id == id)
            {
                if(items[i].page.pageType == PageTypes.Map)
                {
                    for (int j = 1; j < items[i].page.texts.Count; j++)
                    {
                        if(MapItemDataConverter.GetId(items[i].page.texts[j]) == GameManager.Instance.mainPanel.currentPageId)
                        {
                            items[i].page.texts.RemoveAt(j);
                        }
                    }
                }
                else
                {
                    items[i].page.links.Remove(GameManager.Instance.mainPanel.currentPageId);
                }
                Destroy(items[i].gameObject);
                items.RemoveAt(i);
            }
        }
    }

    public MapItem SpawnItem(int id, Vector2 originPosition, float rotationalOffset)
    {
        if (GetItemById(id) != null || id == GameManager.Instance.mainPanel.currentPageId)
        {
            return null;
        }

        MapItem newItem = SpawnItem(id);
        float offsetX = Mathf.Sin(rotationalOffset * Mathf.PI * 2); float offsetY = Mathf.Cos(rotationalOffset * Mathf.PI * 2);
        newItem.rectTransform.anchorMin = originPosition;
        newItem.rectTransform.anchorMin = Utils.NewVector2((((newItem.rectTransform.parent as RectTransform).rect.width * newItem.rectTransform.anchorMin.x) - offsetX * 100) 
            / (newItem.rectTransform.parent as RectTransform).rect.width, (((newItem.rectTransform.parent as RectTransform).rect.height * 
            newItem.rectTransform.anchorMin.y) - offsetY * 100) / (newItem.rectTransform.parent as RectTransform).rect.height);
        newItem.rectTransform.anchorMax = newItem.rectTransform.anchorMin;

        newItem.rectTransform.offsetMax = Vector2.zero;
        newItem.rectTransform.offsetMin = Vector2.zero;

        //newItem.rectTransform.offsetMax = new Vector3(offsetX * 100, offsetY * 100, newItem.rectTransform.localPosition.z);
        //newItem.rectTransform.offsetMin = new Vector3(offsetX * 100, offsetY * 100, newItem.rectTransform.localPosition.z);
        newItem.rectTransform.sizeDelta = Vector2.one * (10 + 150 * sizeSlider.value);
        //newItem.transform.position = Utils.NewVector3(originPosition.x + Mathf.Sin(rotationalOffset * Mathf.PI * 2), originPosition.y + Mathf.Cos(rotationalOffset * Mathf.PI * 2),
        //    originPosition.z);
        //UpdateAnchorsFromLocalposition(items.IndexOf(newItem));

        GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts
            [GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.Count - 1] = (MapItemDataConverter.GetTextFromItem(id, newItem.rectTransform.anchorMin));

        return newItem;
    }

    public MapItem SpawnItem(int id)
    {
        if(GetItemById(id) != null || id == GameManager.Instance.mainPanel.currentPageId)
        {
            return null;
        }
        items.Add(Instantiate(mapItemPrefab, mapItemsParent).GetComponent<MapItem>());
        int newItemId = items.Count - 1;
        items[newItemId].SetPage(GameManager.Instance.currentCampaign.GetPageById(id));
        if(items[newItemId].page.pageType != PageTypes.Map && !items[newItemId].page.links.Contains(GameManager.Instance.mainPanel.currentPageId))
        {
            items[newItemId].page.links.Add(GameManager.Instance.mainPanel.currentPageId);
        }
        else if(items[newItemId].page.pageType == PageTypes.Map)
        {
            items[newItemId].page.texts.Add(MapItemDataConverter.GetTextFromItem(GameManager.Instance.mainPanel.currentPageId, Vector2.one * 0.5f));
        }
        items[newItemId].onEndMove = new UnityEngine.Events.UnityEvent();
        items[newItemId].onEndMove.AddListener(delegate { TextChanged(MapItemDataConverter.GetTextFromItem(items[currentHoverItem].page.id, items[currentHoverItem].rectTransform.anchorMin), currentHoverItem + 1); });

        items[newItemId].rectTransform.anchorMin = Vector2.one * 0.5f;
        items[newItemId].rectTransform.anchorMax = Vector2.one * 0.5f;
        items[newItemId].rectTransform.offsetMin = Vector2.zero;
        items[newItemId].rectTransform.offsetMax = Vector2.zero;
        items[newItemId].rectTransform.sizeDelta = GetCurrentObjectSize();
        items[newItemId].rectTransform.position = Vector3.right * items[newItemId].rectTransform.position.x + Vector3.up * items[newItemId].rectTransform.position.y + Vector3.forward * 50;

        GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.Add(MapItemDataConverter.GetTextFromItem(id, Vector2.one * 0.5f));

        for (int i = 0; i < items.Count - 1; i++)
        {
            if(items[newItemId].page.links.Contains(items[i].page.id) || items[i].page.links.Contains(items[newItemId].page.id))
            {
                int foundItem = i;
                linkItems.Add(Instantiate(mapLinkPrefab).GetComponent<MapItemLink>().SetTransforms(items[newItemId].gameObject.transform, items[foundItem].gameObject.transform, sizeSlider));
            }
        }

        return items[newItemId];
    }

    public void InitiateLinking()
    {
        currentLinkId = items[currentHoverItem].page.id;
        isCreatingLink = true;
    }

    public void InitiateUnlinking()
    {
        currentLinkId = items[currentHoverItem].page.id;
        isCreatingLink = false;
    }

    public MapItem SpawnItem(string text)
    {
        if(GameManager.CurrentCampaign.GetPageById(MapItemDataConverter.GetId(text)) == null)
        {
            return null;
        }

        items.Add(Instantiate(mapItemPrefab, mapItemsParent).GetComponent<MapItem>());
        int newItemId = items.Count - 1;
        items[newItemId].SetPage(GameManager.Instance.currentCampaign.GetPageById(MapItemDataConverter.GetId(text)));
        items[newItemId].onEndMove = new UnityEngine.Events.UnityEvent();
        items[newItemId].onEndMove.AddListener(delegate { TextChanged(MapItemDataConverter.GetTextFromItem(items[currentHoverItem].page.id, items[currentHoverItem].rectTransform.anchorMin), currentHoverItem + 1); });

        items[newItemId].rectTransform.anchorMin = MapItemDataConverter.GetPosition(text);
        items[newItemId].rectTransform.anchorMax = MapItemDataConverter.GetPosition(text);
        items[newItemId].rectTransform.offsetMin = Vector2.zero;
        items[newItemId].rectTransform.offsetMax = Vector2.zero;
        items[newItemId].rectTransform.sizeDelta = GetCurrentObjectSize();
        items[newItemId].rectTransform.position = Vector3.right * items[newItemId].rectTransform.position.x + Vector3.up * items[newItemId].rectTransform.position.y + Vector3.forward * 50;

        for (int i = 0; i < items.Count - 1; i++)
        {
            if (items[newItemId].page.links.Contains(items[i].page.id))
            {
                int foundItem = i;
                linkItems.Add(Instantiate(mapLinkPrefab).GetComponent<MapItemLink>().SetTransforms(items[newItemId].gameObject.transform, items[foundItem].gameObject.transform, sizeSlider));
            }
        }

        return items[newItemId];
    }

    public void SetObjectSize(float scale)
    {
        GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts[0] = scale.ToString();
        for (int i = 0; i < items.Count; i++)
        {
            items[i].rectTransform.sizeDelta = Vector2.one * (10 + 150 * scale);
        }
    }

    public Vector2 GetCurrentObjectSize()
    {
        return Vector2.one * (10 + 150 * sizeSlider.value);
    }

    public void Clear()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }
        items.Clear();
    }

    private void Update()
    {
        if(!GameManager.Instance.prevMenu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentHoverItem = GetIndexOfFirstMapItem(Input.mousePosition);
                if (currentHoverItem >= 0)
                {
                    previourMousePosition = Input.mousePosition;
                    startMousePosition = Input.mousePosition;
                }
                isDrag = false;
            }
            else if (Input.GetMouseButton(0) && currentHoverItem >= 0 && DeltaWorldMousePosition != Vector2.zero)
            {
                items[currentHoverItem].rectTransform.position = items[currentHoverItem].rectTransform.position + (Vector3)DeltaWorldMousePosition;
                previourMousePosition = Input.mousePosition;
                if (((Vector2)Input.mousePosition - startMousePosition).sqrMagnitude > maxRangeTilDrag && !isDrag)
                {
                    isDrag = true;
                }
            }
            else if (Input.GetMouseButtonUp(0) && currentHoverItem >= 0)
            {
                UpdateCurrentHoverAnchors();
                if (!isDrag)
                {
                    if(currentLinkId >= 0)
                    {
                        if(isCreatingLink)
                        {
                            CreateLink(currentLinkId, items[currentHoverItem].page.id);
                        }
                        else
                        {
                            RemoveLink(currentLinkId, items[currentHoverItem].page.id);
                        }
                        currentLinkId = -1;
                    }
                    else
                    {
                        GameManager.Instance.inMenu = true;
                        mapItemContextMenu.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * mapItemContextMenu.position.z;
                        panelTitle.text = items[currentHoverItem].page.name;
                        onItempressedEvent.Invoke();
                    }
                }
            }
        }
    }

    public void UpdateAnchorsFromLocalposition(int itemId)
    {
        if (GetItemById(itemId) == null)
            return;
        Vector2 positionOnParent = Utils.NewVector2(((items[itemId].rectTransform.parent as RectTransform).rect.width * items[itemId].rectTransform.anchorMin.x - 
            items[itemId].rectTransform.offsetMin.x) / (items[itemId].rectTransform.parent as RectTransform).rect.width,
            ((items[itemId].rectTransform.parent as RectTransform).rect.height * items[itemId].rectTransform.anchorMin.y -
            items[itemId].rectTransform.offsetMin.y) / (items[itemId].rectTransform.parent as RectTransform).rect.height);
        positionOnParent += ((Vector2)Input.mousePosition - startMousePosition);

        Vector2 size = items[itemId].rectTransform.sizeDelta;
        items[itemId].rectTransform.offsetMin = Vector2.zero;
        items[itemId].rectTransform.offsetMax = Vector2.zero;
        items[itemId].rectTransform.anchorMin = positionOnParent;
        items[itemId].rectTransform.anchorMin = Vector2.right * Mathf.Max(Mathf.Min(items[itemId].rectTransform.anchorMin.x, 1), 0)
            + Vector2.up * Mathf.Max(Mathf.Min(items[itemId].rectTransform.anchorMin.y, 1), 0);
        items[itemId].rectTransform.anchorMax = items[itemId].rectTransform.anchorMin;
        items[itemId].rectTransform.sizeDelta = size;

        items[itemId].onEndMove.Invoke();
    }

    public void UpdateCurrentHoverAnchors()
    {
        Vector2 positionOnParent = Vector2.right * items[currentHoverItem].rectTransform.anchorMin.x * mapItemsParent.rect.width +
    Vector2.up * items[currentHoverItem].rectTransform.anchorMin.y * mapItemsParent.rect.height;
        positionOnParent += ((Vector2)Input.mousePosition - startMousePosition);

        Vector2 deltaMovement = Vector2.right * (Input.mousePosition.x - startMousePosition.x) / mapItemsParent.rect.width +
            Vector2.up * (Input.mousePosition.y - startMousePosition.y) / mapItemsParent.rect.height;

        Vector2 size = items[currentHoverItem].rectTransform.sizeDelta;
        items[currentHoverItem].rectTransform.offsetMin = Vector2.zero;
        items[currentHoverItem].rectTransform.offsetMax = Vector2.zero;
        items[currentHoverItem].rectTransform.anchorMin = Vector2.right * positionOnParent.x / mapItemsParent.rect.width + Vector2.up * positionOnParent.y / mapItemsParent.rect.height;
        items[currentHoverItem].rectTransform.anchorMin = Vector2.right * Mathf.Max(Mathf.Min(items[currentHoverItem].rectTransform.anchorMin.x, 1), 0)
            + Vector2.up * Mathf.Max(Mathf.Min(items[currentHoverItem].rectTransform.anchorMin.y, 1), 0);
        items[currentHoverItem].rectTransform.anchorMax = items[currentHoverItem].rectTransform.anchorMin;
        items[currentHoverItem].rectTransform.sizeDelta = size;

        items[currentHoverItem].onEndMove.Invoke();
    }

    private void CreateLink(int aId, int bId)
    {
        if (GameManager.CurrentCampaign.GetPageById(aId).links.Contains(bId) || GameManager.CurrentCampaign.GetPageById(bId).links.Contains(aId) || aId == bId)
            return;
        else
        {
            GameManager.CurrentCampaign.GetPageById(aId).links.Add(bId);
            GameManager.CurrentCampaign.GetPageById(bId).links.Add(aId);
            linkItems.Add(Instantiate(mapLinkPrefab).GetComponent<MapItemLink>().SetTransforms(GetItemById(aId).gameObject.transform,GetItemById(bId).gameObject.transform, sizeSlider));
        }
    }

    private void RemoveLink(int aId, int bId)
    {
        GameManager.CurrentCampaign.GetPageById(aId).links.Remove(bId);
        GameManager.CurrentCampaign.GetPageById(bId).links.Remove(aId);
        for (int i = 0; i < linkItems.Count; i++)
        {
            if(linkItems[i].firstObject == GetItemById(aId).transform && linkItems[i].secondObject == GetItemById(bId).transform ||
                linkItems[i].secondObject == GetItemById(aId).transform && linkItems[i].firstObject == GetItemById(bId).transform)
            {
                Destroy(linkItems[i].gameObject);
                linkItems.RemoveAt(i);
            }
        }
    }

    public void CreateLinkedItems()
    {
        if(items[currentHoverItem].page.pageType == PageTypes.Map)
        {
            for (int i = 1; i < items[currentHoverItem].page.texts.Count; i++)
            {
                SpawnItem(MapItemDataConverter.GetId(items[currentHoverItem].page.texts[i]), items[currentHoverItem].rectTransform.anchorMin, (float)i / (float)items[currentHoverItem].page.texts.Count);
            }
        }
        else
        {
            for (int i = 0; i < items[currentHoverItem].page.links.Count; i++)
            {
                SpawnItem(items[currentHoverItem].page.links[i], items[currentHoverItem].rectTransform.anchorMin, (float)i / (float)items[currentHoverItem].page.links.Count);
            }
        }
    }

        private MapItem GetItemById(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].page.id == id)
                return items[i];
        }
        return null;
    }

    private int GetIndexOfFirstMapItem(Vector2 pixelPosition)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if(RectBox.StaticRectBox(Camera.main.WorldToScreenPoint(items[i].rectTransform.position), 
                items[i].rectTransform.sizeDelta.x, items[i].rectTransform.sizeDelta.y).IsPointInBox(pixelPosition))
            {
                return i;
            }
        }
        return -1;
    }

    public void RemoveCurrentPage()
    {
        RemovePage(items[currentHoverItem].page.id);
    }

    public void OpenCurrentPage()
    {
        GameManager.Instance.OpenPage(items[currentHoverItem].page.id);
    }
}

public class RectBox
{
    public Vector2 position;
    public float width;
    public float height;

    public bool IsPointInBox(Vector2 point)
    {
        return point.x > position.x - width / 2 && point.x < position.x + width / 2 && point.y > position.y - height / 2 && point.y < position.y + height / 2;
    }

    public Vector2 NormalizedPositionInBox(Vector2 point)
    {
        return new Vector2((point.x - (position.x - width / 2)) / width, (point.y - (position.y - height / 2)) / height);
    }

    public static RectBox StaticRectBox(Vector2 position, float width, float height)
    {
        return new RectBox(position, width, height);
    }

    public RectBox(Vector2 position, float width, float height)
    {
        this.position = position;
        this.width = width;
        this.height = height;
    }
}

public static class MapItemDataConverter
{
    public static int GetId(string text)
    {
        int result = -1;
        bool success = int.TryParse(text.Split('|')[0], out result);
        if(success)
        {
            return result;
        }
        Debug.LogError("Error On Getting ID Out of Map-Item");
        return -1;
    }

    public static Vector2 GetPosition(string text)
    {
        float x = -1f;
        float y = -1f;
        bool success = float.TryParse(text.Split('|')[1], out x);
        success &= float.TryParse(text.Split('|')[2], out y);
        if(success)
        {
            return new Vector2(x, y);
        }
        Debug.LogError("Error On Getting Position Out of Map-Item");
        return Vector2.zero;
    }

    public static string GetTextFromItem(int id, Vector2 position)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(id.ToString());
        builder.Append("|");
        builder.Append(position.x.ToString());
        builder.Append("|");
        builder.Append(position.y.ToString());
        return builder.ToString();
    }
}
