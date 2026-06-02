using CodependantModules.Simulation;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    private int valueId;
    private Action<int> onButtonDownAction;
    private IModuleConfig simulationModule;

    public void SetUp(IModuleConfig module, int valueId, Action<int> onButtonDownAction)
    {
        this.valueId = valueId;
        this.onButtonDownAction = onButtonDownAction;
        button.onClick.AddListener(OnClick);
        buttonText.SetText($"Add {module.Metadata.Id}");
    }

    public void SetInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;
    }

    private void OnClick()
    {
        onButtonDownAction?.Invoke(valueId);
    }
}
