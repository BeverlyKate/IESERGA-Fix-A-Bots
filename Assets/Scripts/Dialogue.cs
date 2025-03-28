using UnityEngine;
using UnityEngine.UI;
using System;


[System.Serializable]
public class Dialogue
{
    public Image spriteImg;

    public string name;

    [TextArea(3, 10)]
    public string[] sentences;
}
