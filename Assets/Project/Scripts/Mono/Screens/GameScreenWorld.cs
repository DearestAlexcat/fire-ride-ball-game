using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Collections;

// This class is needed to place a copy of the UI elements in world space
// for the blur and shake algorithms to affect

public class GameScreenWorld : Screen
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text factorText;

    [SerializeField] TMP_Text scoreTextShadow;
    [SerializeField] TMP_Text factorShadow;

    [SerializeField] CanvasScaler spaceScreenScaler;
    [SerializeField] RectTransform parent;
    [SerializeField] List<RectTransform> scalableUI;

    [SerializeField] float distance;

    bool IsCancellationLookAt;

    public void SetFactor(int n)
    {
        factorText.text = $"x{n}";
        factorShadow.text = factorText.text;
    }

    public void SetScore(int val)
    {
        scoreText.text = val.ToString();
        scoreTextShadow.text = scoreText.text;
    }

    public IEnumerator Start()
    {
        yield return null;

        SetSize();
        SetScaleAndPosition();
    }
   

    private void OnEnable()
    {
        IsCancellationLookAt = false;

        LookAt().Forget();
    }

    private void OnDisable()
    {
        IsCancellationLookAt = true;
    }

    private void SetSize()
    {
        parent.sizeDelta = new Vector2(UnityEngine.Screen.width, UnityEngine.Screen.height);
        parent.localScale = FindScaleAboutDistance();
    }

    private Vector3 FindScaleAboutDistance()
    {
        return Vector3.one *
                ( 2.0f * distance * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) // frustumHeight
                / UnityEngine.Screen.height );
    }

    private float GetScaleFactor()
    {
        return Mathf.Pow(2f, Mathf.Lerp(
                        Mathf.Log(1f *  UnityEngine.Screen.width / spaceScreenScaler.referenceResolution.x, 2f),
                        Mathf.Log(1f *  UnityEngine.Screen.height / spaceScreenScaler.referenceResolution.y, 2f),
                        spaceScreenScaler.matchWidthOrHeight));
    }

    private void SetScaleAndPosition()
    {
        var factor = GetScaleFactor();

        for (int i = 0; i < scalableUI.Count; i++)
        {
            if(factor >= 1f)
            {
                scalableUI[i].localScale /= factor;
                scalableUI[i].anchoredPosition /= factor;
            }
            else
            {
                scalableUI[i].localScale *= factor;
                scalableUI[i].anchoredPosition *= factor;
            }
        }
    }
    
    async UniTask LookAt()
    {
        while (!IsCancellationLookAt) // Update the orientation because the camera is moving
        {
            SetWorldOrientation();
            await UniTask.NextFrame();
        }
    }

    private void SetWorldOrientation()
    {
        parent.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
        parent.transform.LookAt(parent.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}