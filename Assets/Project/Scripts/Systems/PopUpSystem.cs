using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client 
{
    sealed class PopUpSystem : IEcsRunSystem 
    {
        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsFilterInject<Inc<PopUpRequest>> _popUpFilter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var it in _popUpFilter.Value)
            {
                var popup = _popUpFilter.Pools.Inc1.Get(it);
                var instance = Object.Instantiate(_staticData.Value.PopupText);

                if (popup.Type == MesssageType.PLUSONE)
                {
                    instance.textUP.fontSize = 200;
                }
                else if (popup.Type == MesssageType.PLUSTIME)
                {
                    instance.textUP.fontSize = 130;
                }

                instance.transform.SetParent(Service<UI>.Get().GameScreen.transform);
                instance.rect.anchoredPosition = _staticData.Value.OnePlusShift;
                instance.textUP.text = popup.Value;

                _popUpFilter.Pools.Inc1.Del(it);
            }
        }
    }
}