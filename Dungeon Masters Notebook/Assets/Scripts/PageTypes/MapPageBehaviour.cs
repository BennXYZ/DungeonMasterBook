using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MapPageBehaviour : PageBehaviour
{
    public List<MapItem> items;

    public GameObject mapItemPrefab;
    public RectTransform mapItemsParent;

    public float maxRangeTilDrag;

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
        for (int i = 0; i < texts.Count; i++)
        {
            SpawnItem(texts[i]);
        }
    }

    public void AddPage(int id)
    {
        SpawnItem(id);
    }

    public void RemovePage(int id)
    {
        for (int i = 0; i < GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.Count; i++)
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
                Destroy(items[i].gameObject);
                items.RemoveAt(i);
            }
        }
    }

    public MapItem SpawnItem(int id)
    {
        items.Add(Instantiate(mapItemPrefab, mapItemsParent).GetComponent<MapItem>());
        int newItemId = items.Count - 1;
        items[newItemId].page = GameManager.Instance.currentCampaign.GetPageById(id);
        items[newItemId].onEndMove = new UnityEngine.Events.UnityEvent();
        items[newItemId].onEndMove.AddListener(delegate { TextChanged(MapItemDataConverter.GetTextFromItem(items[newItemId].page.id, items[newItemId].rectTransform.anchorMin), newItemId); });

        Vector2 size = items[newItemId].rectTransform.sizeDelta;
        items[newItemId].rectTransform.anchorMin = Vector2.one * 0.5f;
        items[newItemId].rectTransform.anchorMax = Vector2.one * 0.5f;
        items[newItemId].rectTransform.offsetMin = Vector2.zero;
        items[newItemId].rectTransform.offsetMax = Vector2.zero;
        items[newItemId].rectTransform.sizeDelta = size;

        GameManager.Instance.currentCampaign.GetPageById(GameManager.Instance.mainPanel.currentPageId).texts.Add(MapItemDataConverter.GetTextFromItem(id, Vector2.one * 0.5f));

        return items[newItemId];
    }

    public MapItem SpawnItem(string text)
    {
        items.Add(Instantiate(mapItemPrefab, mapItemsParent).GetComponent<MapItem>());
        int newItemId = items.Count - 1;
        items[newItemId].page = GameManager.Instance.currentCampaign.GetPageById(MapItemDataConverter.GetId(text));
        items[newItemId].onEndMove = new UnityEngine.Events.UnityEvent();
        items[newItemId].onEndMove.AddListener(delegate { TextChanged(MapItemDataConverter.GetTextFromItem(items[newItemId].page.id, items[newItemId].rectTransform.anchorMin), newItemId); });

        Vector2 size = items[newItemId].rectTransform.sizeDelta;
        items[newItemId].rectTransform.anchorMin = MapItemDataConverter.GetPosition(text);
        items[newItemId].rectTransform.anchorMax = MapItemDataConverter.GetPosition(text);
        items[newItemId].rectTransform.offsetMin = Vector2.zero;
        items[newItemId].rectTransform.offsetMax = Vector2.zero;
        items[newItemId].rectTransform.sizeDelta = size;
        return items[newItemId];
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
        if (Input.GetMouseButtonDown(0))
        {
            currentHoverItem = GetIndexOfFirstMapItem(Input.mousePosition);
            if(currentHoverItem >= 0)
            {
                previourMousePosition = Input.mousePosition;
                startMousePosition = Input.mousePosition;
            }
            isDrag = false;
        }
        else if(Input.GetMouseButton(0) && currentHoverItem >= 0 && DeltaWorldMousePosition != Vector2.zero)
        {
            items[currentHoverItem].rectTransform.position = items[currentHoverItem].rectTransform.position + (Vector3)DeltaWorldMousePosition;
            previourMousePosition = Input.mousePosition;
            if(((Vector2)Input.mousePosition - startMousePosition).sqrMagnitude > maxRangeTilDrag && !isDrag)
            {
                isDrag = true;
            }
        }
        else if(Input.GetMouseButtonUp(0) && currentHoverItem >= 0)
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
            items[currentHoverItem].rectTransform.anchorMax = Vector2.right * positionOnParent.x / mapItemsParent.rect.width + Vector2.up * positionOnParent.y / mapItemsParent.rect.height;
            items[currentHoverItem].rectTransform.sizeDelta = size;

            items[currentHoverItem].onEndMove.Invoke();
            if (!isDrag)
            {
                Debug.Log("Do Stuff");
            }
            currentHoverItem = -1;
        }
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
