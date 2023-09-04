using Game;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace Client 
{
    sealed class SpawnSegmentSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<RandomService> _randomService = default;
        private readonly EcsCustomInject<PoolerService> _poolerServices = default;

        private readonly EcsFilterInject<Inc<SpawnSegmentRequest>> _spawnRequestFilter = default;
        private readonly EcsFilterInject<Inc<CurrentPivots>> _segmentPivots = default;
        private readonly EcsFilterInject<Inc<CurvePathComponent>> _curveFilter = default;
 
        private int number;

        List<List<BonusCircle>> gateÑache = new List<List<BonusCircle>>();
        List<List<Chunk>> chunkCache = new List<List<Chunk>>();
        List<List<Rocket>> rocketÑache = new List<List<Rocket>>();
        List<int> segmentEntitys = new List<int>();

        int[] segmentMaterialIndices, segmentCurveIndices;
        int segmentMaterialPivot, segmentCurvePivot;

        int SegmentMaterialPivot
        {
            set
            {
                segmentMaterialPivot = value;
                CheckSegmentMaterialIndices();
            }

            get => segmentMaterialPivot;
        }

        int SegmentCurvePivot
        {
            set
            {
                segmentCurvePivot = value;
                CheckSegmentCurveIndices();
            }

            get => segmentCurvePivot;
        }

        public void Init(IEcsSystems systems)
        {
            _poolerServices.Value.InitChunkPooler(_staticData.Value.Chunk, _staticData.Value.numberChunksInSegment * 2, _sceneContext.Value.LevelRoot);
            _poolerServices.Value.InitGatePooler(_staticData.Value.BonusCircle, 5);

            NewSegmentCurveIndices();
            segmentMaterialIndices = Enumerable.Range(0, _staticData.Value.chunkMaterials.Count).ToArray();

            ÑachesInitialize(systems.GetWorld());
            SegmentInitialize(systems.GetWorld());
            SetFogMaterial();
        }

        public void Run(IEcsSystems systems) 
        {
            foreach (var entity in _spawnRequestFilter.Value)
            {
                SpawnSegment(entity, systems.GetWorld());
            }
        }
        
        private void NewSegmentCurveIndices()
        {
            segmentCurveIndices = new int[_staticData.Value.segmentPatterns.Count];
            ShuffleServices.FillWithRandoms(segmentCurveIndices);
        }

        private void CheckSegmentCurveIndices()
        {
            if (segmentCurvePivot == segmentCurveIndices.Length)
            {
                NewSegmentCurveIndices();
                segmentCurvePivot = 0;
            }
        }

        private void NewSegmentMaterialIndices()
        {
            segmentMaterialIndices = new int[_staticData.Value.chunkMaterials.Count];
            ShuffleServices.FillWithRandoms(segmentMaterialIndices);
        }

        private void CheckSegmentMaterialIndices()
        {
            if(segmentMaterialPivot == segmentMaterialIndices.Length)
            {
                NewSegmentMaterialIndices();
                segmentMaterialPivot = 0;
            }
        }

        private void SaveCurrentPivotsMaterialIndex()
        {
            foreach (var entity in _segmentPivots.Value)
            {
                _segmentPivots.Pools.Inc1.Get(entity).SegmentMaterialPivot = segmentMaterialIndices[segmentMaterialPivot];
                _segmentPivots.Pools.Inc1.Get(entity).SegmentCurvePivot = segmentCurveIndices[segmentCurvePivot];
            }
        }

        private int NewPathSegmentPoint(EcsWorld world)
        {
            return world.NewEntity<SegmentPathComponent>();
        }

        private void AddPointsToPathSegment(EcsWorld world, int entity)
        {
            world.GetPool<SegmentPathComponent>().Get(entity).SegmentPath = new List<Vector3>(chunkCache[chunkCache.Count - 1].Select(x => x.origPosition));
        }

        private void AddPointsToPathCurve()
        {
            if(_sceneContext.Value.PlayerView.IsRocketActive)
            {
                foreach (var entity in _curveFilter.Value)
                {
                    _curveFilter.Pools.Inc1.Get(entity).CurvePath.AddRange(chunkCache[chunkCache.Count - 1].Select(x => x.origPosition));
                }
            }
        }

        private void ÑachesInitialize(EcsWorld world)
        {
            world.NewEntityRef<CurvePathComponent>().CurvePath = new List<Vector3>();
        }

        private void SegmentInitialize(EcsWorld world)
        {
            world.NewEntity<CurrentPivots>();
            world.NewEntityRef<SpawnSegmentRequest>().NumberChunks = _staticData.Value.numberChunksInSegment;
        }

        private void FreeÑompletedSegment()
        {
            int i,
                gateCount = gateÑache[0].Count,
                chunkCount = chunkCache[0].Count;

            for (i = 0; i < gateCount; i++)
            {
                _poolerServices.Value.FreeGate(gateÑache[0][i]);
            }

            for (i = 0; i < chunkCount; i++)
            {
                _poolerServices.Value.FreeChunk(chunkCache[0][i]);
            }

            gateÑache.RemoveAt(0);
            chunkCache.RemoveAt(0);
        }

        private void DeletePassedRockets(EcsWorld world)
        {
            for (int i = 0; i < rocketÑache[0].Count; i++)
            {
                if(rocketÑache[0][i].gameObject.activeSelf)
                {
                    world.GetPool<Component<Rocket>>().Del(rocketÑache[0][i].Entity);
                }

                Object.Destroy(rocketÑache[0][i].gameObject);
                rocketÑache[0].RemoveAt(i);
            }

            rocketÑache.RemoveAt(0);
        }

        private void DeleteSegmentPath(EcsWorld world)
        {
            world.GetPool<SegmentPathComponent>().Del(segmentEntitys[0]);
            segmentEntitys.RemoveAt(0);
        }
 
        private void SpawnSegment(int entity, EcsWorld world)
        {
            ref var component = ref _spawnRequestFilter.Pools.Inc1.Get(entity);

            if(chunkCache.Count > 1)
            {
                FreeÑompletedSegment();
                DeletePassedRockets(world);
                DeleteSegmentPath(world);
            }

            chunkCache.Add(new List<Chunk>());
            gateÑache.Add(new List<BonusCircle>());
            rocketÑache.Add(new List<Rocket>());

            int segmentEntity = NewPathSegmentPoint(world);
            segmentEntitys.Add(segmentEntity);

            float racketSpawnInterval = _randomService.Value.Range(_staticData.Value.spawnIntervalRocket.x, _staticData.Value.spawnIntervalRocket.y);

            int i;

            for (i = 0; i < component.NumberChunks; i++)
            {
                var chunk = _poolerServices.Value.GetChunk();
                chunkCache[chunkCache.Count - 1].Add(chunk);

                if (!chunk.Init)
                {
                    chunk.Initialzie();
                }

                chunk.SetDistanceBetween(_staticData.Value.distanceBetweenChunks);
                SetPositionOnCurve(chunk, 1f * i / component.NumberChunks);
                SetWallMaterial(chunk);
                number++;

                // Set position for object "SpawnNextSegment"
                if (i == component.NumberChunks / 2)
                {
                    _sceneContext.Value.SpawnNextSegment.position = chunk.transform.position;
                    _sceneContext.Value.SpawnNextSegment.gameObject.SetActive(true);
                }

                // Rocket spawn
                if (i == (int)(component.NumberChunks / racketSpawnInterval)) 
                {
                    if (Random.value <= _staticData.Value.spawnProbabilityRocket)
                    {
                        var rocket = Object.Instantiate(_staticData.Value.Rocket);
                        rocket.Pivot = i;
                        rocket.SegmentEntity = segmentEntity;
                        rocket.transform.position = chunk.transform.position;

                        // See class RocketSystem
                        rocket.Entity = world.NewEntity<Component<Rocket>>();
                        world.GetPool<Component<Rocket>>().Get(rocket.Entity).Value = rocket;

                        rocketÑache[rocketÑache.Count - 1].Add(rocket);
                    }
                }

                // Set trigger position for color change object of next segment
                if (i == 0)
                {
                    _sceneContext.Value.NextSegment.transform.position = chunk.transform.position;
                    _sceneContext.Value.NextSegment.gameObject.SetActive(true);
                }
            }

#if UNITY_EDITOR
            if (component.NumberChunks / _staticData.Value.bonusCircleSpawnPosition < _staticData.Value.bonusCircleCount)
            {
                Debug.LogError($"Position {nameof(_staticData.Value.bonusCircleCount)} exceeds specified range {nameof(component.NumberChunks)}. See config Data");
            }
#endif
            // Placement and rotate of gate on field
            int spawnId;
            BonusCircle gate;

            for (i = 1; i <= _staticData.Value.bonusCircleCount; i++)
            {
                spawnId = (_staticData.Value.bonusCircleSpawnPosition * i) - 1;

                gate = _poolerServices.Value.GetGate(Vector3.zero, Quaternion.identity);
                gateÑache[gateÑache.Count - 1].Add(gate);

                gate.transform.localPosition = chunkCache[chunkCache.Count - 1][spawnId].transform.position;

                if (chunkCache[chunkCache.Count - 1].Count > spawnId + 1)
                {
                    gate.transform.rotation *= Quaternion.LookRotation(Vector3.forward + (chunkCache[chunkCache.Count - 1][spawnId + 1].origPosition - chunkCache[chunkCache.Count - 1][spawnId].origPosition).normalized);
                }
            }

            SaveCurrentPivotsMaterialIndex();          
            AddPointsToPathCurve();
            AddPointsToPathSegment(world, segmentEntity);

            SegmentCurvePivot++;
            SegmentMaterialPivot++;
        }

        private void SetFogMaterial()
        {
            var mat = _staticData.Value.chunkMaterials[segmentMaterialIndices[segmentMaterialPivot]].d;
            _sceneContext.Value.FogWall.material = mat;
            RenderSettings.fogColor = mat.color;
        }

        private void SetWallMaterial(Chunk chunk)
        {
            var item = _staticData.Value.chunkMaterials[segmentMaterialIndices[segmentMaterialPivot]];
            chunk.SetMaterial((number & 1) == 0 ? item.a : item.b, item.c);            
        }

        private void SetPositionOnCurve(Chunk chunk, float progress)
        {
            var temp = chunk.transform.position;
            temp.z = number * 2;

            int curveIndex = segmentCurveIndices[segmentCurvePivot];
            var curve = _staticData.Value.segmentPatterns[curveIndex];

            temp.y = curve.Evaluate(progress * curve.keys[curve.keys.Length - 1].time) * _staticData.Value.curveScalerByY;

            chunk.origPosition = temp;

            temp.y += Mathf.PerlinNoise1D(progress) * _randomService.Value.Range(_staticData.Value.noiseForCurvePoints.x, _staticData.Value.noiseForCurvePoints.y);

            chunk.transform.position = temp;
        }
    }
}