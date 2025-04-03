using System;
using UnityEngine;

public class AssemblyStation : MonoBehaviour
{
    [SerializeField]
    private MissionCalls completeMission;

    private GameObject[] repairedParts = new GameObject[3];

    private int partCount = 0;

    public bool missionDone = false;

    // Update is called once per frame
    void Update()
    {
        if (partCount == 3)
        {
            completeMission.CompleteMission();

            foreach(GameObject repairedPart in repairedParts)
            {
                Destroy(transform.Find(repairedPart.name));
            }

            Array.Clear(repairedParts, 0, repairedParts.Length);

            partCount = 0;

            missionDone = true;
        }
    }

    public void AddPart(GameObject addedPart)
    {
        repairedParts[partCount] = addedPart;
        partCount++;
    }
}
