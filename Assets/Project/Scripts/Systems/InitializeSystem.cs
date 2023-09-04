using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    class InitializeSystem : IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<UI> _ui = default;

        public void Init(IEcsSystems systems)
        {
            _ui.Value.CloseAll();
            _world.Value.ChangeState(GameState.BEFORE);
        }
    }
}