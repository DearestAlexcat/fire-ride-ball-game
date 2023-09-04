using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using System.Collections;
using UnityEngine;
using LeoEcsPhysics;
using Game;


namespace Client 
{
    sealed class EcsStartup : MonoBehaviour {
        
        private EcsWorld _world;

        IEcsSystems _update;
        IEcsSystems _fixedUpdate;

        public RuntimeData runtimeData;
        public StaticData staticData;
        public SceneContext sceneContext;

        [SerializeField] private bool _useSeed = default;
        [SerializeField] private int _randomSeed = default;

        public IEnumerator Start() 
        {
            Application.targetFrameRate = 60;

            _world = new EcsWorld();
            _update = new EcsSystems(_world);
            _fixedUpdate = new EcsSystems(_world);
            EcsPhysicsEvents.ecsWorld = _world;

            runtimeData = new RuntimeData();

            Service<EcsWorld>.Set(_world);
            Service<SceneContext>.Set(sceneContext);
            Service<RuntimeData>.Set(runtimeData);
            Service<StaticData>.Set(staticData);

            GameInitialization.FullInit();

            _update
                .Add(new InitializeSystem())
                .Add(new ChangeStateSystem())

                .Add(new CameraInitSystem())
                .Add(new CameraPointerSystem())

                .Add(new PlayerInputSystem())
                .Add(new TapToStartSystem())

                .Add(new RopeRenderSystem())

                .Add(new ScoreSystem())

                .Add(new CameraShakeSystem())
                .DelHere<CameraShakeReguest>()

                .Add(new SpawnSegmentSystem())
                .DelHere<SpawnSegmentRequest>()

                .Add(new PopUpSystem())
#if UNITY_EDITOR
                .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Inject(staticData, runtimeData, sceneContext, Service<UI>.Get())
                .Inject(new RandomService(_useSeed ? _randomSeed : null))
                .Inject(new PoolerService())
                .Init();

            _fixedUpdate
                .Add(new PlayerMovementSystem())               
                .Add(new CameraFollowSystem())              
                .Add(new OnTriggerSystem())
                .Add(new RocketSystem())

                .Inject(staticData, sceneContext, runtimeData, Service<UI>.Get())
                .DelHerePhysics()
                .Init();

            yield return null;
        }

#if UNITY_EDITOR
        public void DisableBlur()
        {
            var blurSettings = Service<SceneContext>.Get().BlurProfile.components[0] as BlurSettings;
            blurSettings.strength.value = 0f;
        }
#endif

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

#if UNITY_EDITOR
            DisableBlur();
#endif
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