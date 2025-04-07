using System.Collections.Generic;
using UnityEngine;

public enum ContentType { Story, Info, TrackedImage }
public enum ContentSource { System, User }

[System.Serializable]
public class ContentItem
{
    public string title;
    public ContentType type;
    public ContentSource source;
    public List<ContentStep> steps;
}
