using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ButtonHandler : MonoBehaviour
{
    public void LoadStoryScene()
    {
        // Create a dummy story with some steps
        List<StoryStep> story = new List<StoryStep>();

        var step1 = new StoryStep();
        step1.text = "Welcome to the story!";
        step1.image = Resources.Load<Texture2D>("liv");
        step1.video = Resources.Load<VideoClip>("video");
        //another step
        var step2 = new StoryStep();
        step2.text = "Welcome to the second story!";
        step2.image = Resources.Load<Texture2D>("bird");
        step2.video = Resources.Load<VideoClip>("dota");

        story.Add(step1);
        story.Add(step2);

        // Set the static holder
        StoryContainer.currentStory = story;

        // Load the AR scene
        SceneManager.LoadScene("AR");
    }
}
