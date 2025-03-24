using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCalls : MonoBehaviour
{
    private Camera myCamera;
    private Ray ray;
    private RaycastHit hit;

    [SerializeField] private GameObject missionPanel; // Reference to the mission panel
    [SerializeField] private Text missionText; // Reference to the text component

    void Start()
    {
        if (missionPanel != null)
            missionPanel.SetActive(false); // Hide mission panel initially
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Use GetMouseButtonDown for a single tap
        {
            HandleRayCast(Input.mousePosition);
        }
    }

    void HandleRayCast(Vector3 myInput)
    {
        ray = myCamera.ScreenPointToRay(myInput);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("You are touching an object! That object is: " + hit.transform.name);

            if (hit.transform.CompareTag("TV")) // Check if the touched object is the TV
            {
                ShowMission("Your mission: Find the missing remote!"); // Display a mission
            }
        }
    }

    void ShowMission(string mission)
    {
        if (missionPanel != null && missionText != null)
        {
            missionText.text = mission;
            missionPanel.SetActive(true);
        }
    }
}