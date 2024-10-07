using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class RocketFlightSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RocketMoveComponent>> _rocketMoveFiler = default;

        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RocketPathService> _pathServices = default;

        private bool PointInCircle(Vector3 a, Vector3 b, float r)
        {
            return (b.x - a.x) * (b.x - a.x) + (b.z - a.z) * (b.z - a.z) <= r * r;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _rocketMoveFiler.Value)
            {
                ref var item1 = ref _rocketMoveFiler.Pools.Inc1.Get(entity);

                item1.Time -= Time.deltaTime;

                if (item1.Time > 0f)
                {
                    var player = _sceneContext.Value.PlayerView;

                    if (PointInCircle(_pathServices.Value.curvePath[item1.Pointer], player.transform.position, _staticData.Value.pointRadiusDestination))
                    {
                        item1.Pointer = item1.Pointer + _staticData.Value.skipWaypointsInFlightRocket;
                    }

                    var direction = (_pathServices.Value.curvePath[item1.Pointer] - player.transform.position).normalized;
                    player.SetVelocity(Vector3.Lerp(player.GetVelocity(), direction * _staticData.Value.rocketSpeed, 0.2f));
                   
                    Quaternion newRotation = Quaternion.LookRotation(player.transform.forward + direction);
                    player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation, Time.deltaTime * _staticData.Value.rocketRotateSpeed);
                }
                else
                {
                    systems.GetWorld().DelEntity(entity);

                    systems.GetWorld().SetIncreaseScoreX();

                    _sceneContext.Value.PlayerView.SetActiveWarpEffect(false);
                    _sceneContext.Value.PlayerView.SetActiveRocket(false);

                    var rocket = Object.Instantiate(_staticData.Value.RocketFalling);
                    rocket.position = _sceneContext.Value.PlayerView.Rocket.transform.position;
                    rocket.rotation = _sceneContext.Value.PlayerView.Rocket.transform.rotation;

                    ref var falling = ref systems.GetWorld().NewEntityRef<Falling>();
                    falling.transform = rocket.transform;
                    falling.velocity = _sceneContext.Value.PlayerView.GetVelocity();
                    falling.mass = 5;

                    _sceneContext.Value.PlayerView.SetVelocity();
                    _sceneContext.Value.PlayerView.SetActiveOrb(true);
                }
            }
        }
    }
}