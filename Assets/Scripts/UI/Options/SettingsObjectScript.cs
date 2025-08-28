using UnityEngine;
using UnityEngine.Audio;

public class SettingsObjectScript : MonoBehaviour
{
    public AudioMixer audioMixer;
    private float savedVolume;
    private float db;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        db = (savedVolume <= 0.0001f) ? -80f : Mathf.Log10(savedVolume) * 20f;
        audioMixer.SetFloat("MasterVolume", db);
    }

    // Update is called once per frame
    void Update()
    {
        savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        db = (savedVolume <= 0.0001f) ? -80f : Mathf.Log10(savedVolume) * 20f;
        audioMixer.SetFloat("MasterVolume", db);
    }
}
