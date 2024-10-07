using UnityEngine;
using Cysharp.Threading.Tasks;

public class BonusCircle : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] public float power;
    
    [Space]
    [SerializeField] Rigidbody[] major;
    [SerializeField] Rigidbody[] minor;

    [Space]
    [SerializeField] Collider majorTrigger;
    [SerializeField] Collider minorTrigger;

    Quaternion[] qMajor, qMinor;

    private void Start()
    {
        qMajor = new Quaternion[major.Length];
        qMinor = new Quaternion[minor.Length];

        for (int i = 0; i < major.Length; i++)
            qMajor[i] = major[i].transform.rotation;
        for (int i = 0; i < minor.Length; i++)
            qMinor[i] = minor[i].transform.rotation;
    }

    private void OnEnable()
    {
        ResetGate();
    }

    public void ResetGate()
    {
        int i;

        for (i = 0; i < major.Length; i++)
        {
            major[i].transform.SetParent(transform);
            major[i].isKinematic = true;
            major[i].transform.localPosition = Vector3.zero;
            if (qMajor != null) 
                major[i].transform.rotation = qMajor[i];
        }

        for (i = 0; i < minor.Length; i++)
        {
            minor[i].transform.SetParent(transform);
            minor[i].isKinematic = true;
            minor[i].transform.localPosition = Vector3.zero;
            if (qMinor != null)
                minor[i].transform.rotation = qMinor[i];
        }

        majorTrigger.enabled = true;
        minorTrigger.enabled = true;
    }

    public void DestructionGate(string tag)
    {
        if(tag == "BonusCircleX1")
        {
            majorTrigger.enabled = false;
            Explosion(minor).Forget();
        }
        else
        {
            minorTrigger.enabled = false;
            Explosion(major).Forget();
        }
    }

    async UniTask Explosion(Rigidbody[] rbs)
    {
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].transform.SetParent(null);
            rbs[i].isKinematic = false;
            rbs[i].AddForce(rbs[i].transform.forward + Random.insideUnitSphere * power, ForceMode.VelocityChange);
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(5), ignoreTimeScale: false);

        if (this == null)
            return;

        gameObject.SetActive(false);
        ResetGate();
    }

    public void PlayFX()
    {
        ps.Play();
    }
}
