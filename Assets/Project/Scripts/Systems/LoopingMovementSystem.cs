using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client 
{
    sealed class LoopingMovementSystem : IEcsRunSystem 
    {      
        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsFilterInject<Inc<LoopingMovement>> _loopFilter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _loopFilter.Value)
            {
                var data = _loopFilter.Pools.Inc1.Get(entity);

                if (data.transform == null || data.transform.gameObject.activeSelf == false)
                {
                    systems.GetWorld().DelEntity(entity);
                    continue;
                }

                if(data.Movement)
                {
                    data.transform.position = Vector3.Lerp(data.transform.position,
                                data.startPosition + _staticData.Value.loopingShiftByY * Mathf.Sin(Time.time) * Vector3.up,
                                Time.deltaTime);
                }
             
                if (data.Rotate)
                {
                    data.transform.Rotate(Vector3.up, _staticData.Value.loopingRotateSpeed * Time.deltaTime);
                }
            }
        }
    }
}