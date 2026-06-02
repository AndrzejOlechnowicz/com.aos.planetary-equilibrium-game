using UnityEngine;

public class TimeController : MonoBehaviour
{
    public bool IsPaused => isPaused;

    private bool isPaused;
    private TimeSpeed currentSpeed;

    private void Awake()
    {
        isPaused = false;
        currentSpeed = TimeSpeed.normal;
    }

    public int GetSpeed()
    {
        switch (currentSpeed)
        {
            case TimeSpeed.normal:
                return 1;
            case TimeSpeed.fast: 
                return 2;
            case TimeSpeed.fastest:
                return 5;
            default:
                return 1;
        }
    }

    public void HandlePlay()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1;
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0;
        }
    }

    public void SetNormalSpeed()
    {
        currentSpeed = TimeSpeed.normal;
    }

    public void SetFasterSpeed()
    {
        currentSpeed = TimeSpeed.fast;
    }

    public void SetFastestSpeed()
    {
        currentSpeed = TimeSpeed.fastest;
    }

    private enum TimeSpeed
    {
        normal = 0,
        fast = 1,
        fastest = 2
    }
}
