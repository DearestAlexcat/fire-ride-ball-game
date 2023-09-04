using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    sealed class RocketSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Component<Rocket>>> _rocketFilter = default;
        private readonly EcsFilterInject<Inc<RocketMoveComponent>> _rocketMoveFiler = default;
        private readonly EcsFilterInject<Inc<AccrualScoreComponent>> _scoreFilter = default;

        private readonly EcsCustomInject<UI> _ui = default;
        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsFilterInject<Inc<CurvePathComponent>> _curveFilter = default;

        private bool PointInCircle(Vector3 a, Vector3 b, float r)
        {
            return (b.x - a.x) * (b.x - a.x) + (b.z - a.z) * (b.z - a.z) <= r * r;
        }

        private void SetAccurelScoreX1()
        {
            foreach (var item in _scoreFilter.Value)
            {
                _scoreFilter.Pools.Inc1.Get(item).AccrualIntervalScore = _staticData.Value.accrualIntervalScore;
            }

            _ui.Value.GameScreen.SetScoreXN(1);
            _ui.Value.GameScreenWorld.SetScoreXN(1);
        }

        private void SetupPlayerInput(EcsWorld world)
        {
            world.NewEntity<PlayerInputComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _rocketFilter.Value)
            {
                _rocketFilter.Pools.Inc1.Get(entity).Value.DoYoyo(_staticData.Value);
            }

            foreach (var entity in _rocketMoveFiler.Value)
            {
                ref var item = ref _rocketMoveFiler.Pools.Inc1.Get(entity);

                item.Time -= Time.deltaTime;

                foreach (var pathEntity in _curveFilter.Value)
                {
                    ref var curve = ref _curveFilter.Pools.Inc1.Get(pathEntity);

                    if (item.Time > 0f)
                    {
                        var player = _sceneContext.Value.PlayerView;

                        if (PointInCircle(curve.CurvePath[item.Pivot], player.transform.position, _staticData.Value.pointRadiusDestination))
                        {
                            item.Pivot = item.Pivot + _staticData.Value.skipWaypointsInFlightRocket;
                        }

                        if (item.Pivot >= curve.CurvePath.Count)
                        {
                            curve.CurvePath = new List<Vector3>();
                            _rocketMoveFiler.Pools.Inc1.Del(entity);
                            _sceneContext.Value.PlayerView.SetActiveRocket(false);
                            SetAccurelScoreX1();
                            SetupPlayerInput(systems.GetWorld());
                            return;
                        }

                        var movement = curve.CurvePath[item.Pivot] - player.transform.position;
                        player.ThisRigidBody.velocity = Vector3.Lerp(player.ThisRigidBody.velocity, movement.normalized * player.RocketSpeed, 0.2f);
                        
                        Quaternion newRotation = Quaternion.LookRotation(player.transform.forward + movement);
                        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation, Time.deltaTime * player.RocketRotateSpeed);
                    }
                    else
                    {
                        curve.CurvePath = new List<Vector3>();    
                        SetAccurelScoreX1();
                        SetupPlayerInput(systems.GetWorld());
                        _rocketMoveFiler.Pools.Inc1.Del(entity);
                        _sceneContext.Value.PlayerView.SetActiveRocket(false);
                    }
                }
            }
        }
    }
}