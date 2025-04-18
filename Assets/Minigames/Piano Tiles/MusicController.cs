using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public Slider slider;
    public AudioSource audioSource;

    public float lenghtPercent = 1f;
    public float volume = 0.5f;
    public float progressInSeconds = 0;
    public bool paused = false;

    private void Start()
    {
        audioSource.volume = volume;
        slider.onValueChanged.AddListener((value) => {audioSource.time = value * audioSource.clip.length * lenghtPercent; });
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = volume;
        progressInSeconds = audioSource.time;
        slider.SetValueWithoutNotify(progressInSeconds / (audioSource.clip.length * lenghtPercent));

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
            if (paused)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            audioSource.time = Mathf.Max(0, audioSource.time - 1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            audioSource.time = Mathf.Min(audioSource.time + 1, audioSource.clip.length * lenghtPercent);
        }
#endif
    }

    public void ChangeSpeed(float newSpeed)
    {
        audioSource.pitch = newSpeed;
    }

    public bool IsCompleted()
    {
        return audioSource.clip.length * lenghtPercent - progressInSeconds < 0.05f;
    }
}
