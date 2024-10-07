using Client;
using DG.Tweening;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : Screen
{
    [SerializeField] TMP_Text bestScore;
    [SerializeField] TMP_Text bestScoreShadow;

    [SerializeField] Image blackout;
    [SerializeField] float blackoutDuration = 1f;
    [SerializeField] Ease blackoutEase;

    public override void Show(bool state = true)
    {
        base.Show(state);

        if(state)
        {
            blackout
                .DOFade(0f, blackoutDuration)
                .SetEase(blackoutEase)
                .OnComplete(() => Service<EcsWorld>.Get().ChangeState(GameState.MAIN));
        }
    }

    private void OnDisable()
    {
        Color color = blackout.color;
        color.a = 1f;
        blackout.color = color;
    }

    public void UpdateBestScore()
    {
        bestScore.text = $"best:\n{Progress.BestScore}";
        bestScoreShadow.text = bestScore.text;
    }
}
