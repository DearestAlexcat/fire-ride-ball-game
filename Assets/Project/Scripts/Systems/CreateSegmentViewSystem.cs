using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;
using UnityEngine;

namespace Client 
{
    // globalColumnIndex is a shared index used as a key to hash an object in a pool.
    // 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | n | -> global column index
    // + | + | + | + | + | + | + | + | + | + | + | -> Chunk Pool
    //   |   |   |   |   | + |   |   |   |   |   | -> Rocket Pool
    //   |   |   |   |   |   | + | + | + | + | + | -> BonusCircle Pool
    // The scheme allows you to save on the InGroup and CacheKey components. 
    // However, the number of rockets and bonus circle should not exceed the number of chunks.
    // Otherwise, need to allocate additional components InGroup and CacheKey.

    sealed class CreateSegmentViewSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RandomService> _randomService = default;

        private readonly EcsCustomInject<PoolerService<Chunk>> _poolerChunk = default;
        private readonly EcsCustomInject<PoolerService<BonusCircle>> _poolerCircle = default;
        private readonly EcsCustomInject<PoolerService<Rocket>> _poolerRocket = default;
        private readonly EcsCustomInject<RocketPathService> _pathServices = default;

        private readonly EcsFilterInject<Inc<SpawnSegmentRequest>> _spawnFilter = default;
        private readonly EcsFilterInject<Inc<Group>> _groupFilter = default;

        int globalColumnIndex;

        int[] segmentMaterialIndices, segmentCurveIndices;
        int segmentMaterialPointer, segmentCurvePointer;

        public void Init(IEcsSystems systems)
        {
            NewSegmentCurveIndices();   // random filling
            segmentMaterialIndices = Enumerable.Range(0, _staticData.Value.chunkMaterials.Count).ToArray();  // sequential filling

            SetFogMaterial();
            SpawnSegment(systems.GetWorld());
        }

        public void Run(IEcsSystems systems) 
        {
            if (_spawnFilter.Value.IsEmpty()) return;

            SpawnSegment(systems.GetWorld());
        }

        private void SpawnSegment(EcsWorld world)
        {
            var groupEntity = world.NewEntity<Group>();
            _groupFilter.Pools.Inc1.Get(groupEntity).Rank = Time.frameCount;

            int i;
            float rocketSpawnFactor = Random.value < 0.5f ? _staticData.Value.rocketSpawnPoint1 : _staticData.Value.rocketSpawnPoint2;
            var numberChunks = _staticData.Value.numberChunksInSegment;

            Chunk chunk;
            int localStartIndex = globalColumnIndex;

            for (i = 0; i < numberChunks; i++)
            {
                var ingroupEntity = world.NewEntity<InGroup>();
                world.GetPool<InGroup>().Get(ingroupEntity).GroupIndex = groupEntity;
                world.AddEntityRef<CacheKey>(ingroupEntity).Key = globalColumnIndex;

                chunk = _poolerChunk.Value.Get(globalColumnIndex);

                SetChunkMaterials(chunk);
                SetPositionOnCurve(chunk, 1f * i / numberChunks, globalColumnIndex);

                _pathServices.Value.curvePath.Add(globalColumnIndex, chunk.transform.position);

                if (i == 0) // Set trigger position for color change object of next segment
                {
                    _sceneContext.Value.GateNextSegment.segmentMatIndex = segmentMaterialIndices[segmentMaterialPointer];
                    _sceneContext.Value.GateNextSegment.transform.position = chunk.transform.position;
                    _sceneContext.Value.GateNextSegment.gameObject.SetActive(true);
                }

                if (i == numberChunks * _staticData.Value.spawnNextSegmentPosition) // Set position for object "SpawnNextSegment"
                {
                    _sceneContext.Value.SpawnNextSegment.position = chunk.transform.position;
                    _sceneContext.Value.SpawnNextSegment.gameObject.SetActive(true);
                }

                if (i == (int)(numberChunks * rocketSpawnFactor))  // Rocket spawn
                {
                    if (Random.value <= _staticData.Value.rocketSpawnProbability)
                    {
                        var rocket = _poolerRocket.Value.Get(globalColumnIndex, false);
                        rocket.Pointer = globalColumnIndex;
                        rocket.transform.position = chunk.transform.position;
                        rocket.PlayRingFx();

                        ref var loop = ref world.NewEntityRef<LoopingMovement>();
                        loop.transform = rocket.transform;
                        loop.startPosition = chunk.transform.position;
                        loop.Movement = true;
                        loop.Rotate = true;
                    }
                }

                globalColumnIndex++;
            }

            int localEndIndex = globalColumnIndex - 1;

            PlaceCircles(world, localStartIndex, localEndIndex);

            IncrementMaterialPointer();
            IncrementCurvePointer();
        }

