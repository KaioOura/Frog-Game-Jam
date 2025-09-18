
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    public void ShouldStopTime(bool shouldStop)
    {
        Time.timeScale = shouldStop ? 0f : 1f;
    }
}
