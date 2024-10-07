using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using DG.Tweening;

namespace Client 
{
    sealed class ScoreSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        private readonly EcsCustomInject<StaticData> _staticData = default;

        private readonly EcsFilterInject<Inc<DelayComponent>> _delayFilter = default;

        private Sequence addScore = null;
        float delay;

        public void Init(IEcsSystems systems)
        {
            delay = _staticData.Value.standartDelay;
        }

        public void Run(IEcsSystems systems) 
        {
            if (_runtimeData.Value.GameState != GameState.PLAYING) return;

            // event to change delay
            foreach (var item in _delayFilter.Value)
            {
                delay = _delayFilter.Pools.Inc1.Get(item).Delay;
                systems.GetWorld().DelEntity(item);
            }

            if (addScore == null) // delayed code execution
            {
                addScore = DOTween.Sequence();
                addScore
                    .AppendInterval(delay) // delay the whole Sequence by n second
                    .OnComplete(() =>
                    {
                        Service<UI>.Get().GameScreen.IncrementScore(1);
                        addScore = null;
                    });
            }
        }
    }
}