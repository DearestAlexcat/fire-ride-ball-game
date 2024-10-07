using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client 
{
    sealed class PlayerMovementSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;

        private readonly EcsFilterInject<Inc<InputHeld>> _heldFilter = default;
        private readonly EcsFilterInject<Inc<LaunchingRopeReguest>> _launchingFilter = default;

        public void Run(IEcsSystems systems)
        {
            if (_runtimeData.Value.GameState == GameState.PLAYING)
            {
                if (!_heldFilter.Value.IsEmpty() && _launchingFilter.Value.IsEmpty()) // player movement
                {
                    _sceneContext.Value.PlayerView.CircularMovement();
                }
            }
            else if (_runtimeData.Value.GameState == GameState.BEFORE ||
                     _runtimeData.Value.GameState == GameState.MAIN ||
                     _runtimeData.Value.GameState == GameState.TAPTOSTART) 
            {
                _sceneContext.Value.PlayerView.SwingingMovement();
            }
        }
    }
}