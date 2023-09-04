using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public struct SpawnSegmentRequest
    {
        public int NumberChunks;      
    }
    
    struct PlayerInputComponent
    {
        public bool InputPressed;
        public bool InputHeld;
        public bool InputReleased;
    }
   
    struct PopUpRequest
    {
        public string TextUP;
        public Vector3 SpawnPosition;
        public Quaternion SpawnRotation;
        public Transform Parent;
        public PopUpText UpText;
    }
   
    struct CurvePathComponent
    {
        public List<Vector3> CurvePath;
    }

    struct SegmentPathComponent
    {
        public List<Vector3> SegmentPath;
    }
 
    struct CameraShakeReguest { }

    struct FallingRequest { }
    
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
        public int Pivot;
    }

    struct AccrualScoreComponent
    {
        public float AccrualIntervalScore;
    }

    struct CurrentPivots
    {
        public int SegmentMaterialPivot;
        public int SegmentCurvePivot;
    }

    public struct Component<T>
    {
        public T Value;
    }
}