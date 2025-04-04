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

    public ClockManager clockManager;
    public GameObject missionPanel; 
    public TMP_Text missionText;
    public int numberOfMission;
    public Material lightUpMaterial;
    public Material dimMaterial;
    public GameObject tvScreen;
    private int currentTask;

    private bool missionActive = false;  
    private bool missionCompleted = false;
    private bool currentOngoing = false;
    private bool isTriggered = false;
    private Material originalMaterial;

    void Start()
    {
        myCamera = Camera.main;
        missionPanel.SetActive(false);
        tvScreen.GetComponent<Renderer>().material = lightUpMaterial;

        currentTask = 0;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                HandleRayCast(Input.GetTouch(0).position);
            }
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

        if (isTriggered == false)
        {
            isTriggered = true;
        }
        tvScreen.GetComponent<Renderer>().material = dimMaterial;
    }

    // Call this method when the mission is completed
    public void CompleteMission()
    {
        missionCompleted = true;
        missionPanel.SetActive(false);
        Debug.Log("Mission Completed!");
        currentTask++;

        if (currentTask == numberOfMission)
        {
            clockManager.EndWorkTime();
        }
        else
        {
            tvScreen.GetComponent<Renderer>().material = lightUpMaterial;
        }
    }
}