using Client;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] GameObject orb;
    [SerializeField] GameObject rocket;
    [SerializeField] GameObject warpEffect;
    [SerializeField] PlayerData data;

#if UNITY_EDITOR
    List<GameObject> trajectoryObj = new List<GameObject>();
    List<Vector3> trajectory = new List<Vector3>();
#endif

    public EcsPackedEntityWithWorld fallingData;

    float angle;

    float angularSpeed; // Angular velocity in rad/s
    float radius;

    int rocketEntity;
    int layerMask;

    float fVelocity;
    float acceleration;

    Vector3 ropeTarget;

    //float progressReachingRope;
    //CancellationTokenSource cts;
    //bool isDisposed = false;

    float gravitationalAcceleration = Physics.gravity.magnitude;

    public float LaunchingRopeDuration => data.LaunchingRopeDuration;

    public GameObject Rocket => rocket;

    public bool IsRocketActive => rocket.activeSelf;

    public int RocketEntity
    {
        get => rocketEntity;
        set
        {
            if (!IsRocketActive)
            {
                rocketEntity = value;
            }
        }
    }

    private void Start()
    {
        layerMask = LayerMask.GetMask("Chunk");
    }

    public void SetActiveOrb(bool value)
    {
        orb.SetActive(value);
        orb.GetComponent<ParticleSystem>().Play();
    }

    public void SetActiveWarpEffect(bool value)
    {
        warpEffect.SetActive(value);
    }

    public void SetActiveRocket(bool value)
    {
        rocket.SetActive(value);
    }

    //private void EndTaskLaunchingRopeToTarget()
    //{
    //    if (!isDisposed && cts != null && !cts.Token.IsCancellationRequested)
    //    {
    //        isDisposed = true;
    //        cts.Cancel();
    //        cts.Dispose();
    //    }
    //}

    //private void RunTaskLaunchingRopeToTarget()
    //{
    //    isDisposed = false;
    //    cts = new CancellationTokenSource();
    //    var token = cts.Token;
    //    LaunchingRopeToTarget(token).Forget();
    //}

    //async UniTask LaunchingRopeToTarget(CancellationToken cancellationToken)
    //{
    //    progressReachingRope = 0f;

    //    while(progressReachingRope < 1f)
    //    {
    //        if (cancellationToken.IsCancellationRequested)
    //        {
    //            return;
    //        }

    //        line.SetPosition(1, Vector3.Lerp(line.GetPosition(1), ropeTarget, progressReachingRope));
    //        progressReachingRope += Time.deltaTime / data.LaunchingRopeDuration;

    //        if(progressReachingRope >= 1f)
    //        {
    //            radius = (line.GetPosition(1) - transform.position).magnitude;
    //            angularSpeed = data.LinearSpeed / radius;
    //            angle = Vector3.Angle(transform.position - line.GetPosition(1), Vector3.up);
    //        }

    //        await UniTask.NextFrame();
    //    }
    //}

    public bool GetRopeTarget()
    {
        if (Physics.Raycast(transform.position, Vector3.Lerp(Vector3.up, Vector3.forward, data.RopeAngle), out RaycastHit hit, 100f, layerMask))
        {
            line.positionCount = 2;
            ropeTarget = hit.point;
            line.SetPosition(1, transform.position);
            return true;
        }

        return false;
    }

    public void UpdateSourceRope()
    {
        if (line.positionCount > 0)
        {
            var dir = (line.GetPosition(1) - transform.position).normalized;
            var size = transform.localScale.x * 0.5f;
            line.SetPosition(0, transform.position + dir * size);
        }
    }

    public void ClearRope()
    {
        line.positionCount = 0;
    }

    public Vector3 GetVelocity()
    {
        if (fallingData.Unpack(out EcsWorld world, out int entity))
        {
            return world.GetEntityRef<Falling>(entity).velocity;
        }
        else
        {
            Debug.LogError("Falling data unpack ERROR.");
        }

        return Vector3.zero;
    }

    public void SetVelocity(Vector3? velocity = null)
    {
        if (fallingData.Unpack(out EcsWorld world, out int entity))
        {
            ref var item = ref world.GetEntityRef<Falling>(entity);
            item.velocity = transform.forward * data.LinearSpeed;

#if UNITY_EDITOR
            Debug.Log($"<color='yellow'>Velocity: {item.velocity}</color>");
#endif

            if (velocity.HasValue)
            {
                item.velocity = velocity.Value;
            }
        }
        else
        {
            Debug.LogError("Falling data unpack ERROR.");
        }
    }

    //public void ObjectFall()
    //{
    //    // Apply gravity to the object
    //    velocity += Vector3.down * gravitationalAcceleration * Time.deltaTime;
    //    // Update object position based on speed
    //    transform.position += velocity * Time.deltaTime;
    //}

    //public void SetVelocity()
    //{
    //    velocity = transform.forward * data.LinearSpeed;
    //}

    //public void LineSetPosition(float progressReachingRope)
    //{
    //    line.SetPosition(0, transform.position);
    //    line.SetPosition(1, Vector3.Lerp(line.GetPosition(1), ropeTarget, progressReachingRope));
    //}

    public void LineSetPosition(float progressReachingRope)
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, Vector3.Lerp(line.GetPosition(1), ropeTarget, progressReachingRope));
    }

    public bool SetAngle()
    {
        radius = (ropeTarget - transform.position).magnitude;

        if (Mathf.Approximately(radius, 0f))
        {
#if UNITY_EDITOR
            Debug.Log($"<color='red'>SetAngle: radius equals 0.</color>");
#endif
            return false;
        }

        angularSpeed = data.LinearSpeed / radius;
        angle = Vector3.Angle(transform.position - ropeTarget, Vector3.up); // 0 - 180 - 0
        Vector3 cross = Vector3.Cross(transform.position - ropeTarget, Vector3.up);
        if (Mathf.Sign(cross.x) < 0)
            angle = 360f - angle; // 0 - 360 - 0

        return true;
    }

    float EaseInSine(float x)
    {
        return 1f - Mathf.Cos(x * Mathf.PI / 2f);
    }

    Vector3 GetNextPositionOnCircle()
    {
        angle += Time.deltaTime * angularSpeed * Mathf.Rad2Deg;
        Vector3 direction = Quaternion.AngleAxis(-angle, Vector3.right) * Vector3.up;
        return ropeTarget + direction * radius;
    }

    public void CircularMovement()
    {
        if (line.positionCount > 0)
        {
            transform.position = GetNextPositionOnCircle();
            SetVelocity(Vector3.zero);
            transform.forward = -Vector3.Cross(ropeTarget - transform.position, Vector3.right).normalized;

#if UNITY_EDITOR
                DrawDir(Vector3.up);
#endif
        }
    }

    // Other version
    /*public void CircularMovement()
    {
        // var magnitude = ThisRigidBody.velocity.magnitude;
        // var left = Vector2.SignedAngle(rb.velocity, diffVector) > 0;
        // if (!left) newDirection *= -1;

        if (line.positionCount > 0)
        {
            var diffVector = (line.GetPosition(1) - transform.position).normalized;
            var newDirection = Vector3.Cross(diffVector, Vector3.right).normalized;
            thisRigidBody.velocity = Vector3.Lerp(thisRigidBody.velocity, -newDirection * angularSpeed, coef);
            coef = Mathf.Clamp01(coef + Time.deltaTime);
        }
    } */

