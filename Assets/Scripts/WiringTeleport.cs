using UnityEngine;

public class WiringTeleport : MonoBehaviour
{
    public Transform wiringPos;

    public GameObject teleportTo;

    public void TriggerTeleport()
    {
        transform.parent = teleportTo.transform;
        transform.position = wiringPos.position;
        teleportTo.GetComponent<AssemblyStation>().AddPart(gameObject);
    }
}
