using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using DG.Tweening;

namespace Client 
{
    sealed class ScoreSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        private readonly EcsFilterInject<Inc<AccrualScoreComponent>> _scoreFilter = default;
        private readonly EcsCustomInject<StaticData> _staticData = default;

        private readonly EcsCustomInject<UI> _ui = default;

        private Sequence addScore = null;

        public void Init(IEcsSystems systems)
        {
            systems.GetWorld().NewEntityRef<AccrualScoreComponent>().AccrualIntervalScore = _staticData.Value.accrualIntervalScore;
        }

        public void Run(IEcsSystems systems) 
        {
            if (_runtimeData.Value.GameState == GameState.PLAYING)
            {
                foreach (var item in _scoreFilter.Value)
                {
                    if (addScore == null)
                    {
                        addScore = DOTween.Sequence();
                        addScore
                            .AppendInterval(_scoreFilter.Pools.Inc1.Get(item).AccrualIntervalScore)
                            .OnComplete(() =>
                            {
                                _ui.Value.GameScreen.IncrementScore(1);
                                addScore = null;
                            });
                    }
                }
            }
        }
    }
}