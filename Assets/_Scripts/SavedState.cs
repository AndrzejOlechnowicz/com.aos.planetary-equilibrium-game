using UnityEngine;
using CodependantModules.Simulation;
using System;

[CreateAssetMenu(menuName = "Planetary/Values/State")]
public class SavedState : ScriptableObject
{
    [SerializeField] private ModuleWithInitialValue[] modules;

    public float[] GetInitialValues()
    {
        float[] result = new float[modules.Length];

        for (int i = 0; i < modules.Length; i++)
        {
            result[i] = modules[i].initialValue;
        }

        return result;
    }

    public IModuleConfig[] GetModulesSO()
    {
        IModuleConfig[] result = new IModuleConfig[modules.Length];

        for (int i = 0; i < modules.Length; i++)
        {
            result[i] = modules[i].moduleSO as IModuleConfig;
        }

        return result;
    }
}

[Serializable]
public class ModuleWithInitialValue
{
    public ScriptableObject moduleSO;
    public float initialValue;
}
