using UnityEngine;

public class NextSegmentBroken : MonoBehaviour
{
    public Transform brokenObject;
    public float magtudeCol, radius, power, upwards;

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        Instantiate(brokenObject, transform.position, transform.rotation);
        //brokenObject.localScale = transform.localScale;
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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.relativeVelocity.magnitude > magtudeCol)
    //    {
    //        gameObject.SetActive(false);
    //        Instantiate(brokenObject, transform.position, transform.rotation);
    //        brokenObject.localScale = transform.localScale;
    //        var explosionPos = transform.position;
    //        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

    //        foreach (var hit in colliders)
    //        {
    //            if(hit.attachedRigidbody)
    //            {
    //                hit.attachedRigidbody.AddExplosionForce(power * collision.relativeVelocity.magnitude, explosionPos, radius, upwards);
    //            }
    //        }
    //    }
    //}
}
