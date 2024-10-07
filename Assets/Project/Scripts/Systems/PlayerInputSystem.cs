using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client 
{
    sealed class PlayerInputSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;

#if UNITY_EDITOR
        private readonly EcsCustomInject<StaticData> _staticData = default;
#endif

        private readonly EcsFilterInject<Inc<RocketMoveComponent>> _rocketMoveFiler = default;

        public void Run(IEcsSystems systems) 
        {
            if (_runtimeData.Value.GameState != GameState.PLAYING) return;
            
            if (!_rocketMoveFiler.Value.IsEmpty()) return;

            // Exit if interact with ui, for example, the pause button
            if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0)) return;

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F))
            {
                Time.timeScale = Time.timeScale < 1f ? 1f : _staticData.Value.slowmo;
            }
#endif

            if (Input.GetMouseButtonDown(0))
            {
                systems.GetWorld().NewEntity<InputPressed>();
            }

            if (Input.GetMouseButtonUp(0))
            {
                systems.GetWorld().NewEntity<InputReleased>();
            }
            
            if (Input.GetMouseButton(0))
            {
                systems.GetWorld().NewEntity<InputHeld>();
            }
        }
    }
}