#if UNITY_EDITOR
    public void DrawDir(Vector3 up)
    {
        Debug.DrawRay(transform.position, transform.right, Color.yellow, 100);
        Debug.DrawRay(transform.position, up, Color.green, 100);
        Debug.DrawRay(transform.position, transform.forward, Color.red, 100);
    }
#endif

    // --------------------------------------------------------------------------------------------------

    void AngleInitForSwinging()
    {
        angle = 150f * Mathf.Deg2Rad;
    }

#if UNITY_EDITOR
    void CreatePointForSwinging()
    { 
        if (!data.DebugMode) return;

        float desiredAngle;
        float shiftedAngle = 60f / data.NumberPoints;
        var angle = 180f - 30f;

        for (int i = 0; i < data.NumberPoints; i++)
        {
            desiredAngle = angle + shiftedAngle * i;
            Vector3 direction = Quaternion.AngleAxis(-desiredAngle, Vector3.right) * Vector3.up;
            trajectory.Add(line.GetPosition(1) + direction * radius);
            trajectoryObj.Add(Object.Instantiate(Service<StaticData>.Get().Empty, trajectory[trajectory.Count - 1], Quaternion.identity));
        }

        transform.position = trajectory[trajectory.Count - 1];
    }
#endif

    public void SwingingMovement()
    {
        if (line.positionCount > 0)
        {
            acceleration = -0.4f / radius * Mathf.Sin(angle) * Time.deltaTime;
            fVelocity += acceleration;
            fVelocity *= data.AngularDragForSwinging;
            angle -= fVelocity * Time.deltaTime * data.AngularSpeedForSwinging;
            Vector3 direction = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.right) * Vector3.up;
            transform.position = line.GetPosition(1) + direction * radius;
        }
    }

    public async UniTask GetRopeTargetForSwinging()
    {
        // After restarting the scene, the collision does not work immediately
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.08f), ignoreTimeScale: false);
        
        if (Physics.Raycast(transform.position, Vector3.Lerp(Vector3.up, Vector3.forward, data.RopeAngle), out RaycastHit hit, 100f, layerMask))
        {
            line.positionCount = 2;
            line.SetPosition(1, hit.point);
            radius = (line.GetPosition(1) - transform.position).magnitude;

            AngleInitForSwinging();

#if UNITY_EDITOR
            CreatePointForSwinging();
#endif 
        }
    }
}
