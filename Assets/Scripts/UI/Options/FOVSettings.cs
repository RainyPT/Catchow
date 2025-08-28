using UnityEngine;
using UnityEngine.UI;

public class FOVSettings : MonoBehaviour
{
    public Slider fovSlider;

    void Start()
    {
        float savedFOV = PlayerPrefs.GetFloat("FOV", 60f);
        fovSlider.value = savedFOV;
        fovSlider.onValueChanged.AddListener(SetFOV);
    }

    void SetFOV(float value)
    {
        PlayerPrefs.SetFloat("FOV", value);
        PlayerPrefs.Save();
    }
}
