using UnityEngine;

public class CallbackParticle : MonoBehaviour
{
    [SerializeField] ParticleSystem thisparticle;
    [SerializeField] GameObject go;

    private void OnParticleSystemStopped()
    {
        go.SetActive(false);
    }
}
