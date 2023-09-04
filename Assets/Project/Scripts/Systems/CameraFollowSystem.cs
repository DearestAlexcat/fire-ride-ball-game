using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client 
{
    sealed class CameraFollowSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        
        private Vector3 cameraCurrentVelocity;

        public void Init(IEcsSystems systems)
        {
            Camera.main.transform.rotation = Quaternion.Euler(_staticData.Value.cameraRotation);
            Camera.main.transform.position = _sceneContext.Value.PlayerView.transform.position + _staticData.Value.cameraStartOffset;

            _sceneContext.Value.FogWall.transform.position = Camera.main.transform.position - _staticData.Value.fogWallStartOffset;
            _sceneContext.Value.FogWall.transform.rotation = Quaternion.Euler(_staticData.Value.fogWallRotation);
        }

        public void Run(IEcsSystems systems)
        {
            SetOrientation();
        }

        private void SetOrientation()
        {
            Vector3 currentPosition, targetPoint, targetWallOffset;
            currentPosition = Camera.main.transform.position;
            Camera.main.transform.rotation = Quaternion.Euler(_staticData.Value.cameraRotation);

            targetPoint = _sceneContext.Value.PlayerView.transform.position + _staticData.Value.cameraOffset;
            targetWallOffset = _staticData.Value.fogWallOffset;

            if (_runtimeData.Value.GameState == GameState.BEFORE || _runtimeData.Value.GameState == GameState.NONE)
            {
                targetPoint = _sceneContext.Value.PlayerView.transform.position + _staticData.Value.cameraStartOffset;
                targetWallOffset = _staticData.Value.fogWallStartOffset;
            }

            if (_runtimeData.Value.GameState == GameState.TAPTOSTART)
            {
                Camera.main.transform.position = Vector3.MoveTowards(currentPosition, targetPoint, _staticData.Value.cameraSmoothnessSpeed * Time.deltaTime);
                
                _sceneContext.Value.FogWall.transform.position = Vector3.MoveTowards(currentPosition - targetWallOffset, targetPoint, _staticData.Value.cameraSmoothnessSpeed * Time.deltaTime);
            }
            else
            {
                Camera.main.transform.position = Vector3.SmoothDamp(currentPosition, targetPoint, ref cameraCurrentVelocity, _staticData.Value.cameraSmoothness);
                
                _sceneContext.Value.FogWall.transform.position = Vector3.MoveTowards(currentPosition - targetWallOffset, targetPoint, _staticData.Value.cameraSmoothnessSpeed * Time.deltaTime);
            }
        }
    }
}