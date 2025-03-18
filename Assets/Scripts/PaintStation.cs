using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PaintStation : MonoBehaviour
{
    public GameObject cubeToRotate;
    public float rotationSpeed = 1.0f;
    public GameObject[] cylinders;
    public GameObject[] blackPanels; 
    public Button resetButton; 
    public float proximityThreshold = 0.5f; // Adjust this value to control how close the cylinder needs to be to a plate

    private Vector3[] initialPositions;
    private Color[] originalPanelColors; 
    private GameObject selectedCylinder = null;
    private float cylinderDepth = 2.0f; 

    private void Start()
    {
        // Store the original positions of cylinders
        initialPositions = new Vector3[cylinders.Length];
        for (int i = 0; i < cylinders.Length; i++)
        {
            initialPositions[i] = cylinders[i].transform.position;
        }

        // Store the original colors of black panels
        originalPanelColors = new Color[blackPanels.Length];
        for (int i = 0; i < blackPanels.Length; i++)
        {
            Renderer panelRenderer = blackPanels[i].GetComponent<Renderer>();
            if (panelRenderer != null)
            {
                originalPanelColors[i] = panelRenderer.material.color;
            }
        }

        // Set up the reset button
        if (resetButton != null)
        {
            // Remove any existing listeners to avoid duplicates
            resetButton.onClick.RemoveAllListeners();

            // Add our reset function
            resetButton.onClick.AddListener(ResetPanels);
        }
        else
        {
            Debug.LogError("Reset button is not assigned in the inspector!");
        }
    }

    private void Update()
    {
        if (cubeToRotate == null || cylinders == null) return;

        if (Input.touchCount == 1)
        {
            Touch screenTouch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(screenTouch.position);
            RaycastHit hit;

            switch (screenTouch.phase)
            {
                case TouchPhase.Began:
                    if (Physics.Raycast(ray, out hit))
                    {
                        foreach (GameObject cylinder in cylinders)
                        {
                            if (hit.transform.gameObject == cylinder)
                            {
                                selectedCylinder = cylinder;
                                break;
                            }
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (selectedCylinder != null)
                    {
                        Vector3 touchPos = screenTouch.position;
                        touchPos.z = cylinderDepth;
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPos);

                        selectedCylinder.transform.position = worldPosition;
                    }
                    else
                    {
                        cubeToRotate.transform.Rotate(0f, -screenTouch.deltaPosition.x * rotationSpeed, 0f);
                    }
                    break;

                case TouchPhase.Ended:
                    if (selectedCylinder != null)
                    {
                        FindAndColorPlateUnderCylinder(selectedCylinder);

                        // Return cylinder to original position
                        for (int i = 0; i < cylinders.Length; i++)
                        {
                            if (cylinders[i] == selectedCylinder)
                            {
                                cylinders[i].transform.position = initialPositions[i];
                                break;
                            }
                        }

                        selectedCylinder = null;
                    }
                    break;
            }
        }
    }

    private void FindAndColorPlateUnderCylinder(GameObject cylinder)
    {
       
        Vector3 cylinderPosition = cylinder.transform.position;

        
        RaycastHit[] hits = Physics.RaycastAll(
            cylinderPosition,
            Vector3.down,
            5.0f
        );

        Debug.Log("RaycastAll hit count: " + hits.Length);

        
        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Hit object: " + hit.transform.name + " | Parent: " +
                (hit.transform.parent != null ? hit.transform.parent.name : "none"));

            
            if (hit.transform.name.Contains("Plate") ||
                (hit.transform.parent != null && hit.transform.parent.name.Contains("Sequence")))
            {
                
                float distanceToPlate = Vector3.Distance(cylinderPosition, hit.point);
                Debug.Log("Distance to plate " + hit.transform.name + ": " + distanceToPlate);

                if (distanceToPlate <= proximityThreshold)
                {
                    Renderer plateRenderer = hit.transform.GetComponent<Renderer>();
                    if (plateRenderer != null && IsBlackColor(plateRenderer.material.color))
                    {
                        Color cylinderColor = cylinder.GetComponent<Renderer>().material.color;
                        plateRenderer.material.color = cylinderColor;
                        Debug.Log("Plate " + hit.transform.name + " color changed to: " + cylinderColor);
                        return; 
                    }
                }
                else
                {
                    Debug.Log("Plate " + hit.transform.name + " too far away: " + distanceToPlate);
                }
            }
        }

       
        GameObject closestPlate = null;
        float minDistance = float.MaxValue;

        foreach (GameObject panel in blackPanels)
        {
            
            float horizontalDistance = Vector2.Distance(
                new Vector2(cylinderPosition.x, cylinderPosition.z),
                new Vector2(panel.transform.position.x, panel.transform.position.z)
            );

            
            float verticalDistance = Mathf.Abs(cylinderPosition.y - panel.transform.position.y);

            
            float combinedDistance = horizontalDistance + verticalDistance;

            if (combinedDistance < minDistance)
            {
                minDistance = combinedDistance;
                closestPlate = panel;
            }
        }

        
        if (closestPlate != null && minDistance <= proximityThreshold)
        {
            Renderer plateRenderer = closestPlate.GetComponent<Renderer>();
            if (plateRenderer != null && IsBlackColor(plateRenderer.material.color))
            {
                Color cylinderColor = cylinder.GetComponent<Renderer>().material.color;
                plateRenderer.material.color = cylinderColor;
                Debug.Log("Closest plate " + closestPlate.name + " color changed to: " + cylinderColor + " (distance: " + minDistance + ")");
            }
        }
        else if (closestPlate != null)
        {
            Debug.Log("Closest plate " + closestPlate.name + " too far away: " + minDistance);
        }
    }

    private bool IsBlackColor(Color color)
    {
        return color.r < 0.1f && color.g < 0.1f && color.b < 0.1f;
    }

    public void ResetPanels()
    {
        Debug.Log("ResetPanels function called!");

        if (blackPanels == null || blackPanels.Length == 0)
        {
            Debug.LogError("Black panels array is null or empty!");
            return;
        }

        if (originalPanelColors == null || originalPanelColors.Length == 0)
        {
            Debug.LogError("Original panel colors array is null or empty!");
            return;
        }

        for (int i = 0; i < blackPanels.Length; i++)
        {
            if (blackPanels[i] == null)
            {
                Debug.LogWarning("Black panel at index " + i + " is null!");
                continue;
            }

            Renderer panelRenderer = blackPanels[i].GetComponent<Renderer>();
            if (panelRenderer != null)
            {
                if (i < originalPanelColors.Length)
                {
                    panelRenderer.material.color = originalPanelColors[i]; // Reset to the original panels
                }
                else
                {
                    panelRenderer.material.color = Color.black; //Assigns black instead if not found
                }
            }
            else
            {
                Debug.LogWarning("No renderer on panel " + blackPanels[i].name);
            }
        }

        Debug.Log("Panels reset to original colors completed.");
    }
}
