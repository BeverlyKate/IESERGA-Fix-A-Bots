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
    public float proximityThreshold = 0.5f;

    private Vector3[] initialPositions;
    private Color[] originalPanelColors;
    private GameObject selectedCylinder = null;
    private float cylinderDepth = 2.0f;

    private void Start()
    {
        initialPositions = new Vector3[cylinders.Length];
        for (int i = 0; i < cylinders.Length; i++)
        {
            initialPositions[i] = cylinders[i].transform.position;
        }

        originalPanelColors = new Color[blackPanels.Length];
        for (int i = 0; i < blackPanels.Length; i++)
        {
            Renderer panelRenderer = blackPanels[i].GetComponent<Renderer>();
            if (panelRenderer != null)
            {
                originalPanelColors[i] = Color.black; // Ensures true black for reset
            }
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
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

        RaycastHit[] hits = Physics.RaycastAll(cylinderPosition, Vector3.down, 5.0f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.name.Contains("Plate") ||
                (hit.transform.parent != null && hit.transform.parent.name.Contains("Sequence")))
            {
                float distanceToPlate = Vector3.Distance(cylinderPosition, hit.point);

                if (distanceToPlate <= proximityThreshold)
                {
                    Renderer plateRenderer = hit.transform.GetComponent<Renderer>();
                    if (plateRenderer != null)
                    {
                        Color existingColor = plateRenderer.material.color;
                        Color cylinderColor = cylinder.GetComponent<Renderer>().material.color;

                        if (IsBlackColor(existingColor))
                        {
                            plateRenderer.material.color = cylinderColor;
                        }
                        else
                        {
                            plateRenderer.material.color = MixColors(existingColor, cylinderColor);
                        }
                        return;
                    }
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
            if (plateRenderer != null)
            {
                Color existingColor = plateRenderer.material.color;
                Color cylinderColor = cylinder.GetComponent<Renderer>().material.color;

                if (IsBlackColor(existingColor))
                {
                    plateRenderer.material.color = cylinderColor;
                }
                else
                {
                    plateRenderer.material.color = MixColors(existingColor, cylinderColor);
                }
            }
        }
    }

    private bool IsBlackColor(Color color)
    {
        return color.r < 0.1f && color.g < 0.1f && color.b < 0.1f;
    }

    private Color MixColors(Color existingColor, Color cylinderColor)
    {
        /*
         * Custom color mixing logic to handle cases where the RGB averaging doesn't match
         * expected visual results when mixing pigment colors
         */

        // Yellow + Blue = Green
        if ((IsBlueColor(existingColor) && IsYellowColor(cylinderColor)) ||
            (IsYellowColor(existingColor) && IsBlueColor(cylinderColor)))
        {
            return Color.green;
        }

        // Red + Yellow = Orange
        if ((IsRedColor(existingColor) && IsYellowColor(cylinderColor)) ||
            (IsYellowColor(existingColor) && IsRedColor(cylinderColor)))
        {
            return new Color(1.0f, 0.5019608f, 0.0f); // Bright orange
        }

        // Red + Blue = Purple
        if ((IsRedColor(existingColor) && IsBlueColor(cylinderColor)) ||
            (IsBlueColor(existingColor) && IsRedColor(cylinderColor)))
        {
            return new Color(0.5842372f, 0.2320754f, 1.0f); // Purple
        }

        // Default to averaging for other color combinations
        return new Color(
            (existingColor.r + cylinderColor.r) / 2,
            (existingColor.g + cylinderColor.g) / 2,
            (existingColor.b + cylinderColor.b) / 2
        );
    }

    // Helper methods to identify specific colors
    private bool IsBlueColor(Color color)
    {
        return color.b > 0.5f && color.r < 0.3f && color.g < 0.3f;
    }

    private bool IsYellowColor(Color color)
    {
        return color.r > 0.5f && color.g > 0.5f && color.b < 0.3f;
    }

    private bool IsRedColor(Color color)
    {
        return color.r > 0.5f && color.g < 0.3f && color.b < 0.3f;
    }

    public void ResetPanels()
    {
        if (blackPanels == null || blackPanels.Length == 0)
        {
            Debug.LogError("Black panels array is null or empty!");
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
                panelRenderer.material.color = Color.black;
            }
            else
            {
                Debug.LogWarning("No renderer on panel " + blackPanels[i].name);
            }
        }

        Debug.Log("Panels reset to black.");
    }
}


/*
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
*/