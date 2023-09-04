
using Client;
using DG.Tweening;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : Screen
{
    [SerializeField] TMP_Text bestScore;

    [SerializeField] Image blackout;
    [SerializeField] float blackoutDuration = 1f;
    [SerializeField] Ease blackoutEase;

    private void OnEnable()
    {
        blackout.DOFade(0f, blackoutDuration).SetEase(blackoutEase);
        Service<EcsWorld>.Get().ChangeState(GameState.NONE);
    }

    private void OnDisable()
    {
        Color color = blackout.color;
        color.a = 1f;
        blackout.color = color;
    }

    public void SetBestScore(int score)
    {
        bestScore.text = "best:\n" + score;
    }
}
