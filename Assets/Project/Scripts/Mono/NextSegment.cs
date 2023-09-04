using UnityEngine;

public class NextSegment : MonoBehaviour
{
    [SerializeField] MeshRenderer mat;

    private float CurrentOffset;
    [SerializeField] float Speed;

    [SerializeField] ParticleSystem ps;

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
