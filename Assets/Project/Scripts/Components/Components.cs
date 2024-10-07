using UnityEngine;

namespace Client
{
    public struct SpawnSegmentRequest { }

    public struct InputHeld { }

    public struct InputReleased { }
    
    public struct InputPressed { }

    public struct CacheKey
    {
        public int Key; // used for association with pool objects
    }

    struct PopUpRequest
    {
        public string Value;
        public MesssageType Type;
    }

    struct FXRequest
    {
        public Vector3 position;
    }

    struct Falling 
    {
        public Transform transform;
        public Vector3 velocity;
        public float mass;
    }

    struct Group 
    {
        public int Rank;
    }

    struct InGroup 
    {
        public int GroupIndex;
    }

    struct CameraShakeReguest { }

    struct LaunchingRopeReguest 
    {
        public float ProgressReachingRope;
    }

    struct LoopingMovement
    {
        public Transform transform;
        public Vector3 startPosition;
        public bool Rotate;
        public bool Movement;
    }

    public struct ExecutionDelay
    {
        public float time;
        public System.Action action;
    }

    struct ChangeStateEvent
    {
        public GameState NewGameState;
    }

    struct RocketMoveComponent
    {
        public float Time;
        public int Pointer;
    }

    struct DelayComponent
    {
        public float Delay;
    }

    public struct Component<T>
    {
        public T Value;
    }
}