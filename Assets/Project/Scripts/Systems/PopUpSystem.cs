using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client 
{
    sealed class PopUpSystem : IEcsRunSystem 
    {
        private readonly EcsFilterInject<Inc<PopUpRequest>> _popUpFilter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var it in _popUpFilter.Value)
            {
                var c = _popUpFilter.Pools.Inc1.Get(it);
                var popUP = Object.Instantiate<PopUpText>(c.UpText);
                popUP.transform.SetParent(c.Parent);
                popUP.rect.anchoredPosition = c.SpawnPosition;
                popUP.textUP.text = c.TextUP;

                _popUpFilter.Pools.Inc1.Del(it);
            }
        }
    }
}