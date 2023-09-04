
using TMPro;
using UnityEngine;
using Client;

public class RestartScreen : Screen
{
    [SerializeField] TMP_Text bestScore;
    [SerializeField] TMP_Text currentScore;

    public void EnableBlur()
    {
        var blurSettings = Service<SceneContext>.Get().BlurProfile.components[0] as BlurSettings;
        blurSettings.strength.value = Service<StaticData>.Get().levelBlur;
    }

    public void SetBestScore(int value)
    {
        bestScore.text = "best: " + value;
    }

    public void SetCurrentScore(int value)
    {
        currentScore.text = value.ToString();
    }
}
