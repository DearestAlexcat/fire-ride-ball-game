using Client;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoseScreen : Screen
{
    [SerializeField] Button buttonForward;
    [Space]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] Ease fadeEase;
    [Space]
    [SerializeField] Image blackout;
    [SerializeField] float blackoutDuration = 1f;
    [SerializeField] Ease blackoutEase;

    private void OnEnable()
    {
        canvasGroup.DOFade(1f, fadeDuration).SetEase(fadeEase);
    }

    private void OnDisable()
    {
        canvasGroup.alpha = 0f;

        Color color = blackout.color;
        color.a = 0f;
        blackout.color = color;
    }

    private void Start()
    {
        ButtonForwardInitialize();
    }

    private void ButtonForwardInitialize()
    {
        buttonForward.onClick.AddListener(() =>
        {
            blackout.DOFade(1f, blackoutDuration).SetEase(blackoutEase).OnComplete(() => Levels.LoadCurrentWithSkip());
        });
    }
}
