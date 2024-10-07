using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using System.Collections;
using UnityEngine;
using LeoEcsPhysics;

namespace Client 
{
    sealed class EcsStartup : MonoBehaviour 
    {
        private EcsWorld _world;

        IEcsSystems _update;
        IEcsSystems _fixedUpdate;

        public RuntimeData runtimeData;
        public StaticData staticData;

        [SerializeField] private bool _useSeed = default;
        //[SerializeField] private int _randomSeed = default;

        public IEnumerator Start() 
        {
            Application.targetFrameRate = 60;

            _world = new EcsWorld();
            _update = new EcsSystems(_world);
            _fixedUpdate = new EcsSystems(_world);
            EcsPhysicsEvents.ecsWorld = _world;

            runtimeData = new RuntimeData();

            Service<EcsWorld>.Set(_world);
            Service<RuntimeData>.Set(runtimeData);

            RocketPathService pathService = new();

            _update
                .Add(new InitializeSystem())
                .Add(new ChangeStateSystem())

                .Add(new CameraInitSystem())    

                .Add(new TapToStartSystem())
                
                .Add(new PlayerInputSystem())   

                .Add(new ScoreSystem())         

                .Add(new CameraShakeSystem())   

                .Add(new FreeSegmentViewSystem())       
                .Add(new CreateSegmentViewSystem())    
                
                .Add(new LoopingMovementSystem())

                .Add(new PopUpSystem())

                .Add(new RocketFlightSystem())
                .Add(new ExitingCircularMovement())   
                
                .Add(new PlayerMovementSystem())
                .Add(new FallingObjectSystem())

                .Add(new RopeHandlingSystem())
                .Add(new LaunchingRopeSystem())

                .Add(new FxSystem())
#if UNITY_EDITOR
                .Add(new AutoFlightPlayerSystem())      
#endif

                .Add(new CameraFollowSystem())          

                .DelHere<FXRequest>()
                .DelHere<CameraShakeReguest>()
                .DelHere<SpawnSegmentRequest>()
                .DelHere<InputReleased>()
                .DelHere<InputHeld>()
                .DelHere<InputPressed>()
#if UNITY_EDITOR
                .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Inject(staticData, runtimeData, GetComponent<SceneContext>())
                .Inject(new RandomService(_useSeed ? (int)(Time.realtimeSinceStartup * 100) : null))
                
                .Inject(new PoolerService<Chunk>(staticData.Chunk, staticData.numberChunksInSegment * 2), 
                        new PoolerService<BonusCircle>(staticData.BonusCircle, staticData.bonusCircleCount),
                        new PoolerService<Rocket>(staticData.Rocket, staticData.minNumSegments - 1))
                
                .Inject(pathService)
                .Init();

            _fixedUpdate
                .Add(new OnTriggerSystem())             
                .Inject(staticData, GetComponent<SceneContext>(), runtimeData)

                .DelHerePhysics()
                .Init();

            yield return null;
        }

        public void Update() 
        {
            _update?.Run();
        }

        public void FixedUpdate()
        {
            _fixedUpdate?.Run();
        }

        public void OnDestroy() 
        {
            if (_world != null)
            {
                EcsPhysicsEvents.ecsWorld = null;
                _world.Destroy();
                _world = null;
            }

            if (_update != null) 
            {
                _update.Destroy();   
                _update = null;
            }

            if (_fixedUpdate != null)
            {              
                _fixedUpdate.Destroy();            
                _fixedUpdate = null;            
            }
        }
    }
}