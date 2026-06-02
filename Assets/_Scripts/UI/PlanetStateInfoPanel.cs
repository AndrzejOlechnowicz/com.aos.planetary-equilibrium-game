using CodependantModules.Simulation;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.HID.HID;

public class PlanetStateInfoPanel : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private ModuleInfoUI[] moduleInfoTexts;
    [SerializeField] private TextMeshProUGUI eqPointsText;
    [SerializeField] private Color equilibriumColor;
    [SerializeField] private TextMeshProUGUI scoreText;

    int waterModuleIndex;
    WaterModule waterModule;

    private void Start()
    {
        gameManager.Simulation.OnStepCompleted += UpdateInfo;

        waterModuleIndex = gameManager.Simulation.ModuleRegistry.GetIndex("water");
        waterModule = gameManager.Simulation.ModuleRegistry.Modules[waterModuleIndex] as WaterModule;

        for (int i = 0; i < moduleInfoTexts.Length; i++)
        {
            ModuleInfoUI moduleInfo = moduleInfoTexts[i];
            Sprite icon;
            icon = gameManager.Simulation.ModuleRegistry.Modules[i].Metadata.Icon;

            moduleInfo.SetIcon(icon);
        }
    }

    private void OnDestroy()
    {
        gameManager.Simulation.OnStepCompleted -= UpdateInfo;
    }

    private void UpdateInfo(SystemState planet, ModuleOutput stateDeltas)
    {
        if (gameManager.Simulation.ModuleRegistry == null)
            return;

        for (int i = 0; i < moduleInfoTexts.Length; i++)
        {
            ModuleInfoUI moduleInfo = moduleInfoTexts[i];
            string infoText;
            infoText = gameManager.Simulation.ModuleRegistry.Modules[i].GetReadableData(planet.Get(i));
            moduleInfo.SetData(infoText, stateDeltas.Derivatives[i]);
        }

        eqPointsText.SetText($"EQ Points: {gameManager.ScoreManager.EQPoints:n0}");

        if( gameManager.ScoreManager.IsEquilibrium)
            eqPointsText.color = equilibriumColor;
        else
            eqPointsText.color = Color.white;
        //equilibriumIndicator.SetActive(gameManager.Simulation.IsEquilibrium);

        scoreText.SetText(gameManager.ScoreManager.Score.ToString("n0"));
    }
}
