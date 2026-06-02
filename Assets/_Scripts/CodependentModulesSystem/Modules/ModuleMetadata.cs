using System.Collections.Generic;
using System;
using UnityEngine;

namespace CodependantModules.Simulation
{
    /// <summary>
    /// Metadata describing a module for editor tooling and dependency checks.
    /// </summary>
    [Serializable]
    public class ModuleMetadata
    {
        public string Id;
        public string DisplayName;
        public bool Deterministic = true;
        public string Description = string.Empty;
        public Sprite Icon;

        public ModuleMetadata() { }

        public ModuleMetadata(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }
}
