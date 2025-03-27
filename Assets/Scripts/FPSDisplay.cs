using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float fps;
    public TMPro.TextMeshProUGUI FPSCounterText;

    void Start()
    {
        InvokeRepeating("GetFPS", 1, 1);
    }

    void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        FPSCounterText.text = "FPS: " + fps.ToString();
    }
}
