using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client 
{
    sealed class PlayerInputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerInputComponent>> _inputFilter = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;

        public void Init(IEcsSystems systems)
        {
            systems.GetWorld().NewEntity<PlayerInputComponent>();
        }

        public void Run(IEcsSystems systems) 
        {
            if (_runtimeData.Value.GameState != GameState.PLAYING) return;
            DoInput();
        }

        public void DoInput()
        {
            foreach (var entity in _inputFilter.Value)
            {
                if(Input.GetKeyDown(KeyCode.F))
                {
                    Time.timeScale = 0.3f;
                }

                if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0))
                {
                    return;
                }

                _inputFilter.Pools.Inc1.Get(entity).InputPressed = Input.GetMouseButtonDown(0);
                _inputFilter.Pools.Inc1.Get(entity).InputReleased = Input.GetMouseButtonUp(0);
                _inputFilter.Pools.Inc1.Get(entity).InputHeld = Input.GetMouseButton(0);
            }
        }
    }
}