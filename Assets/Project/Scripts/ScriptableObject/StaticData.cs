using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        [Header("REQUIRED PREFABS")]
        public UI UI;
        public PopUpText PopupText;
        public BonusCircle BonusCircle;
        public Chunk Chunk;
        public Rocket Rocket;
        public Transform RocketFalling;

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
        public int minNumSegments = 3;
        public int numberChunksInSegment = 10;
        public List<AnimationCurve> segmentPatterns;    
        public float curveScalerByY;
        public List<ChunkMaterials> chunkMaterials;
        public Vector2 noiseForCurvePoints;
        public int bonusCircleCount;
        [Range(0f, 1f)] public float spawnNextSegmentPosition = 0.5f;

        [System.Serializable]
        public class ChunkMaterials
        {
            public Material columnA, columnB, floor, fog; // дать нормальные имена
        }

        [Header("ROCKET")]
        [Range(0f, 1f)] public float rocketSpawnProbability;
        [Range(0f, 1f)] public float rocketSpawnPoint1;
        [Range(0f, 1f)] public float rocketSpawnPoint2;
        public Vector2Int flightTimeRangeRocket;
        public int skipWaypointsInFlightRocket;
        public float pointRadiusDestination;
        public float rocketSpeed = 35f;
        public float rocketRotateSpeed = 2f;

        [Header("LOOPINGMOVE")]
        public float loopingRotateSpeed = 75f;
        public float loopingShiftByY = 5f;

        [Header("OTHER")]
        public float standartDelay = 1f;
        public Vector2 OnePlusShift;
        public float launchingRopeDuration = 0.5f;
        public GameObject explosionEffect;
        public float playerMass = 1f;

#if UNITY_EDITOR
        [Header("UNITY_EDITOR")]
        public float slowmo = 0.1f;
        public GameObject Empty;
#endif
        [Header("LEVELS")]
        public Levels ThisLevels;
    }
}
