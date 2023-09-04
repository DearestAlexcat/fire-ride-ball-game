using UnityEngine;

public class BonusCircle : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] public float power;
    
    [Space]
    [SerializeField] Rigidbody[] gate1;
    [SerializeField] Rigidbody[] gate2;

    [Space]
    [SerializeField] Collider gate1Trigger;
    [SerializeField] Collider gate2Trigger;


    Quaternion[] rotGate1, rotGate2;

    private void Start()
    {
        rotGate1 = new Quaternion[gate1.Length];
        rotGate2 = new Quaternion[gate2.Length];

        for (int i = 0; i < gate1.Length; i++)
        {
            rotGate1[i] = gate1[i].transform.rotation;
        }

        for (int i = 0; i < gate2.Length; i++)
        {
            rotGate2[i] = gate2[i].transform.rotation;
        }
    }

    private void OnEnable()
    {
        ResetGate();
    }

    public void ResetGate()
    {
        for (int i = 0; i < gate1.Length; i++)
        {
            gate1[i].isKinematic = true;
            gate1[i].transform.localPosition = Vector3.zero;
            if(rotGate1 != null)
                gate1[i].transform.rotation = rotGate1[i];
        }

        for (int i = 0; i < gate2.Length; i++)
        {
            gate2[i].isKinematic = true;
            gate2[i].transform.localPosition = Vector3.zero;
            if (rotGate2 != null)
                gate2[i].transform.rotation = rotGate2[i];
        }

        gate1Trigger.enabled = true;
        gate2Trigger.enabled = true;
    }

    public void DestructionGate(string tag)
    {
        if(tag == "BonusCircleX1")
        {
            gate1Trigger.enabled = false;

            for (int i = 0; i < gate2.Length; i++)
            {
                gate2[i].isKinematic = false;
            }

            Explosion(gate2);
        }
        else
        {
            gate2Trigger.enabled = false;

            for (int i = 0; i < gate1.Length; i++)
            {
                gate1[i].isKinematic = false;
            }

            Explosion(gate1);
        }
    }

    void Explosion(Rigidbody[] rig)
    {
        foreach (var hit in rig)
        {
            hit.AddForceAtPosition((Vector3.forward + Random.insideUnitSphere) * power, transform.position);
        }
    }

    public void PlayFX()
    {
        ps.Play();
    }
}
