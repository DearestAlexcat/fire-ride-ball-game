
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class GameScreenWorld : Screen
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text scoreTextXN;

    [SerializeField] CanvasScaler spaceScreenScaler;
    [SerializeField] RectTransform parent;
    [SerializeField] List<RectTransform> UIElements; 
    Vector3 scale;
    public float distance;

    bool Init = false;

    CancellationTokenSource disableCancellation = new CancellationTokenSource();

    private void FindSizeAboutDistance()
    {
        var frustumHeight = 2.0f * distance * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float scale = frustumHeight / UnityEngine.Screen.height;
        this.scale = Vector3.one * scale;
    }

    public void SetScoreXN(int n)
    {
        scoreTextXN.text = "x" + n;
    }

    public void SetScore(int val)
    {
        scoreText.text = val.ToString();
    }

    private void OnEnable()
    {
        if (!Init)
        {
            Init = true;

            FindSizeAboutDistance();
            SetSize();
            UIScaleAndPos();
        }

        if (disableCancellation != null)
        {
            disableCancellation.Dispose();
        }

        disableCancellation = new CancellationTokenSource();

        LookAt().Forget();
    }

    private void OnDisable()
    {
        disableCancellation.Cancel();
    }

    async UniTask LookAt()
    {
        while (!disableCancellation.IsCancellationRequested)
        {
            SetWorldPosition();
            await UniTask.NextFrame();
        }
    }

    private float GetScaleFactor()
    {
        Vector2 vector2 = new Vector2((float)UnityEngine.Screen.width, 
            (float)UnityEngine.Screen.height);

        float scaleFactor = Mathf.Pow
        (2f,
            Mathf.Lerp(
                        Mathf.Log(vector2.x / spaceScreenScaler.referenceResolution.x, 2f),
                        Mathf.Log(vector2.y / spaceScreenScaler.referenceResolution.y, 2f), 
                        spaceScreenScaler.matchWidthOrHeight
                    )

         );

        return scaleFactor;
    }

    private void UIScaleAndPos()
    {
        var size = GetScaleFactor();

        Vector3 temp;

        for (int i = 0; i < UIElements.Count; i++)
        {
            temp = UIElements[i].localScale;
            temp.x *= size;
            temp.y *= size;
            UIElements[i].localScale = temp;

            temp = UIElements[i].anchoredPosition;
            temp.x *= size;
            temp.y *= size;
            UIElements[i].anchoredPosition = temp;
        }
    }

    private void SetSize()
    {
        parent.sizeDelta = new Vector2(UnityEngine.Screen.width, UnityEngine.Screen.height);
        parent.localScale = scale;
    }

    private void SetWorldPosition()
    {
        parent.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
        parent.transform.LookAt(parent.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
