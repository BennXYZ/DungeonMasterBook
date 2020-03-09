using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag
{
    public string name;
    public bool active;

    public Tag(string name)
    {
        this.name = name;
        active = false;
    }
}
