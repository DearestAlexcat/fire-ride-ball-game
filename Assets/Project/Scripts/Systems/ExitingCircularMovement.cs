using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client 
{
    sealed class ExitingCircularMovement : IEcsRunSystem 
    {
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;

        private readonly EcsFilterInject<Inc<InputReleased>> _releasedFilter = default;

        public void Run(IEcsSystems systems)
        {
            if (_runtimeData.Value.GameState == GameState.PLAYING)
            {
                if (!_releasedFilter.Value.IsEmpty())
                {
                    _sceneContext.Value.PlayerView.SetVelocity();
                }
            }
        }
    }
}