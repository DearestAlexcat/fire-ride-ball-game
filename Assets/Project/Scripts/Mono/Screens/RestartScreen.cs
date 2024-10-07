using Client;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

public class RestartScreen : Screen
{
    [SerializeField] TMP_Text bestScore;
    [SerializeField] TMP_Text currentScore;
    
    public void SetBestScore(int value)
    {
        bestScore.text = $"best: {value}";
    }

    public void SetCurrentScore(int value)
    {
        currentScore.text = value.ToString();
    }

    public void SetZoomBackState() // Animation Event
    {
        Service<EcsWorld>.Get().ChangeState(GameState.ZOOMBACK);
    }
}
