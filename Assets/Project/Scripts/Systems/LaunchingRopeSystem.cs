using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client 
{
    sealed class LaunchingRopeSystem : IEcsRunSystem 
    {
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        
        private readonly EcsFilterInject<Inc<LaunchingRopeReguest>> _launchingFilter = default;

        public void Run (IEcsSystems systems)
        {
            if (_runtimeData.Value.GameState != GameState.PLAYING) return;

            foreach (var item in _launchingFilter.Value)
            {
                var player = _sceneContext.Value.PlayerView;
                ref var progress = ref _launchingFilter.Pools.Inc1.Get(item);

                progress.ProgressReachingRope += Time.deltaTime * player.LaunchingRopeDuration;
                player.LineSetPosition(progress.ProgressReachingRope);
               
                if (progress.ProgressReachingRope >= 1f)
                {
                    if(player.SetAngle())
                    {
                        systems.GetWorld().DelEntity(item);
                    }
                    else
                    {
                        progress.ProgressReachingRope = 0;
                    }
                }
            }
        }
    }
}