using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class StoryManager : MonoBehaviour
{
    public TextMeshProUGUI textElement;
    public RawImage imageElement;
    public VideoPlayer videoPlayer;

    private List<StoryStep> storySteps;
    private int currentStepIndex = 0;

    void Start()
    {
        // Grab the story from the container
        storySteps = new List<StoryStep>(StoryContainer.currentStory);

        if (storySteps == null || storySteps.Count == 0)
        {
            Debug.LogError("No story loaded!");
            return;
        }

        ShowCurrentStep();
    }

    public void NextStep()
    {
        if (currentStepIndex < storySteps.Count - 1)
        {
            currentStepIndex++;
            ShowCurrentStep();
        }
    }

    public void PreviousStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            ShowCurrentStep();
        }
    }

    private void ShowCurrentStep()
    {
        var step = storySteps[currentStepIndex];

        textElement.text = step.text;

        imageElement.texture = step.image;
        imageElement.gameObject.SetActive(step.image != null);

        if (step.video != null)
        {
            videoPlayer.clip = step.video;
            videoPlayer.Play();
            videoPlayer.gameObject.SetActive(true);
        }
        else
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
        }
    }
}
