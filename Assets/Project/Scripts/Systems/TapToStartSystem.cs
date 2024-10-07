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
            // exit GameState.TAPTOSTART state
            if (_runtimeData.Value.GameState == GameState.TAPTOSTART && Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0))
                {
                    return;
                }

                systems.GetWorld().NewEntityRef<ChangeStateEvent>().NewGameState = GameState.PLAYING;
            }

            // enter GameState.TAPTOSTART state
            else if (_runtimeData.Value.GameState == GameState.MAIN && Input.GetMouseButtonUp(0))   // MainScreen.cs sets GameState.MAIN state
            {
                systems.GetWorld().NewEntityRef<ChangeStateEvent>().NewGameState = GameState.TAPTOSTART;
            }
        }
    }
}