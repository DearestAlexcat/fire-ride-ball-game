using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        [Header("REQUIRED PREFABS")]
        public UI UI;
        public PopUpText PlusOne;
        public PopUpText PlusTime;
        public BonusCircle BonusCircle;
        public Chunk Chunk;
        public Rocket Rocket;
        public GameObject Empty;

        [Header("CAMERA")]
        public float levelBlur = 15;
        public float hFOV = 30f;
        public float orthoSize = 10f;
        public Vector2 DefaultResolution = new Vector2(1920, 1080);
        [Range(0f, 1f)] public float WidthOrHeight = 0;
        public Vector3 cameraStartOffset;
        public Vector3 cameraOffset;
        public Vector3 cameraRotation;
        public float cameraSmoothness;
        public float cameraSmoothnessSpeed;

        [Header("CAMERA SHAKE")]
        public float shakeTime = 0.15f;
        public float shakeAmount = 1.1f;
        public float shakeSpeed = 3f;

        [Header("FOG WALL")]
        public Vector3 fogWallStartOffset;
        public Vector3 fogWallOffset;
        public Vector3 fogWallRotation;

        [Header("TRACK")]
        public int numberChunksInSegment = 10;
        public List<AnimationCurve> segmentPatterns;    
        public float curveScalerByY;
        public float distanceBetweenChunks = 10f;           
        public List<ChunkMaterials> chunkMaterials;
        public Vector2 noiseForCurvePoints;
        public int bonusCircleSpawnPosition;
        public int bonusCircleCount;

        [System.Serializable]
        public class ChunkMaterials
        {
            public Material a, b, c, d;
        }

        [Header("ROCKET")]
        public float spawnProbabilityRocket;
        public float yoyoSpeedRocket;
        public float yoyoRotSpeedRocket;
        public float yoyoShiftByYRocket;
        public Vector2 spawnIntervalRocket;
        public Vector2Int flightTimeRangeRocket;
        public int skipWaypointsInFlightRocket;
        public float pointRadiusDestination;

        [Header("OTHER")]
        public float accrualIntervalScore = 0.8f;
        public Vector2 OnePlusShift;

        [Header("LEVELS")]
        public Levels ThisLevels;
    }
}
