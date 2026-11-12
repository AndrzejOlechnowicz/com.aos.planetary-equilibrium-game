using CodependantModules.Simulation;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float InputCost;
    private readonly float startInputCost = 5f;

    public static event Action OnSimulationStart;
    public event Action OnImpulseApplied;

    public SimulationCore Simulation => simulation;
    public ScoreManager ScoreManager => scoreManager;

    public TimeController TimeController => timeController;

    [SerializeField] private SavedState initialValues;

    [SerializeField] private float playersImpulseValue = 0.1f;
    [SerializeField] private TimeController timeController;
    [SerializeField] private PlanetHeightBaker planetBaker;

    [SerializeField] private bool isRandom;

    private SimulationCore simulation;
    
    private ScoreManager scoreManager;

    [Header("Testing")]
    [SerializeField] private int applyImpulseOverrideIndex = 0;

    private void Awake()
    {
        // initiate the modules registry
        var moduleRegistry = new ModuleRegistry(initialValues.GetModulesSO());

        // create the integrator
        IIntegrator integrator = new SemiImplicitIntegrator();
        
        // cache initial modules state
        var initialState = new SystemState(initialValues.GetInitialValues(), time: 0.0, dt: 0.002f, step: 0);

        // create the simulation
        simulation = new SimulationCore(moduleRegistry, integrator, initialState);

        // create the scoring manager
        scoreManager = new ScoreManager(this, simulation);

        if(isRandom)
            planetBaker.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        planetBaker.OnBakeCompleted += SimulationStart;

        // pause the simulation until the planet's height map is baked
        timeController.HandlePlay();

        // set the input cost
        InputCost = startInputCost;
    }

    private void OnDestroy()
    {
        planetBaker.OnBakeCompleted -= SimulationStart;
    }

    private void Update()
    {
        if(!timeController.IsPaused)
            NextStep();
    }

    public void ApplyImpulse(int valueId)
    {
        simulation.ApplyImpulse(valueId, playersImpulseValue);
        OnImpulseApplied?.Invoke();
        InputCost++;
    }

    private void NextStep()
    {
        var speedMultiplier = timeController.GetSpeed();
        simulation.StepMultiple(speedMultiplier);
    }

    private void SimulationStart()
    {
        timeController.HandlePlay();
        OnSimulationStart?.Invoke();
    }

    [EasyButtons.Button]
    private void ApplyImpulse()
    {
        ApplyImpulse(applyImpulseOverrideIndex);
    }

    [EasyButtons.Button]
    private void AddEQPoints()
    {
        ScoreManager.AddEqPoints();
    }
}