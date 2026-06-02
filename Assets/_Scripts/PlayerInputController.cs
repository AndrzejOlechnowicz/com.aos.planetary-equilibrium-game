using CodependantModules.Simulation;
using System;
using TMPro;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerInputButton buttonPrefab;
    [SerializeField] private TextMeshProUGUI costLabel;

    private PlayerInputButton[] buttons;

    private void Start()
    {
        buttons = new PlayerInputButton[gameManager.Simulation.ModuleRegistry.Count];

        for(int i = 0; i < gameManager.Simulation.ModuleRegistry.Count; i++)
        {
            var button = Instantiate(buttonPrefab, transform);
            button.SetUp(gameManager.Simulation.ModuleRegistry.Modules[i], i,(i) => gameManager.ApplyImpulse(i));
            buttons[i] = button;
        }

        gameManager.Simulation.OnStepCompleted += OnSimulationStep;

        gameManager.OnImpulseApplied += UpdateCostLabel;

        UpdateCostLabel();
    }

    private void OnDestroy()
    {
        gameManager.Simulation.OnStepCompleted -= OnSimulationStep;
        gameManager.OnImpulseApplied -= UpdateCostLabel;
    }

    private void UpdateCostLabel()
    {
        costLabel.SetText($"Cost: {GameManager.InputCost} EQ Points");
    }
    

    private void OnSimulationStep(SystemState state, ModuleOutput output)
    {
        bool canSpendPoints = gameManager.ScoreManager.EQPoints >= GameManager.InputCost;
        SetButtonsActive(canSpendPoints);
    }

    private void SetButtonsActive(bool active)
    {
        foreach(var inputButton in buttons)
        {
            inputButton.SetInteractable(active);
        }
    }
}
