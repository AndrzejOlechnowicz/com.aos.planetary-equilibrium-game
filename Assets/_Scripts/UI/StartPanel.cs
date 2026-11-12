using UnityEngine;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private PlanetHeightBaker planetBaker;

    private float startTime;

    public void StartBaking()
    {
        startTime = Time.unscaledTime;
        GameManager.OnSimulationStart += PrintBakeTime;

        planetBaker.StartBakeAsync();
        Debug.Log($"AsyncBaking started!");
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.OnSimulationStart -= PrintBakeTime;
    }

    private void PrintBakeTime()
    {
        Debug.Log($"AsyncBaking took: {Time.unscaledTime - startTime}");
    }
}
