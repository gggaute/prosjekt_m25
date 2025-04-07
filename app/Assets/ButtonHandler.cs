using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ButtonHandler : MonoBehaviour
{
    public static Dictionary<string, List<ContentItem>> contentByLocation;

    [Header("UI References")]
    public ContentMenuManager contentMenuManager; // Reference to ContentMenuManager

    void Awake()
    {
        contentByLocation = new Dictionary<string, List<ContentItem>>
        {
            {
                "marker1", new List<ContentItem>
                {
                    new ContentItem
                    {
                        title = "Marker 1 - Content 1",
                        type = ContentType.Story,
                        source = ContentSource.System,
                        steps = new List<ContentStep>
                        {
                            new ContentStep
                            {
                                text = "Marker 1, content 1 - Step 1",
                                image = Resources.Load<Texture>("Overlay1/liv"),
                                video = null
                            },
                            new ContentStep
                            {
                                text = "Marker 1, content 1 - Step 2",
                                image = null,
                                video = Resources.Load<VideoClip>("Overlay1/video")
                            }
                        }
                    },
                    new ContentItem
                    {
                        title = "Content 2",
                        type = ContentType.Story,
                        source = ContentSource.User,
                        steps = new List<ContentStep>
                        {
                            new ContentStep
                            {
                                text = "Content 2 - step 1",
                                image = Resources.Load<Texture>("Overlay2/bird"),
                                video = null
                            },
                            new ContentStep
                            {
                                text = "Content 2 - step 2",
                                image = null,
                                video = Resources.Load<VideoClip>("Overlay2/dota")
                            }
                        }
                    }
                }
            },
            {
                "marker2", new List<ContentItem>
                {
                    new ContentItem
                    {
                        title = "Marker 2 - Content 1",
                        type = ContentType.Story,
                        source = ContentSource.System,
                        steps = new List<ContentStep>
                        {
                            new ContentStep
                            {
                                text = "Marker 2, content 1 - Step 1",
                                image = Resources.Load<Texture>("Overlay1/liv"),
                                video = null
                            },
                            new ContentStep
                            {
                                text = "Marker 2, content 1 - Step 2",
                                image = null,
                                video = Resources.Load<VideoClip>("Overlay1/video")
                            }
                        }
                    },
                    new ContentItem
                    {
                        title = "Content 2",
                        type = ContentType.Story,
                        source = ContentSource.User,
                        steps = new List<ContentStep>
                        {
                            new ContentStep
                            {
                                text = "Content 2 - step 1",
                                image = Resources.Load<Texture>("Overlay2/bird"),
                                video = null
                            },
                            new ContentStep
                            {
                                text = "Content 2 - step 2",
                                image = null,
                                video = Resources.Load<VideoClip>("Overlay2/dota")
                            }
                        }
                    }
                }
            }
        };
    }

    public void OpenMenu(string markerId)
    {
        contentMenuManager.OpenMenu(markerId);
    }
}

