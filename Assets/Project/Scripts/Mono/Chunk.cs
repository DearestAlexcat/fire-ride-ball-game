using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] MeshRenderer Top;
    [SerializeField] MeshRenderer Bottom;
    [SerializeField] MeshRenderer Floor;

    Vector3 topStartPosition;
    Vector3 bottomStartPosition;

    public Vector3 origPosition;

    public bool Init { get; private set; } = false;

    public void Initialzie()
    {
        Init = true;
        topStartPosition = Top.transform.localPosition;
        bottomStartPosition = Bottom.transform.localPosition;
    }

    public void SetMaterial(Material material, Material floor)
    {
        Top.material = material;
        Bottom.material = material;
        Floor.material = floor;
    }

    public void SetDistanceBetween(float distance)
    {
        SetDistance(Top.transform, topStartPosition,  distance);
        SetDistance(Bottom.transform, bottomStartPosition, -distance);
    }

    private void SetDistance(Transform item, Vector3 startPosition, float distance)
    {
        startPosition.y += distance;
        item.position = startPosition;
    }
}
