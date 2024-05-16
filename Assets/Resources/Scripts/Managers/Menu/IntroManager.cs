using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        // Add a listener to the loopPointReached event
        videoPlayer.loopPointReached += OnVideoEnd;

        // Play the video
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        // Remove the listener to avoid memory leaks
        videoPlayer.loopPointReached -= OnVideoEnd;
    }

    private void Update()
    {
        if(InputManager.IsClickDown())
            OnVideoEnd(videoPlayer);
    }
}
