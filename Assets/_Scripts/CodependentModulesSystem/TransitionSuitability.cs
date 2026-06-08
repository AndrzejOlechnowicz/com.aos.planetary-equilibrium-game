public readonly struct TransitionSuitability
{
    public float Forward { get; }
    public float Reverse { get; }

    public TransitionSuitability(float forward)
    {
        Forward = forward;
        Reverse = 1f - forward;
    }
}