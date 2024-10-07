using UnityEngine;

namespace Client
{
    [CreateAssetMenu]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] float ropeAngle;
        [SerializeField] float launchingRopeDuration = 0.12f;
        [SerializeField] float linearSpeed = 16f; // Linear speed in m/s
        [SerializeField] float angularSpeedForSwinging = 20f;
        [SerializeField] float angularDragForSwinging = 0.2f;

        public float RopeAngle => ropeAngle;
        public float LaunchingRopeDuration => launchingRopeDuration;
        public float LinearSpeed => linearSpeed;
        public float AngularSpeedForSwinging => angularSpeedForSwinging;
        public float AngularDragForSwinging => angularDragForSwinging;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] int numberPoints = 20;
        [SerializeField] bool debugMode = true;
        public int NumberPoints => numberPoints;
        public bool DebugMode => debugMode;
#endif
    }
}