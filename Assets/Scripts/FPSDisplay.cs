using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;

    private int frameCount = 0;
    private float deltaTime = 0f;
    private float fps = 0f;
    private float lowestFPS = Mathf.Infinity;
    private float totalFPS = 0f;
    private int totalSeconds = 0;

    void Update()
    {
        frameCount++;
        deltaTime += Time.unscaledDeltaTime;

        if (deltaTime >= 1f)
        {
            fps = frameCount / deltaTime;
            totalFPS += fps;
            totalSeconds++;

            if (fps < lowestFPS)
            {
                lowestFPS = fps;
            }

            float averageFPS = totalFPS / totalSeconds;

            fpsText.text = $"FPS: {fps:F1}\nAverage: {averageFPS:F1}\nLowest: {lowestFPS:F1}";

            frameCount = 0;
            deltaTime = 0f;
        }
    }
}
