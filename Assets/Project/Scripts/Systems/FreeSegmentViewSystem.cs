using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client 
{
    sealed class FreeSegmentViewSystem : IEcsRunSystem 
    {     
        private readonly EcsFilterInject<Inc<Group>> _groupFilter = default;
        private readonly EcsFilterInject<Inc<InGroup, CacheKey>> _elementFilter = default;

        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<RocketPathService> _pathServices = default;

        private readonly EcsCustomInject<PoolerService<Chunk>> _poolerChunk = default;
        private readonly EcsCustomInject<PoolerService<BonusCircle>> _poolerCircle = default;
        private readonly EcsCustomInject<PoolerService<Rocket>> _poolerRocket = default;

        public void Run (IEcsSystems systems) 
        {
            if (_groupFilter.Value.GetEntitiesCount() < _staticData.Value.minNumSegments) return;

            int min = int.MaxValue;
            int groupIndex = -1;

            // To delete, selected the oldest group
            foreach (var group in _groupFilter.Value)
            {
                var g = _groupFilter.Pools.Inc1.Get(group);
                if(g.Rank < min)
                {
                    min = g.Rank;
                    groupIndex = group;
                }
            }

            systems.GetWorld().DelEntity(groupIndex);

            int cacheKey;

            foreach (var i in _elementFilter.Value)
            {
                if(groupIndex == _elementFilter.Pools.Inc1.Get(i).GroupIndex)
                {
                    cacheKey = _elementFilter.Pools.Inc2.Get(i).Key;

                    _poolerRocket.Value.Free(cacheKey);
                    _poolerChunk.Value.Free(cacheKey);
                    _poolerCircle.Value.Free(cacheKey);

                    _pathServices.Value.curvePath.Remove(cacheKey);

                    systems.GetWorld().DelEntity(i);
                }
            }
        }
    }
}