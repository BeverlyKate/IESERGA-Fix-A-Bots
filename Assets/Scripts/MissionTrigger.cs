using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering;

public class MissionTrigger : MonoBehaviour
{

    public Mission[] mission;

    private GameObject[] stations;
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
                if(station.gameObject.name == "Tool")
                {
                    Debug.Log("Distributing Heads");
                    locationToSpawn = station.transform.Find("Part Location").position;
                    currentSpawn = (GameObject) Instantiate(mission[currentTask].toolTask, station.transform);
                    currentSpawn.transform.position = locationToSpawn;
                }
                else if (station.gameObject.name == "Wiring")
                {
                    locationToSpawn = station.transform.Find("Location").position;
                    currentSpawn = (GameObject)Instantiate(mission[currentTask].wireTask);
                    currentSpawn.transform.localPosition = locationToSpawn;
                }
                else if (station.gameObject.name == "Paint")
                {
                    locationToSpawn = station.transform.Find("Location").position;
                    currentSpawn = (GameObject)Instantiate(mission[currentTask].paintTask, station.transform);
                    station.transform.GetComponent<PaintStation>().cubeToRotate = currentSpawn;
                    currentSpawn.transform.position = locationToSpawn;
                }
            }
        }
    }
}
