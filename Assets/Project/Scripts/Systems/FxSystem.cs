using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class FxSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<FXRequest>> _fxFilter = null;

        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;

        public void Run(IEcsSystems systems)
        {
            if (_runtimeData.Value.GameState != GameState.PLAYING) return;

            foreach (var it in _fxFilter.Value)
            {
                var fx = Object.Instantiate(_staticData.Value.explosionEffect, _fxFilter.Pools.Inc1.Get(it).position, Quaternion.identity);
                Object.Destroy(fx, 10f);
            }
        }
    }
}
