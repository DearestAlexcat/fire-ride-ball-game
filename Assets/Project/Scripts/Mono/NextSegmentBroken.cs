using UnityEngine;

public class NextSegmentBroken : MonoBehaviour
{
    public Transform brokenObject;
    public float radius, power, upwards;

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        Instantiate(brokenObject, transform.position, transform.rotation);
      
        var explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (var hit in colliders)
        {
            if (hit.attachedRigidbody)
            {
                hit.attachedRigidbody.AddExplosionForce(power, explosionPos, radius, upwards);
            }
        }
    }
}
