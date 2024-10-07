using UnityEngine;

public class GateNextSegment : MonoBehaviour
{
    [SerializeField] MeshRenderer mat;
    [SerializeField] float Speed;
    [SerializeField] ParticleSystem ps;

    private float CurrentOffset;

    [HideInInspector]
    public int segmentMatIndex;

    public void PlayFX()
    {
        ps.Play();
    }

    public void Update()
    {
        CurrentOffset += Time.deltaTime * Speed;
        mat.sharedMaterial.SetTextureOffset("_BaseMap", new Vector2(0, CurrentOffset));
    }
}
