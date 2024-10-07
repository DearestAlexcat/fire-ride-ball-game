using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client 
{
    sealed class AutoFlightPlayerSystem : IEcsRunSystem 
    {
        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RocketPathService> _pathServices = default;

        int pivot = 0;
        bool flight = false;

        private bool PointInCircle(Vector3 a, Vector3 b, float r)
        {
            return (b.x - a.x) * (b.x - a.x) + (b.z - a.z) * (b.z - a.z) <= r * r;
        }

        public void Run(IEcsSystems systems) 
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                flight = !flight;
            }

            if (flight)
            {
                var player = _sceneContext.Value.PlayerView;

                if (PointInCircle(_pathServices.Value.curvePath[pivot], player.transform.position, _staticData.Value.pointRadiusDestination))
                {
                    pivot += _staticData.Value.skipWaypointsInFlightRocket;
                }

                var direction = (_pathServices.Value.curvePath[pivot] - player.transform.position).normalized;
                player.SetVelocity(Vector3.Lerp(player.GetVelocity(), direction * _staticData.Value.rocketSpeed, 0.2f));

                Quaternion newRotation = Quaternion.LookRotation(player.transform.forward + direction);
                player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation, Time.deltaTime * _staticData.Value.rocketRotateSpeed);
            }
        }
    }
}