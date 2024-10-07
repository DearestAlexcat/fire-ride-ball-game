using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client 
{
    // First move the ball, and then render the rope

    sealed class RopeHandlingSystem : IEcsRunSystem 
    {
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;

        private readonly EcsFilterInject<Inc<InputPressed>> _pressedFilter = default;
        private readonly EcsFilterInject<Inc<InputHeld>> _heldFilter = default;
        private readonly EcsFilterInject<Inc<InputReleased>> _releasedFilter = default;

        private readonly EcsFilterInject<Inc<LaunchingRopeReguest>> _launchingFilter = default;

        public void Run(IEcsSystems systems)
        {
            if (_runtimeData.Value.GameState == GameState.PLAYING)
            {
                if (!_releasedFilter.Value.IsEmpty())
                {
                    _sceneContext.Value.PlayerView.ClearRope();
                    foreach (var item in _launchingFilter.Value)
                        systems.GetWorld().DelEntity(item);
                }

                if (!_pressedFilter.Value.IsEmpty())
                {
                    if(_sceneContext.Value.PlayerView.GetRopeTarget())
                    {
                        systems.GetWorld().NewEntity<LaunchingRopeReguest>();
                    }
                }

                if (!_heldFilter.Value.IsEmpty())
                {
                    _sceneContext.Value.PlayerView.UpdateSourceRope();
                }
            }
            else if (_runtimeData.Value.GameState == GameState.BEFORE ||
                     _runtimeData.Value.GameState == GameState.MAIN ||
                     _runtimeData.Value.GameState == GameState.TAPTOSTART)
            {
                // swing processing
                _sceneContext.Value.PlayerView.UpdateSourceRope();
            }
        }
    }
}