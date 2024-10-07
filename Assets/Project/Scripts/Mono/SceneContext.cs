using UnityEngine;

namespace Client
{ 
    public class SceneContext : MonoBehaviour
    {
        [field: SerializeField] public Player PlayerView { get; private set; }
        [field: SerializeField] public Transform SpawnNextSegment { get; private set; }
        [field: SerializeField] public GateNextSegment GateNextSegment { get; private set; }
        [field: SerializeField] public MeshRenderer FogWall { get; private set; }
        [field: SerializeField] public Camera NoPostCamera { get; private set; }
    }
}