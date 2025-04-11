using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public Slider slider;
    public AudioSource audioSource;

    public float progressInSeconds = 0;
    public bool paused = false;

    private void Start()
    {
        audioSource.volume = 0.05f;
        slider.onValueChanged.AddListener((value) => {audioSource.time = value * audioSource.clip.length; });
    }

    // Update is called once per frame
    void Update()
    {
        progressInSeconds = audioSource.time;
        slider.SetValueWithoutNotify(progressInSeconds / audioSource.clip.length);

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
            audioSource.time = Mathf.Min(audioSource.time + 1, audioSource.clip.length);
        }
#endif
    }

    public void ChangeSpeed(float newSpeed)
    {
        audioSource.pitch = newSpeed;
    }
}
