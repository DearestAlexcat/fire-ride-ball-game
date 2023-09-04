using UnityEngine;
using UnityEngine.Rendering;

namespace Client
{ 
    public class SceneContext : MonoBehaviour
    {
        [field: SerializeField] public Player PlayerView { get; private set; }
        [field: SerializeField] public Transform LevelRoot { get; private set; }
        [field: SerializeField] public VolumeProfile BlurProfile { get; private set; }
        [field: SerializeField] public Transform SpawnNextSegment { get; private set; }
        [field: SerializeField] public NextSegment NextSegment { get; private set; }
        [field: SerializeField] public MeshRenderer FogWall { get; private set; }
        [field: SerializeField] public Camera NoPostCamera { get; private set; }

    }
}