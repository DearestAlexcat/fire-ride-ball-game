
using Client;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

public class GameScreen : Screen
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text scoreTextXN;

    int score;
    public int Score => score;

    public void SetScoreXN(int n)
    {
        scoreTextXN.text = "x" + n;
    }

    public void SetScore(int val)
    {
        score = val;
        scoreText.text = val.ToString();
    }

    public void IncrementScore(int val)
    {
        score += val;
        scoreText.text = score.ToString();
    }

    public void ShowPauseScreen()
    {
        Service<EcsWorld>.Get().ChangeState(GameState.PAUSE);
    }
}
