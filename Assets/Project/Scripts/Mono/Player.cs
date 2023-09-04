using Client;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] Rigidbody thisRigidBody;
    [SerializeField] float ropeAngle;
    [SerializeField] float rocketSpeed = 2f;
    [SerializeField] float rocketRotateSpeed = 2f;
    [Space]
    [SerializeField] GameObject rocket;
    [SerializeField] GameObject warpEffect;
    [Space]
    [SerializeField] float angularSpeed = 2f;
    [SerializeField] float angularSpeedForSwinging = 20f;
    [SerializeField] float angularDragForSwinging = 0.2f;

    [SerializeField] int numberPoints = 20;
    [SerializeField] float radiusDestination = 0.2f;
    [SerializeField] int trajectoryPivotIncrement = 1;

#if UNITY_EDITOR
    public bool DebugMode = true;
    List<GameObject> trajectoryObj = new List<GameObject>();
#endif

    public Rigidbody ThisRigidBody => thisRigidBody;
    public float RocketSpeed => rocketSpeed;
    public float RocketRotateSpeed => rocketRotateSpeed;

    int rocketEntity;
    int layerMask;
    float radius;
    float angle;
    float aVelocity;
    float aAcceleration;

    List<Vector3> trajectory = new List<Vector3>();
    //int trajectoryPivot;
    float coef;

    public bool IsRocketActive => rocket.activeSelf;

    public int RocketEntity
    {
        get => rocketEntity;
        set
        {
            if(!IsRocketActive)
            {
                rocketEntity = value;
            }
        }
    }

    private void Start()
    {
        layerMask = LayerMask.GetMask("ChunkTop");
    }

    public void CreateTrajectory()
    {
        AngleInit();
        CreatePoint();
    }

    void AngleInit()
    {
        var diffVector = (line.GetPosition(1) - transform.position).normalized;
        angle = 180f - Vector3.Angle(diffVector, Vector3.up);
    }

    void CreatePoint()
    {
        float desiredAngle;
        float shiftedAngle = 360f / numberPoints;

        for (int i = 0; i < numberPoints; i++)
        {
            desiredAngle = angle + shiftedAngle * i;
            Vector3 direction = Quaternion.AngleAxis(-desiredAngle, Vector3.right) * Vector3.up;
            trajectory.Add(line.GetPosition(1) + direction * radius);

#if UNITY_EDITOR
            if (DebugMode)
            {
                trajectoryObj.Add(Object.Instantiate(Service<StaticData>.Get().Empty, trajectory[trajectory.Count - 1], Quaternion.identity));
            }
#endif  
        }
    }

    public void ClearTrajectory()
    {
        //trajectoryPivot = 0;
        trajectory = new List<Vector3>();  

#if UNITY_EDITOR
        if (DebugMode)
        {
            for (int i = 0; i < trajectoryObj.Count; i++)
            {
                Object.Destroy(trajectoryObj[i]);
            }

            trajectoryObj = new List<GameObject>();
        }
#endif
    }

    public void SetActiveRocket(bool value)
    {
        thisRigidBody.isKinematic = false;
        rocket.SetActive(value);
        warpEffect.SetActive(value);
    }

#if UNITY_EDITOR
    public void DrawDir()
    {
        Debug.DrawRay(thisRigidBody.position, Vector3.right, Color.yellow, 100);
        Debug.DrawRay(thisRigidBody.position, thisRigidBody.transform.up, Color.green, 100);
        Debug.DrawRay(thisRigidBody.position, thisRigidBody.transform.forward, Color.red, 100);
    }
