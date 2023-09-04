using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client {
    sealed class RopeRenderSystem : IEcsRunSystem 
    {
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        private readonly EcsFilterInject<Inc<PlayerInputComponent>> _inputFilter = default;

        public void Run(IEcsSystems systems)
        {
            if (_runtimeData.Value.GameState == GameState.PLAYING)
            {
                RopeRender();
            }
            else if (_runtimeData.Value.GameState == GameState.TAPTOSTART || _runtimeData.Value.GameState == GameState.NONE)
            {
                RopeRenderForSwinging();

                foreach (var entity in _inputFilter.Value)
                {
                    _inputFilter.Pools.Inc1.Get(entity).InputPressed = false;
                }
            }
        }

        private void RopeRenderForSwinging()
        {
            foreach (var entity in _inputFilter.Value)
            {
                ref var input = ref _inputFilter.Pools.Inc1.Get(entity);

                if (input.InputPressed)
                {
                    _sceneContext.Value.PlayerView.GetRopeTargetForSwinging().Forget();
                }

                if (input.InputHeld)
                {
                    _sceneContext.Value.PlayerView.UpdateSourceRope();
                }

                input.InputPressed = false;
            }
        }

        private void RopeRender()
        {
            foreach (var entity in _inputFilter.Value)
            {
                ref var input = ref _inputFilter.Pools.Inc1.Get(entity);

                if (input.InputPressed)
                {
                    _sceneContext.Value.PlayerView.GetRopeTarget();
                }

                if (input.InputReleased)
                {
                    _sceneContext.Value.PlayerView.ClearRope();
                }

                if (input.InputHeld)
                {
                    _sceneContext.Value.PlayerView.UpdateSourceRope();
                }
            }
        }
    }
}