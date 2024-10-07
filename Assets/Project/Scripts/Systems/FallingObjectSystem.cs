using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client 
{
    sealed class FallingObjectSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        private readonly EcsCustomInject<StaticData> _staticData = default;

        private readonly EcsFilterInject<Inc<Falling>> _fallingFilter = default;

        float gravitationalAcceleration = Physics.gravity.magnitude;

        public void Init(IEcsSystems systems)
        {
            PlayerInit(systems);
        }

        void PlayerInit(IEcsSystems systems)
        {
            int entity = systems.GetWorld().NewEntity<Falling>();
            ref var falling = ref systems.GetWorld().GetEntityRef<Falling>(entity);
            falling.transform = _sceneContext.Value.PlayerView.transform;
            falling.mass = _staticData.Value.playerMass;

            _sceneContext.Value.PlayerView.fallingData = systems.GetWorld().PackEntityWithWorld(entity);
        }

        public void Run(IEcsSystems systems) 
        {
            if (_runtimeData.Value.GameState != GameState.PLAYING) return;

            foreach (var entity in _fallingFilter.Value)
            {
                ref var item = ref _fallingFilter.Pools.Inc1.Get(entity);

                if (item.transform == null)
                {
                    systems.GetWorld().DelEntity(entity);
                    continue;
                }

                // Apply gravity to the object
                item.velocity += Vector3.down * gravitationalAcceleration * item.mass * Time.deltaTime;

                // Update object position based on speed
                item.transform.position += item.velocity * Time.deltaTime;
            }
        }
    }
}