using CodependantModules.Simulation;
using UnityEngine;

public class AtmosphereVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Oxygen")]
    [SerializeField] private Material oxygenMaterial;
    [SerializeField] private Color minOxygenColor, maxOxygenColor;

    [Header("Green House Gasses")]
    [SerializeField] private Material ghgMaterial;
    [SerializeField] private Color minGhgColor, maxGhgColor;

    private int oxygenId, ghgId;

    private readonly string oxygenMatColorRef = "_FresnelColor";
    private readonly string ghgMatColorRef = "_Color";

    private void Start()
    {
        oxygenId = gameManager.Simulation.ModuleRegistry.GetIndex("oxygen");
        ghgId = gameManager.Simulation.ModuleRegistry.GetIndex("ghg");

        gameManager.Simulation.OnStepCompleted += UpdateVisual;
    }

    private void OnDestroy()
    {
        gameManager.Simulation.OnStepCompleted -= UpdateVisual;
    }

    private void UpdateVisual(SystemState state, ModuleOutput output)
    {
        var oxygenColor = Color.Lerp(minOxygenColor, maxOxygenColor, state.Get(oxygenId));
        oxygenMaterial.SetColor(oxygenMatColorRef, oxygenColor);

        var ghgColor = Color.Lerp(minGhgColor, maxGhgColor, state.Get(ghgId));
        ghgMaterial.SetColor(ghgMatColorRef, ghgColor);
    }
}
