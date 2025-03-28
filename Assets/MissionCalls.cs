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
    private SerializeField[] task;
    private GameObject[] tasklist;

    public GameObject missionPanel; 
    public TMP_Text missionText;       
    public Button okButton;         

    private bool missionActive = false;  
    private bool missionCompleted = false; 

    void Start()
    {
        myCamera = Camera.main;
        missionPanel.SetActive(false); 

     
        okButton.onClick.AddListener(HideMission);
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
        missionText.text = "Your task: You shouldn't have pressed the TV...";
        missionActive = true;
    }

    void HideMission()
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
    }
}