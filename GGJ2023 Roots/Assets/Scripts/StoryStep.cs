using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum FontSize
{
    Normal,
    Large,
    ExtraLarge
}

[System.Serializable]
public class StoryStep
{
    [TextArea] public string Message;
    public FontSize FontSize = FontSize.Normal;
    public bool Completed = false;
}
