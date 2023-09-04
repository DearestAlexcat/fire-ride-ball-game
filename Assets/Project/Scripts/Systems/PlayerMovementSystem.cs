using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client 
{
    sealed class PlayerMovementSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;

        private readonly EcsFilterInject<Inc<PlayerInputComponent>> _inputFilter = default;

        public void Run(IEcsSystems systems)
        {
            if (_runtimeData.Value.GameState == GameState.PLAYING)
            {         
                PlayerMovement();
            }
            else if(_runtimeData.Value.GameState == GameState.TAPTOSTART || _runtimeData.Value.GameState == GameState.NONE)
            {
                _sceneContext.Value.PlayerView.SwingingMovement();
            }
        }

        private void PlayerMovement()
        {
            foreach (var entity in _inputFilter.Value)
            {
                var input = _inputFilter.Pools.Inc1.Get(entity);
 
                if (input.InputHeld)
                {
                    _sceneContext.Value.PlayerView.CircularMovement();
                }
            }
        }
    }
}