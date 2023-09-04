using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client 
{
    sealed class TapToStartSystem : IEcsRunSystem 
    {
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;
 
        public void Run(IEcsSystems systems) 
        {
            // MainScreen.cs sets the state to NONE
            if (_runtimeData.Value.GameState == GameState.NONE || _runtimeData.Value.GameState == GameState.TAPTOSTART)
            {
                if (Input.GetMouseButtonDown(0) && _runtimeData.Value.GameState == GameState.TAPTOSTART)
                {
                    if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0))
                    {
                        return;
                    }

                    systems.GetWorld().NewEntityRef<ChangeStateEvent>().NewGameState = GameState.PLAYING;
                }

                if (Input.GetMouseButtonDown(0) && _runtimeData.Value.GameState == GameState.NONE)
                {
                    systems.GetWorld().NewEntityRef<ChangeStateEvent>().NewGameState = GameState.TAPTOSTART;
                }
            }
        }
    }
}