        void PlaceCircles(EcsWorld world, int localStartIndex, int localEndIndex)
        {
            // Placement and rotate of BonusCircle on field
            int positionIndex, columnIndex = globalColumnIndex;
            BonusCircle bc;

            // +1 so that the last element does not end up at the end of the segment
            int circleCount = Mathf.Clamp(_staticData.Value.bonusCircleCount + 1, 1, _staticData.Value.numberChunksInSegment);
            int spawnCircleStep = _staticData.Value.numberChunksInSegment / circleCount;

            for (int i = 1; i < circleCount; i++)
            {
                // Ñonvert from localIndex to globalIndex
                positionIndex = localStartIndex + (localEndIndex - localStartIndex) * (spawnCircleStep * i - 1) / _staticData.Value.numberChunksInSegment;

                columnIndex--; // Occupy free indexes from the end. See the scheme above

                bc = _poolerCircle.Value.Get(columnIndex);
                bc.transform.position = _pathServices.Value.curvePath[positionIndex];

                if (localStartIndex > _staticData.Value.numberChunksInSegment * 3)
                {
                    ref var loop = ref world.NewEntityRef<LoopingMovement>();
                    loop.transform = bc.transform;
                    loop.startPosition = bc.transform.position;
                    loop.Movement = true;
                }

                int j;
                Vector3 avgNearestPoints = _pathServices.Value.curvePath[positionIndex];
                for (j = 1; j <= 5; j++)
                {
                    if (positionIndex + j >= globalColumnIndex)
                        break;

                    avgNearestPoints += _pathServices.Value.curvePath[positionIndex + j];
                }

                j--;
                avgNearestPoints.y /= j;
                avgNearestPoints.z /= j;

                bc.transform.rotation *= Quaternion.LookRotation(Vector3.forward + (avgNearestPoints - bc.transform.position));
            }
        }

        void IncrementMaterialPointer()
        {
            segmentMaterialPointer++;

            if (segmentMaterialPointer == segmentMaterialIndices.Length)
            {
                segmentMaterialIndices = new int[_staticData.Value.chunkMaterials.Count];
                ShuffleServices.FillWithRandoms(segmentMaterialIndices);

                segmentMaterialPointer = 0;
            }
        }

        void IncrementCurvePointer()
        {
            segmentCurvePointer++;

            if (segmentCurvePointer == segmentCurveIndices.Length)
            {
                NewSegmentCurveIndices();
                segmentCurvePointer = 0;
            }
        }

        private void NewSegmentCurveIndices()
        {
            segmentCurveIndices = new int[_staticData.Value.segmentPatterns.Count];
            ShuffleServices.FillWithRandoms(segmentCurveIndices);
        }

        private void SetChunkMaterials(Chunk chunk)
        {
            var item = _staticData.Value.chunkMaterials[segmentMaterialIndices[segmentMaterialPointer]];
            chunk.SetMaterial((globalColumnIndex & 1) == 0 ? item.columnA : item.columnB, item.floor);
        }

        private void SetFogMaterial()
        {
            var mat = _staticData.Value.chunkMaterials[segmentMaterialIndices[segmentMaterialPointer]].fog;
            _sceneContext.Value.FogWall.material = mat;
            RenderSettings.fogColor = mat.color;
        }

        private void SetPositionOnCurve(Chunk chunk, float progress, int key)
        {
            var temp = chunk.transform.position;
            temp.z = globalColumnIndex * 2;

            int curveIndex = segmentCurveIndices[segmentCurvePointer];
            var curve = _staticData.Value.segmentPatterns[curveIndex];

            temp.y = curve.Evaluate(progress * curve.keys[curve.keys.Length - 1].time) * _staticData.Value.curveScalerByY; // time is y axis
            temp.y += Mathf.PerlinNoise1D(progress) * _randomService.Value.Range(_staticData.Value.noiseForCurvePoints.x, _staticData.Value.noiseForCurvePoints.y);
            
            chunk.transform.position = temp;
        }
    }
}