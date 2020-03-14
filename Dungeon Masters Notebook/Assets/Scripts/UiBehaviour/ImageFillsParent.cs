using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image)), ExecuteInEditMode]
public class ImageFillsParent : MonoBehaviour
{
    public enum Modes
    {
        useHeight,
        useWidth
    }

    public Image image;

    public Modes mode;

    private void Update()
    {
        if(image != null && image.sprite != null && image.sprite.texture != null)
        {
            mode = image.sprite.rect.width / image.sprite.rect.height > 
                (transform.parent as RectTransform).rect.width / (transform.parent as RectTransform).rect.height ? Modes.useWidth : Modes.useHeight;

            (transform as RectTransform).anchorMin = Vector2.right * (mode == Modes.useHeight ? 0.5f : 0) + Vector2.up * (mode == Modes.useWidth ? 0.5f : 0);
            (transform as RectTransform).anchorMax = Vector2.right * (mode == Modes.useHeight ? 0.5f : 1) + Vector2.up * (mode == Modes.useWidth ? 0.5f : 1);
            (transform as RectTransform).offsetMin = Vector2.zero;
            (transform as RectTransform).offsetMax = Vector2.zero;
            switch (mode)
            {
                case Modes.useWidth:
                    (transform as RectTransform).sizeDelta = Vector2.right * (transform as RectTransform).sizeDelta.x + Vector2.up *
                        (transform.parent as RectTransform).rect.width * (image.sprite.rect.height / image.sprite.rect.width);
                    break;
                case Modes.useHeight:
                    (transform as RectTransform).sizeDelta = Vector2.right * (transform.parent as RectTransform).rect.height * (image.sprite.rect.width / image.sprite.rect.height)
                        + Vector2.up * (transform as RectTransform).sizeDelta.y;
                    break;
            }
        }
    }
}
