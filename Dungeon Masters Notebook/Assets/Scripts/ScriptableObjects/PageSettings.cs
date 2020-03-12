using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Page Data", menuName ="Pages/Page Data")]
public class PageSettings : ScriptableObject
{
    public PageTypes type;
    public int numberOfTexts;
    public int numberOfImages;
}
