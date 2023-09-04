using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;

namespace Client 
{
    sealed class OnTriggerSystem : IEcsInitSystem, IEcsRunSystem
    {
        EcsFilter _filterEnter;
        EcsPool<OnTriggerEnterEvent> _poolEnter;

        private readonly EcsCustomInject<SceneContext> _sceneContext = default;
        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<UI> _ui = default;

        private readonly EcsFilterInject<Inc<PlayerInputComponent>> _inputFilter = default;
        private readonly EcsFilterInject<Inc<CurrentPivots>> _segmentPivots = default;
        private readonly EcsFilterInject<Inc<AccrualScoreComponent>> _scoreFilter = default;
        private readonly EcsFilterInject<Inc<CurvePathComponent>> _curveFilter = default;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            _filterEnter = world.Filter<OnTriggerEnterEvent>().End();
            _poolEnter = world.GetPool<OnTriggerEnterEvent>();
        }

        public void Run(IEcsSystems systems) 
        {
            foreach (var entity in _filterEnter)
            {
                ref var eventData = ref _poolEnter.Get(entity);
                var other = eventData.collider;

                if (other == null)
                {
                    continue;
                }

                EcsWorld world = systems.GetWorld();

                if (other.CompareTag("ChunkObstacle"))
                {
                  //  Debug.Log("ChunkObstacle");

                    _sceneContext.Value.PlayerView.gameObject.SetActive(false);
                    
                    world.NewEntity<CameraShakeReguest>();                    
                    world.NewEntityRef<ChangeStateEvent>().NewGameState = GameState.LOSE;
                }

                else if (other.CompareTag("Rocket"))
                {
                    //Debug.Log("Rocket");

                    // Destroy object rocket
                    var rocketObj = other.gameObject.GetComponent<Rocket>();
                    rocketObj.gameObject.SetActive(false);
                    world.GetPool<Component<Rocket>>().Del(rocketObj.Entity);

                    // Logic with rocket flight work
                    float flightTime = Random.Range(_staticData.Value.flightTimeRangeRocket.x, _staticData.Value.flightTimeRangeRocket.y);

                    if (_sceneContext.Value.PlayerView.IsRocketActive)
                    {
                        world.GetPool<RocketMoveComponent>().Get(_sceneContext.Value.PlayerView.RocketEntity).Time += flightTime;
                        CreatePopUpText(world, _staticData.Value.PlusTime, "+" + flightTime + " sec", _staticData.Value.OnePlusShift, _ui.Value.GameScreen.transform);
                    }
                    else
                    {
                        _sceneContext.Value.PlayerView.RocketEntity = world.NewEntity<RocketMoveComponent>();
                        
                        ref var rocket = ref world.GetPool<RocketMoveComponent>().Get(_sceneContext.Value.PlayerView.RocketEntity);
                        rocket.Time = flightTime;
                        rocket.Pivot = other.gameObject.GetComponent<Rocket>().Pivot + _staticData.Value.skipWaypointsInFlightRocket;

                        _sceneContext.Value.PlayerView.SetActiveRocket(true);
                        CreatePopUpText(world, _staticData.Value.PlusTime, flightTime + " sec", _staticData.Value.OnePlusShift, _ui.Value.GameScreen.transform);

                        SetAccurelScoreX2();
                        RemovePlayerInput();
                        AddPointsToPathCurve(world, rocketObj.SegmentEntity);
                    }
                }

                else if (other.CompareTag("GotoNextSegment"))
                {
                    //Debug.Log("GotoNextSegment");

                    SetFogMaterial();
                    _sceneContext.Value.NextSegment.PlayFX();
                }

                else if (other.CompareTag("BonusCircleObj"))
                {
                    //Debug.Log("BonusCircleObj");

                    var gate = eventData.senderGameObject.transform.parent.GetComponentInParent<BonusCircle>();

                    if (eventData.senderGameObject.CompareTag("BonusCircleX2"))
                    {
                        gate.DestructionGate("BonusCircleX2");
                        gate.PlayFX();
                        _ui.Value.GameScreen.IncrementScore(2);
                        CreatePopUpText(world, _staticData.Value.PlusOne, "+2", _staticData.Value.OnePlusShift, _ui.Value.GameScreen.transform);
                    }
                    else // if (gate.gameObject.CompareTag("Gate2"))
                    {
                        gate.DestructionGate("BonusCircleX1");
                        _ui.Value.GameScreen.IncrementScore(1);
                        CreatePopUpText(world, _staticData.Value.PlusOne, "+1", _staticData.Value.OnePlusShift, _ui.Value.GameScreen.transform);
                    }
                }

                else if (other.CompareTag("SpawnNextSegment"))
                {
                    //Debug.Log("SpawnNextSegment");

                    _sceneContext.Value.SpawnNextSegment.gameObject.SetActive(false);
                    world.NewEntityRef<SpawnSegmentRequest>().NumberChunks = _staticData.Value.numberChunksInSegment;
                }
            }
        }

        private void RemovePlayerInput()
        {
            foreach (var item in _inputFilter.Value)
            {
                _inputFilter.Pools.Inc1.Del(item);
            }
        }

        private void AddPointsToPathCurve(EcsWorld world, int segmentEntity)
        {
            foreach (var entity in _curveFilter.Value)
            {
                _curveFilter.Pools.Inc1.Get(entity).CurvePath.AddRange(world.GetPool<SegmentPathComponent>().Get(segmentEntity).SegmentPath);
            }
        }

        private void SetAccurelScoreX2()
        {
            foreach (var item in _scoreFilter.Value)
            {
                _scoreFilter.Pools.Inc1.Get(item).AccrualIntervalScore = _staticData.Value.accrualIntervalScore * 0.5f;
            }

            _ui.Value.GameScreen.SetScoreXN(2);
            _ui.Value.GameScreenWorld.SetScoreXN(2);
        }

        private void SetFogMaterial()
        {
            foreach (var entity in _segmentPivots.Value)
            {
                var index = _segmentPivots.Pools.Inc1.Get(entity).SegmentMaterialPivot;
                var mat = _staticData.Value.chunkMaterials[index].d;

                _sceneContext.Value.FogWall.material.DOColor(mat.color, 1f);
                DOTween.To(() => RenderSettings.fogColor, color => RenderSettings.fogColor = color, mat.color, 1f);
            }    
        }

        private void CreatePopUpText(EcsWorld world, PopUpText upText, string msg, Vector3 position, Transform parent = null)
        {
            ref var component = ref world.NewEntityRef<PopUpRequest>();
            component.SpawnPosition = position;
            component.SpawnRotation = Quaternion.identity;
            component.TextUP = msg;
            component.Parent = parent;
            component.UpText = upText;
        }
    }
}