#endif

    public void UpdateSourceRope()
    {
        if(line.positionCount > 0)
        {
            line.SetPosition(0, thisRigidBody.position);
        }
    }

    public void SetActiveKinematic(bool value)
    {
        ClearRope();
        thisRigidBody.isKinematic = value;
    }

    public async UniTask SetVelocityZero()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.08f), ignoreTimeScale: false);
        thisRigidBody.velocity = Vector3.zero;
    }


    public void GetRopeTarget()
    {
        if(Physics.Raycast(thisRigidBody.position, Vector3.Lerp(Vector3.up, Vector3.forward, ropeAngle), out RaycastHit hit, 100f, layerMask))
        {
            coef = 0f;

            line.positionCount = 2;
            line.SetPosition(1, hit.point);
            radius = (line.GetPosition(1) - thisRigidBody.position).magnitude;

            CreateTrajectory();
        }
    }

    public void ClearRope()
    {
        thisRigidBody.isKinematic = false;
        ClearTrajectory();
        line.positionCount = 0;
    }

    //private bool PointInCircle(Vector3 a, Vector3 b, float r)
    //{
    //    return (b.x - a.x) * (b.x - a.x) + (b.z - a.z) * (b.z - a.z) <= r * r;
    //}

    //public void CircularMovement()
    //{
    //    if (line.positionCount > 0)
    //    {
    //        if (PointInCircle(trajectory[trajectoryPivot], thisRigidBody.position, radiusDestination))
    //        {
    //            trajectoryPivot = (trajectoryPivot + trajectoryPivotIncrement) % trajectory.Count;
    //        }

    //        var movement = (trajectory[trajectoryPivot] - thisRigidBody.position).normalized;
    //        //thisRigidBody.velocity = Vector3.Lerp(thisRigidBody.velocity, movement * angularSpeed, coef);

    //        Seek(movement);

    //        coef = Mathf.Clamp01(coef + Time.deltaTime * 0.5f);

    //        Quaternion newRotation = Quaternion.LookRotation(transform.forward + movement);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * angularSpeed);
    //    }
    //}

    //public void CircularMovement12()
    //{
    //    if (line.positionCount > 0)
    //    {
    //        angle += Time.deltaTime * angularSpeed;
    //        Vector3 direction = Quaternion.AngleAxis(-angle, Vector3.right) * Vector3.up;
    //        thisRigidBody.MovePosition(line.GetPosition(1) + direction * radius);
    //        Quaternion newRotation = Quaternion.LookRotation(transform.forward + line.GetPosition(1) + direction * radius);
    //        thisRigidBody.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * angularSpeed);
    //    }
    //}

    //public void CircularMovement1()
    //{
    //    if (line.positionCount > 0)
    //    {
    //        angle += Time.deltaTime * angularSpeed;                                 
    //        Vector3 direction = Quaternion.AngleAxis(-angle, Vector3.right) * Vector3.up;
    //        transform.position = line.GetPosition(1) + direction * radius;
    //    }
    //}

    public void CircularMovement()
    {
        // var magnitude = ThisRigidBody.velocity.magnitude;
        // var left = Vector2.SignedAngle(rb.velocity, diffVector) > 0;
        // if (!left) newDirection *= -1;

        if (line.positionCount > 0)
        {
            var diffVector = line.GetPosition(1) - thisRigidBody.position;
            var newDirection = Vector3.Cross(diffVector, Vector3.right).normalized;

            thisRigidBody.velocity = Vector3.Lerp(thisRigidBody.velocity, -newDirection * angularSpeed, coef);
            coef = Mathf.Clamp01(coef + Time.deltaTime);
        }
    }


    // --------------------------------------------------------------------------------------------------

    public void CreateSwingingTrajectory()
    {
        AngleInitForSwinging();
        CreatePointForSwinging();
    }

    void AngleInitForSwinging()
    {
        angle = (180f - 30f) * Mathf.Deg2Rad;
    }

    void CreatePointForSwinging()
    {
        float desiredAngle;
        float shiftedAngle = 60f / numberPoints;
       var angle = (180f - 30f) ;
        for (int i = 0; i < numberPoints; i++)
        {
            desiredAngle = angle + shiftedAngle * i;
            Vector3 direction = Quaternion.AngleAxis(-desiredAngle, Vector3.right) * Vector3.up;
            trajectory.Add(line.GetPosition(1) + direction * radius);

#if UNITY_EDITOR
            if (DebugMode)
            {
                trajectoryObj.Add(Object.Instantiate(Service<StaticData>.Get().Empty, trajectory[trajectory.Count - 1], Quaternion.identity));
            }
#endif  
        }

        transform.position = trajectory[trajectory.Count - 1];
        gameObject.GetComponent<ParticleSystem>().Play(false);
    }


    public void SwingingMovement()
    {
        if (line.positionCount > 0)
        {
            aAcceleration = -0.4f / radius * Mathf.Sin(angle) * Time.deltaTime;  
            aVelocity += aAcceleration;               
            aVelocity *= angularDragForSwinging;                   
            angle -= aVelocity * Time.deltaTime * angularSpeedForSwinging;
            Vector3 direction = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.right) * Vector3.up;
            transform.position = line.GetPosition(1) + direction * radius;
        }
    }

    public async UniTask GetRopeTargetForSwinging()
    {
        //After restarting the scene, the collision does not work immediately
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.08f), ignoreTimeScale: false);
        
        if (Physics.Raycast(thisRigidBody.position, Vector3.Lerp(Vector3.up, Vector3.forward, ropeAngle), out RaycastHit hit, 100f, layerMask))
        //Vector3 point = Vector3.Lerp(Vector3.up, Vector3.forward, ropeAngle).normalized * 15f;
        {
            line.positionCount = 2;
            line.SetPosition(1, hit.point);
            radius = (line.GetPosition(1) - thisRigidBody.position).magnitude;

            CreateSwingingTrajectory();
        }
    }

}
