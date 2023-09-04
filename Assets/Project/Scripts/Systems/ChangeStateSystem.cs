using System;
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
        private readonly EcsCustomInject<StaticData> _staticData = default;
        private readonly EcsCustomInject<UI> _ui = default;
        
        private readonly EcsFilterInject<Inc<PlayerInputComponent>> _inputFilter = default;
        private readonly EcsFilterInject<Inc<ChangeStateEvent>> _stateFilter = default;

        private void SetActiveBlur(bool value)
        {
            var blurSettings = _sceneContext.Value.BlurProfile.components[0] as BlurSettings;
            blurSettings.strength.value = value ? _staticData.Value.levelBlur : blurSettings.strength.min;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _stateFilter.Value)
            {
                var state = _stateFilter.Pools.Inc1.Get(entity).NewGameState;

                _runtimeData.Value.PrevGameState = _runtimeData.Value.GameState;
                _runtimeData.Value.GameState = state;

                switch (state)
                {
                    case GameState.NONE:
                        //Debug.Log("NONE");

                        // Trigger event for swing
                        foreach (var item in _inputFilter.Value)
                        {
                            ref var c = ref _inputFilter.Pools.Inc1.Get(item);
                            c.InputPressed = true;
                            c.InputHeld = true;
                        }

                        break;

                    case GameState.TAPTOSTART:
                        //Debug.Log("TAPTOSTART");
                        
                        Time.timeScale = 1f;
                        SetActiveBlur(false);

                        _ui.Value.GameScreenWorld.Show(false);
                        _ui.Value.PauseScreen.Show(false);
                        _ui.Value.MainScreen.Show(false);
                        _ui.Value.GameScreen.Show();

                        break;

                    case GameState.BEFORE:
                        //Debug.Log("BEFORE");

                        SetActiveBlur(false);

                        Time.timeScale = 1f;

                        _ui.Value.GameScreen.SetScore(1);
                        _ui.Value.GameScreenWorld.Show(false);
                        _ui.Value.MainScreen.SetBestScore(Progress.BestScore);
                        _ui.Value.MainScreen.Show();
                        
                        break;
                   
                    case GameState.PLAYING:
                        //Debug.Log("PLAYING");

                        Time.timeScale = 1f;
                        SetActiveBlur(false);

                        _sceneContext.Value.PlayerView.ClearRope();

                        if (_runtimeData.Value.PrevGameState == GameState.TAPTOSTART)
                        {
                            _sceneContext.Value.PlayerView.SetVelocityZero().Forget();
                        }

                        _ui.Value.PauseScreen.Show(false);
                        _ui.Value.MainScreen.Show(false);

                        _ui.Value.GameScreenWorld.Show(false);
                        _ui.Value.GameScreen.Show();
                        
                        break;
                   
                    case GameState.LOSE:
                        //Debug.Log("LOSE");

                        //SetActiveBlur(true);

                        if (Progress.BestScore < _ui.Value.GameScreen.Score)
                        {
                            Progress.BestScore = _ui.Value.GameScreen.Score;
                        }

                        _ui.Value.RestartScreen.SetCurrentScore(_ui.Value.GameScreen.Score);
                        _ui.Value.RestartScreen.SetBestScore(Progress.BestScore);

                        _ui.Value.GameScreenWorld.Show();
                        _ui.Value.GameScreenWorld.SetScore(_ui.Value.GameScreen.Score);
                        _ui.Value.GameScreen.Show(false);
                       
                        _ui.Value.RestartScreen.Show();
                        
                        break;

                    case GameState.PAUSE:
                        //Debug.Log("PAUSE");
                        SetActiveBlur(true);

                        _ui.Value.GameScreenWorld.Show();
                        _ui.Value.GameScreenWorld.SetScore(_ui.Value.GameScreen.Score);
                        _ui.Value.GameScreen.Show(false);

                        _ui.Value.PauseScreen.Show();

                        Time.timeScale = 0f;
                       
                        break;
                   
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                systems.GetWorld().DelEntity(entity);
            }
        }
    }
}
