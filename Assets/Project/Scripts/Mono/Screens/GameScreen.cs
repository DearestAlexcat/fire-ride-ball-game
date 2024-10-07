using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

namespace Client
{
    public class GameScreen : Screen
    {
        [SerializeField] TMP_Text score;
        [SerializeField] TMP_Text factor;
        [SerializeField] TMP_Text scoreShadow;
        [SerializeField] TMP_Text factorShadow;

        int currentScore;
        public int Score
        {
            get => currentScore;
            set
            {
                currentScore = value;
                score.text = value.ToString();
                scoreShadow.text = score.text;
            }
        }

        public void SetFactor(int n)
        {
            factor.text = $"x{n}";
            factorShadow.text = factor.text;
        }

        public void IncrementScore(int value)
        {
            Score += value;
        }

        public void ShowPauseScreen()
        {
            Service<EcsWorld>.Get().ChangeState(GameState.PAUSE);
        }
    }
}