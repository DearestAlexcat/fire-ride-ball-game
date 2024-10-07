using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] MeshRenderer Top;
    [SerializeField] MeshRenderer Bottom;
    [SerializeField] MeshRenderer Floor;

    public void SetMaterial(Material column, Material floor)
    {
        Top.material = column;
        Bottom.material = column;
        Floor.material = floor;
    }
}
