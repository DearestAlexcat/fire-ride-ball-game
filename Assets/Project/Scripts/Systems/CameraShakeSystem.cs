using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraShakeSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CameraShakeReguest>> _shakeFilter = default;
        private readonly EcsCustomInject<StaticData> _staticData = default;

        public void Run(IEcsSystems systems)
        {
            if (_shakeFilter.Value.IsEmpty()) return;

            foreach (var item in _shakeFilter.Value)
            {
                Shake().Forget();
            }
        }

        async UniTask Shake()
        {
            var origPosition = Camera.main.transform.localPosition;
            float elapsedTime = 0f;
            var thisTransform = Camera.main.transform;

            while (elapsedTime < _staticData.Value.shakeTime)
            {
                var randomPoint = origPosition + Random.insideUnitSphere * _staticData.Value.shakeAmount;
                
                thisTransform.localPosition = Vector3.Lerp(thisTransform.localPosition, randomPoint, Time.deltaTime * _staticData.Value.shakeSpeed);

                await UniTask.NextFrame();

                elapsedTime += Time.deltaTime;
            }

            thisTransform.localPosition = origPosition;
        }

    }
}
