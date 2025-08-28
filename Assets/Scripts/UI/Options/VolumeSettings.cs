using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    private const float minDB = -80f;
    private const float maxDB = 0f;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        volumeSlider.value = savedVolume;
        ApplyVolume(savedVolume);
        volumeSlider.onValueChanged.AddListener(ApplyVolume);
    }

    public void ApplyVolume(float sliderValue)
    {
        float dB = (sliderValue <= 0.0001f) ? minDB : Mathf.Log10(sliderValue) * 20f;

        audioMixer.SetFloat("MasterVolume", dB);

        // Save the slider value (0-1) for persistence
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
        PlayerPrefs.Save();
    }
}
