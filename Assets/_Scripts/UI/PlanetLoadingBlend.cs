using System.Collections;
using TMPro;
using UI;
using UnityEngine;

public class PlanetLoadingBlend : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private CanvasGroupController canvasGroupController;
    [SerializeField] private TextMeshProUGUI loadingText;

    private Coroutine textCoro;

    private void Start()
    {
        gameManager.OnSimulationStart += Hide;
        loadingText.SetText("Finding suitable planet...");
        textCoro = StartCoroutine(LoadingTextAnimCoro());
    }

    private void Hide()
    {
        //StartCoroutine(HideWithDelay());
        StopCoroutine(textCoro);
        canvasGroupController.FadeIn(0.25f);
    }

    private IEnumerator HideWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        StopCoroutine(textCoro);
        canvasGroupController.FadeIn(0.25f);
    }

    private IEnumerator LoadingTextAnimCoro()
    {
        while (true) 
        {
            yield return new WaitForSecondsRealtime(0.25f);
            loadingText.SetText("Finding suitable planet");
            yield return new WaitForSecondsRealtime(0.25f);
            loadingText.SetText("Finding suitable planet.");
            yield return new WaitForSecondsRealtime(0.25f);
            loadingText.SetText("Finding suitable planet..");
            yield return new WaitForSecondsRealtime(0.25f);
            loadingText.SetText("Finding suitable planet...");
            yield return new WaitForSecondsRealtime(0.25f);
        }
    }
}
