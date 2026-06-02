using DG.Tweening;
using CodependantModules.Simulation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moduleText;
    [SerializeField] private Image moduleIcon;
    [SerializeField] private Image derivativeStateIcon;

    private DerivativeState lastState;
    private Tween derivativeTween;

    private void Awake()
    {
        lastState = DerivativeState.UNDEFINED;
    }

    public void SetIcon(Sprite icon)
    {
        moduleIcon.sprite = icon;
    }

    public void SetData(string state, float derivative)
    {
        

        var newState = GetDerivativeState(derivative);
        moduleText.SetText(state + $"| {derivative:n4}, derivative state: {newState}");
        if (newState != lastState)
        {
            DerivativeIconAnim(newState);
            lastState = newState;
        }
    }

    private DerivativeState GetDerivativeState(float derivative)
    {
        if (derivative < -0.02)
            return DerivativeState.FastDown;

        if(derivative < 0)
            return DerivativeState.Down;

        if (derivative > -0.001 && derivative < 0.001)
            return DerivativeState.Stable;

        if(derivative > 0 && derivative < 0.02)
            return DerivativeState.Up;

        return DerivativeState.FastUp;
    }

    private void DerivativeIconAnim(DerivativeState derivativeState)
    {
        // Kill previous animation (SetData is called every frame)
        derivativeTween?.Kill();
        derivativeTween = null;

        // Stable → hide icon and exit
        if (derivativeState == DerivativeState.Stable)
        {
            
            derivativeStateIcon.gameObject.SetActive(false);
            return;
        }

        derivativeStateIcon.gameObject.SetActive(true);

        // Determine direction
        bool isUp = derivativeState == DerivativeState.Up || derivativeState == DerivativeState.FastUp;
        bool isFast = derivativeState == DerivativeState.FastUp || derivativeState == DerivativeState.FastDown;

        // Rotate arrow
        derivativeStateIcon.rectTransform.localRotation =
            Quaternion.Euler(0, 0, isUp ? 180f : 0f);

        // Reset fill instantly
        derivativeStateIcon.fillAmount = 0f;

        float fillDuration = isFast ? 0.25f : 0.5f;
        float holdDuration = isFast ? 0.1f : 0.25f;

        // Build animation sequence
        Sequence seq = DOTween.Sequence();

        seq.Append(
                derivativeStateIcon
                    .DOFillAmount(1f, fillDuration)
            )
            .AppendInterval(holdDuration)
            .AppendCallback(() =>
            {
                // Snap back to zero instantly
                derivativeStateIcon.fillAmount = 0f;
            })
            .SetLoops(-1, LoopType.Restart);

        derivativeTween = seq;
    }

    private enum DerivativeState
    {
        UNDEFINED = -3,
        FastDown = -2,
        Down = -1,
        Stable = 0,
        Up = 1,
        FastUp = 2
    }
}
