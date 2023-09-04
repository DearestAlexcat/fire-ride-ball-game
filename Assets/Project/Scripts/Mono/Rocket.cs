using Client;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public int Pivot { get; set; }
    public int SegmentEntity { get; set; }
    public int Entity { get; set; }

    float shift;
    Vector3 startPosition, temp;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void DoYoyo(StaticData staticData)
    {
        shift += staticData.yoyoShiftByYRocket * Mathf.Sin(Time.time) * staticData.yoyoSpeedRocket * Time.deltaTime;
        temp = transform.position;
        temp.y = (startPosition.y - staticData.yoyoShiftByYRocket) + shift;
        transform.position = temp;
        transform.Rotate(0, staticData.yoyoRotSpeedRocket * Time.deltaTime, 0, Space.Self);
    }
}
