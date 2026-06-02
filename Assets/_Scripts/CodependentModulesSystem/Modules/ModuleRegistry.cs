using System;
using System.Collections.Generic;

namespace CodependantModules.Simulation
{
    public class ModuleRegistry
    {
        private readonly List<IModuleConfig> _configs = new List<IModuleConfig>();
        private readonly List<IModuleCompute> _computes = new List<IModuleCompute>();
        private readonly Dictionary<string, int> _nameToIndex = new Dictionary<string, int>(StringComparer.Ordinal);

        public IReadOnlyList<IModuleConfig> Modules => _configs;
        public IReadOnlyList<IModuleCompute> Computes => _computes;
        public int Count => _nameToIndex.Count;

        private readonly int seed;

        public ModuleRegistry(IModuleConfig[] moduleConfigs, int seed = 0)
        {
            // register all modules first
            foreach (var config in moduleConfigs)
            {
                Register(config, seed);
            }

            // initiate the modules
            InitModules();

            // get the computes
            foreach (var config in moduleConfigs)
            {
                _computes.Add(config.GetModuleCompute());
            }

        }

        public int GetIndex(string name)
        {
            if (_nameToIndex.ContainsKey(name))
            {
                return _nameToIndex[name];
            }
            else
            {
                UnityEngine.Debug.LogError($"There was no module with the name: {name} in the registry");
                return -1;
            }
        }

        // register module, call Init so module can register names
        private void Register(IModuleConfig module, int seed = 0)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            var id = module.Metadata?.Id ?? module.GetType().Name;
            if (!_nameToIndex.ContainsKey(id))
            {
                _nameToIndex[id] = _configs.Count;
                _configs.Add(module);
            }
        }

        private void InitModules()
        {
            foreach (var module in _configs)
            {
                module.Init(this, seed);
            }
        }
    }
}

