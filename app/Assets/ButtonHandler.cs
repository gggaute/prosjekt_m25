using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ButtonHandler : MonoBehaviour
{
    public static Dictionary<string, List<StoryStep>> stories;

    void Awake()
    {
        stories = new Dictionary<string, List<StoryStep>>
        {
            {
                "marker1", new List<StoryStep>
                {
                    new StoryStep
                    {
                        text = "Marker 1 - Step 1",
                        image = Resources.Load<Texture>("Overlay1/liv"),
                        video = null
                    },
                    new StoryStep
                    {
                        text = "Marker 1 - Step 2",
                        image = null,
                        video = Resources.Load<VideoClip>("Overlay1/video")
                    }
                }
            },
            {
                "marker2", new List<StoryStep>
                {
                    new StoryStep
                    {
                        text = "Marker 2 - Intro",
                        image = Resources.Load<Texture>("Overlay2/bird"),
                        video = null
                    },
                    new StoryStep
                    {
                        text = "Marker 2 - step 2",
                        image = null,
                        video = Resources.Load<VideoClip>("Overlay2/dota")
                    }
                }
            },
            {
                "marker3", new List<StoryStep>
                {
                    new StoryStep
                    {
                        text = "Marker 3 - Welcome",
                        image = null,
                        video = Resources.Load<VideoClip>("Overlay1/video")
                    },
                    new StoryStep
                    {
                        text = "Marker 3 - step 2",
                        image = Resources.Load<Texture>("Overlay3/screenshot"),
                        video = null
                    }
                }
            }
        };
    }
    public void LoadStoryScene(string marker)
    {

        if (stories.ContainsKey(marker))
        {
            // Set the static holder
            StoryContainer.currentStory = stories[marker];

            // Load the AR scene
            SceneManager.LoadScene("AR");
        }
    }
}
