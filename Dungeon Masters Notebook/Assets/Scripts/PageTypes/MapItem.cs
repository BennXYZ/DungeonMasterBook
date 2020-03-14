﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapItem : MonoBehaviour
{
    public Page page;

    public UnityEvent onEndMove;

    public Image image;
    public Text text;

    public void SetPage(Page page)
    {
        this.page = page;
        this.page.onPageChanged.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        text.text = page.name;
        image.sprite = page.textures[0];
    }

    public RectTransform rectTransform
    {
        get
        {
            return transform as RectTransform;
        }
    }
}