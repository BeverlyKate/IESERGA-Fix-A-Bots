using UnityEngine;

public class WiringTeleport : MonoBehaviour
{
    public Transform wiringPos;

    public GameObject teleportTo;

    void Update()
    {
        if(GetComponentInChildren<WireStation>() != null)
        {
            if (GetComponentInChildren<WireStation>().GameIsDone())
            {
                transform.parent = teleportTo.transform;
                transform.position = wiringPos.position;
            }
        }
        else if(GetComponent<WiringGame>() != null)
        {
            if (GetComponentInChildren<WiringGame>().GameIsDone())
            {
                transform.parent = teleportTo.transform;
                transform.position = wiringPos.position;
            }
        }
        else if (GetComponent<WiringGame2>() != null)
        {
            if (GetComponentInChildren<WiringGame2>().GameIsDone())
            {
                transform.parent = teleportTo.transform;
                transform.position = wiringPos.position;
            }
        }
        else if (GetComponent<WiringGame3>() != null)
        {
            if (GetComponentInChildren<WiringGame3>().GameIsDone())
            {
                transform.parent = teleportTo.transform;
                transform.position = wiringPos.position;
            }
        }

    }
}
