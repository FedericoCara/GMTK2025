using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphicsSettingsUI : MonoBehaviour
{
    public Dropdown graphicsDropdown;
    public Toggle postprocessingToggle;

    private Volume postProcessingVolume;

    private void Awake()
    {
        InitGraphicsDropdown();

        if (Camera.main != null)
        {
            Camera.main.TryGetComponent(out postProcessingVolume);
        }
    }

    public void InitGraphicsDropdown()
    {
        string[] names = QualitySettings.names;
        List<string> options = new List<string>(names);

        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(options);

        int currentQuality = QualitySettings.GetQualityLevel();
        graphicsDropdown.value = currentQuality;
    }

    public void SetGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }

    public void TogglePostProcessing()
    {
        if (postProcessingVolume != null)
        {
            postProcessingVolume.enabled = postprocessingToggle.isOn;
        }
    }
}