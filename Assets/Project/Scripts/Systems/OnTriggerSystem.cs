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

        public void Init(IEcsSystems systems)
        {
            _filterEnter = systems.GetWorld().Filter<OnTriggerEnterEvent>().End();
            _poolEnter = systems.GetWorld().GetPool<OnTriggerEnterEvent>();
        }

        public void Run(IEcsSystems systems) 
        {
            foreach (var entity in _filterEnter)
            {
                ref var eventData = ref _poolEnter.Get(entity);
                var other = eventData.collider;

                if (other == null) continue;

                EcsWorld world = systems.GetWorld();

                if (other.CompareTag("ChunkObstacle"))
                {
                    if (eventData.senderGameObject == null) return;

                    if (eventData.senderGameObject.CompareTag("Player"))
                    {
                        _sceneContext.Value.PlayerView.gameObject.SetActive(false);

                        world.NewEntityRef<CameraShakeReguest>();
                        world.NewEntityRef<ChangeStateEvent>().NewGameState = GameState.LOSE;
                    }
                    else if(eventData.senderGameObject.CompareTag("FallingRocket"))
                    {
                        systems.GetWorld().NewEntityRef<FXRequest>().position = eventData.senderGameObject.transform.position;
                        Object.Destroy(eventData.senderGameObject);
                    }
                }

                else if (other.CompareTag("Rocket"))
                {
                    if (!eventData.senderGameObject.TryGetComponent(out Player player)) return;

                    player.SetActiveOrb(false);
                    player.ClearRope();

                    var rocketObj = other.gameObject.GetComponent<Rocket>();
                    rocketObj.PlayPickupFx();
                    rocketObj.Disappearance();

                    // Logic with rocket flight work
                    float flightTime = Random.Range(_staticData.Value.flightTimeRangeRocket.x, _staticData.Value.flightTimeRangeRocket.y);

                    if (_sceneContext.Value.PlayerView.IsRocketActive)
                    {
                        world.GetPool<RocketMoveComponent>().Get(_sceneContext.Value.PlayerView.RocketEntity).Time += flightTime;
                        world.CreatePopUpText($"+{flightTime} sec", MesssageType.PLUSTIME);
                    }
                    else
                    {
                        _sceneContext.Value.PlayerView.RocketEntity = world.NewEntity<RocketMoveComponent>();
                        
                        ref var rocket = ref world.GetPool<RocketMoveComponent>().Get(_sceneContext.Value.PlayerView.RocketEntity);
                        rocket.Time = flightTime;
                        rocket.Pointer = other.gameObject.GetComponent<Rocket>().Pointer + _staticData.Value.skipWaypointsInFlightRocket;
                      
                        _sceneContext.Value.PlayerView.SetActiveWarpEffect(true);
                        _sceneContext.Value.PlayerView.SetActiveRocket(true);

                        world.CreatePopUpText($"{flightTime} sec", MesssageType.PLUSTIME);

                        systems.GetWorld().SetIncreaseScoreX(2);
                    }
                }

                else if (other.CompareTag("Interacting"))
                {
                    var gate = eventData.senderGameObject.transform.parent.GetComponentInParent<BonusCircle>();

                    if (eventData.senderGameObject.CompareTag("BonusCircleX2"))
                    {
                        gate.DestructionGate("BonusCircleX2");
                        gate.PlayFX();
                        Service<UI>.Get().GameScreen.IncrementScore(2);

                        world.CreatePopUpText("+2", MesssageType.PLUSONE);
                    }
                    else
                    {
                        gate.DestructionGate("BonusCircleX1");
                        Service<UI>.Get().GameScreen.IncrementScore(1);

                        world.CreatePopUpText("+1", MesssageType.PLUSONE);
                    }
                }

                else if (other.CompareTag("MoveNextSegment"))
                {
                    if (!eventData.senderGameObject.CompareTag("Player")) return;

                    SetFogMaterial(other.GetComponent<GateNextSegment>().segmentMatIndex);
                    _sceneContext.Value.GateNextSegment.PlayFX();
                }

                else if (other.CompareTag("SpawnNextSegment"))
                {
                    if (!eventData.senderGameObject.CompareTag("Player")) return;

                    _sceneContext.Value.SpawnNextSegment.gameObject.SetActive(false);
                    world.NewEntity<SpawnSegmentRequest>();
                }
            }
        }

        private void SetFogMaterial(int index)
        {
            var mat = _staticData.Value.chunkMaterials[index].fog;
            _sceneContext.Value.FogWall.material.DOColor(mat.color, 1f);
            DOTween.To(() => RenderSettings.fogColor, color => RenderSettings.fogColor = color, mat.color, 1f);
        }
    }
}