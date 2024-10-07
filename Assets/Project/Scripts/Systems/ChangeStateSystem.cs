using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    class ChangeStateSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        private readonly EcsCustomInject<SceneContext> _sceneContext = default;

        private readonly EcsFilterInject<Inc<ChangeStateEvent>> _stateFilter = default;

        // Entities are finally released after the system has finished its work? Release entities after all work.

        public void Run(IEcsSystems systems)
        {
            // This loop ensures that new requests (ChangeStateEvent) created from the same system are executed in the same frame.
            // However, one must not forget about the possibility of looping.
            while (!_stateFilter.Value.IsEmpty()) 
            {
                // Prevents possible exception: entity is already in filter.
                // Release entities after all the work.
                List<int> entitiesToBeDeleted = new List<int>();  

                foreach (var entity in _stateFilter.Value)
                {
                    var state = _stateFilter.Pools.Inc1.Get(entity).NewGameState;

                    _runtimeData.Value.PrevGameState = _runtimeData.Value.GameState;
                    _runtimeData.Value.GameState = state;

                    switch (state)
                    {
                        case GameState.BEFORE:

                            Time.timeScale = 1f;

                            Service<UI>.Get().GameScreen.Score = 1;
                            Service<UI>.Get().GameScreenWorld.Show(false);
                            Service<UI>.Get().MainScreen.UpdateBestScore();
                            Service<UI>.Get().MainScreen.Show();

                            _sceneContext.Value.PlayerView.GetRopeTargetForSwinging().Forget();

                            break;

                        case GameState.MAIN:

                            _sceneContext.Value.PlayerView.GetComponent<ParticleSystem>().Play();

                            break;

                        case GameState.TAPTOSTART:

                            Time.timeScale = 1f;

                            Service<UI>.Get().GameScreenWorld.Show(false);
                            Service<UI>.Get().PauseScreen.Show(false);
                            Service<UI>.Get().MainScreen.Show(false);
                            Service<UI>.Get().GameScreen.Show();

                            break;

                        case GameState.PLAYING:

                            Time.timeScale = 1f;

                            _sceneContext.Value.PlayerView.ClearRope();

                            Service<UI>.Get().PauseScreen.Show(false);
                            Service<UI>.Get().MainScreen.Show(false);
                            Service<UI>.Get().GameScreenWorld.Show(false);
                            Service<UI>.Get().GameScreen.Show();

                            break;

                        case GameState.LOSE:

                            if (Progress.BestScore < Service<UI>.Get().GameScreen.Score)
                            {
                                Progress.BestScore = Service<UI>.Get().GameScreen.Score;
                            }

                            Service<UI>.Get().RestartScreen.SetCurrentScore(Service<UI>.Get().GameScreen.Score);
                            Service<UI>.Get().RestartScreen.SetBestScore(Progress.BestScore);

                            Service<UI>.Get().GameScreenWorld.Show();
                            Service<UI>.Get().GameScreenWorld.SetScore(Service<UI>.Get().GameScreen.Score);
                            Service<UI>.Get().GameScreen.Show(false);

                            Service<UI>.Get().RestartScreen.Show();

                            break;

                        case GameState.PAUSE:

                            Service<UI>.Get().GameScreenWorld.Show();
                            Service<UI>.Get().GameScreenWorld.SetScore(Service<UI>.Get().GameScreen.Score);
                            Service<UI>.Get().GameScreen.Show(false);

                            Service<UI>.Get().PauseScreen.Show();

                            Time.timeScale = 0f;

                            break;
                    }

                    entitiesToBeDeleted.Add(entity);
                }

                // In this particular case, performing an operation via DelHere may result in the release of objects that have not yet been processed.
                // Because new requests may be added from the same system with the specified delay.
                foreach (var entity in entitiesToBeDeleted)
                {
                    systems.GetWorld().DelEntity(entity);
                }
            }
        }
    }
}
