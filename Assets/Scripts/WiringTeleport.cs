using UnityEngine;

public class WiringTeleport : MonoBehaviour
{
    public Transform wiringPos;

    public GameObject teleportTo;

    void Update()
    {
        if (GetComponentInChildren<WireStation>().GameIsDone())
        {
            transform.parent = teleportTo.transform;
            transform.position = wiringPos.position;
        }
    }
}
