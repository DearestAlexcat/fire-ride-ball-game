using UnityEngine;
using DG.Tweening;
using TMPro;

public class PopUpText : MonoBehaviour
{
    public TMP_Text textUP;
    public RectTransform rect;

    [Space]
    public float durationUP;
    public float durationFade;
    public float positionUP;
    public float timeToDestruction;

    [Space]
    public Ease easeFade;
    public Ease easeMove;

    [Space]
    public float minScale = 1.3f;
    public float maxScale = 1.1f;
    public float normalScale = 1.1f;
    public float scaleTime = 0.2f;

    private void Start()
    {
        rect.localScale = Vector3.one * minScale;

        DOTween.Sequence()
            .Append(rect.DOScale(maxScale, scaleTime)) 
            .Append(rect.DOScale(normalScale, scaleTime))
            .Append(
                rect.DOAnchorPos(rect.anchoredPosition + Vector2.up * positionUP, durationUP)
                    .SetEase(easeMove)
                    .SetLink(gameObject)
                    .OnComplete(DestoryGO)    
            )
            .Join(rect.DOScale(minScale, durationUP))
            .Join(textUP.DOFade(0f, durationFade).SetEase(easeFade));

        void DestoryGO()
        {
            Destroy(gameObject, timeToDestruction);
        }
    }
}
