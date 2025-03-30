using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class MissionCalls : MonoBehaviour
{
    private Camera myCamera;
    private Ray ray;
    private RaycastHit hit;

    public GameObject missionPanel; 
    public TMP_Text missionText;
    public int numberOfMission;
    private int currentTask;

    private bool missionActive = false;  
    private bool missionCompleted = false;
    private bool currentOngoing = false;

    void Start()
    {
        myCamera = Camera.main;
        missionPanel.SetActive(false);

        currentTask = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleRayCast(Input.mousePosition);
        }
    }

    void HandleRayCast(Vector3 myInput)
    {
        ray = myCamera.ScreenPointToRay(myInput);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("TV")) // Make sure the TV object has the tag "TV"
            {
                if (!missionCompleted) // Only show mission if not completed
                {
                    ShowMission();
                }
                else
                {
                    Debug.Log("Mission already completed!");
                }
            }
        }
    }

    void ShowMission()
    {
        missionPanel.SetActive(true);
        missionText.text = string.Format("Repair {0} of the day (Check the tables)", currentTask+1);
        missionActive = true;
        if (!currentOngoing)
        {
            FindAnyObjectByType<MissionTrigger>().StationSetUp(currentTask);
            currentOngoing = true;
        }
    }

    public void HideMission()
    {
        missionPanel.SetActive(false);
        missionActive = false;
    }

    // Call this method when the mission is completed
    public void CompleteMission()
    {
        missionCompleted = true;
        missionPanel.SetActive(false);
        Debug.Log("Mission Completed!");
        if (currentTask < numberOfMission)
        {
            currentTask++;
        }
    }
}