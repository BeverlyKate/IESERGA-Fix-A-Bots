using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering;

public class MissionTrigger : MonoBehaviour
{

    public Mission[] mission;

    public Transform targetTelepotPosition;

    public Transform[] teleportPos;

    private GameObject[] stations;

    private Dictionary<string, string[]> panelAssignments = new Dictionary<string, string[]>
    {
        { "PaintLevel1(Clone)", new string[] { "Plate 4", "Plate 11" } },
        { "PaintLevel2(Clone)", new string[] { "Plate 5", "Plate 9", "Plate 12" } },
        { "PaintLevel3(Clone)", new string[] { "Plate 6", "Plate 9", "Plate 10", "Plate 11" } }
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stations = GameObject.FindGameObjectsWithTag("Station");
    }

    // Update is called once per frame
    public void StationSetUp(int currentTask)
    {
        Vector3 locationToSpawn;
        GameObject currentSpawn;
        foreach (GameObject station in stations)
        {
            if (station != null)
            {
                if (station.gameObject.name == "Tool")
                {
                    Debug.Log("Distributing Heads");
                    locationToSpawn = station.transform.Find("Part Location").position;
                    currentSpawn = (GameObject)Instantiate(mission[currentTask].toolTask, station.transform);
                    currentSpawn.transform.position = locationToSpawn;
                    station.GetComponent<ToolStation>().roboPart = currentSpawn;
                    station.GetComponent<ToolStation>().teleportTo = targetTelepotPosition.gameObject;
                    station.GetComponent<ToolStation>().toolPos = teleportPos[0];
                }
                else if (station.gameObject.name == "Wiring")
                {
                    currentSpawn = (GameObject)Instantiate(mission[currentTask].wireTask);
                    if (currentSpawn.GetComponentInChildren<WireStation>() != null || currentSpawn.GetComponentInChildren<WireStation3>() != null)
                    {
                        Debug.Log("Original Wire Game!");
                        locationToSpawn = station.transform.Find("Location").position;
                    }
                    else
                    {
                        locationToSpawn = station.transform.Find("Location1").position;
                    }
                    currentSpawn.transform.localPosition = locationToSpawn;
                    if(currentSpawn.GetComponentInChildren<WireStation>() != null)
                    {
                        currentSpawn.GetComponentInChildren<WireStation>().mainCamera = station.GetComponent<Wiring>().camera;
                        currentSpawn.GetComponentInChildren<WireStation>().targetObject = targetTelepotPosition;
                    }
                    else if (currentSpawn.GetComponentInChildren<WireStation3>() != null)
                    {
                        currentSpawn.GetComponentInChildren<WireStation3>().mainCamera = station.GetComponent<Wiring>().camera;
                        //ws3.GetComponentInChildren<WireStation3>().targetObject = targetTelepotPosition;
                    }

                    if(currentSpawn.GetComponent<WiringGame>() != null)
                    {
                        currentSpawn.GetComponent<WiringGame>().raycastCamera = station.GetComponent<Wiring>().camera;
                        currentSpawn.GetComponent<WiringGame>().targetObject = targetTelepotPosition.gameObject;
                    }
                    else if (currentSpawn.GetComponent<WiringGame2>() != null)
                    {
                        currentSpawn.GetComponent<WiringGame2>().raycastCamera = station.GetComponent<Wiring>().camera;
                        currentSpawn.GetComponent<WiringGame2>().targetObject = targetTelepotPosition.gameObject;
                    }
                    else if (currentSpawn.GetComponent<WiringGame3>() != null)
                    {
                        currentSpawn.GetComponent<WiringGame3>().raycastCamera = station.GetComponent<Wiring>().camera;
                        currentSpawn.GetComponent<WiringGame3>().targetObject = targetTelepotPosition.gameObject;
                    }

                    currentSpawn.GetComponent<WiringTeleport>().teleportTo = targetTelepotPosition.gameObject;
                    currentSpawn.GetComponent<WiringTeleport>().wiringPos = teleportPos[1];
                    currentSpawn.GetComponent<WiringTeleport>().indicator = station.transform.Find("Indicator").gameObject;
                    station.GetComponent<Wiring>().wiringGame = currentSpawn;
                }
                else if (station.gameObject.name == "Paint")
                {
                    locationToSpawn = station.transform.Find("Location").position;
                    currentSpawn = (GameObject)Instantiate(mission[currentTask].paintTask, station.transform);

                    station.transform.GetComponent<PaintStation>().cubeToRotate = currentSpawn;
                    currentSpawn.transform.position = locationToSpawn;

                    if (station.transform.GetComponent<PaintStation>().cubeToRotate.name == "PaintLevel1(Clone)")
                    {
                        GameObject plateOne4 = currentSpawn.transform.Find("Robot Body 1/Sequence/Plate 4")?.gameObject;
                        GameObject plateOne11 = currentSpawn.transform.Find("Robot Body 1/Sequence/Plate 11")?.gameObject;


                        station.transform.GetComponent<PaintStation>().blackPanels = new GameObject[]{ plateOne4, plateOne11}; 
                    }

                    if (station.transform.GetComponent<PaintStation>().cubeToRotate.name == "PaintLevel2(Clone)")
                    {
                        GameObject plateTwo5 = currentSpawn.transform.Find("Robot Body 2/Sequence/Plate 5")?.gameObject;
                        GameObject plateTwo9 = currentSpawn.transform.Find("Robot Body 2/Sequence/Plate 9")?.gameObject;
                        GameObject plateTwo12 = currentSpawn.transform.Find("Robot Body 2/Sequence/Plate 12")?.gameObject;


                        station.transform.GetComponent<PaintStation>().blackPanels = new GameObject[] { plateTwo5, plateTwo9, plateTwo12 };
                    }

                    if (station.transform.GetComponent<PaintStation>().cubeToRotate.name == "PaintLevel3(Clone)")
                    {
                        GameObject plateThree6 = currentSpawn.transform.Find("Robot Body 3/Sequence/Plate 6")?.gameObject;
                        GameObject plateThree9 = currentSpawn.transform.Find("Robot Body 3/Sequence/Plate 9")?.gameObject;
                        GameObject plateThree10 = currentSpawn.transform.Find("Robot Body 3/Sequence/Plate 10")?.gameObject;
                        GameObject plateThree11 = currentSpawn.transform.Find("Robot Body 3/Sequence/Plate 11")?.gameObject;


                        station.transform.GetComponent<PaintStation>().blackPanels = new GameObject[] { plateThree6, plateThree9, plateThree10, plateThree11 };
                    }
                }

                station.transform.Find("Indicator").gameObject.SetActive(true);
            }
        }
    }
}
