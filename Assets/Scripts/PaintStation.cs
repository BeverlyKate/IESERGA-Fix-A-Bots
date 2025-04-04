using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PaintStation : MonoBehaviour
{
    public GameObject cubeToRotate;
    public float rotationSpeed = 1.0f;
    public GameObject[] cylinders;
    public GameObject[] blackPanels;
    public Button resetButton;
    public float proximityThreshold = 0.001f;
    public Camera cam;
    public TextMeshPro statusText;
    public GameObject teleportTo;
    public Transform paintPos;
    //public List <GameObject> blackPanels = new List<GameObject>();

    private Vector3[] initialPositions;
    private Color[] originalPanelColors;
    private GameObject selectedCylinder = null;
    private float cylinderDepth = 2.0f;

    private Dictionary<string, List<Color>> targetColors = new Dictionary<string, List<Color>>();

    Ray GetRay() => cam.ScreenPointToRay(Input.GetTouch(0).position);

    void Start()
    {
        resetButton.gameObject.SetActive(false);
        initialPositions = new Vector3[cylinders.Length];
        for (int i = 0; i < cylinders.Length; i++)
        {
            initialPositions[i] = cylinders[i].transform.position;
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

        if (statusText != null)
        {
            statusText.text = "Not done";
        }


        // Define target colors for different cubeToRotate names
        targetColors["PaintLevel1(Clone)"] = new List<Color> { Color.blue, Color.yellow };
        targetColors["PaintLevel2(Clone)"] = new List<Color> { Color.blue, Color.green, new Color(0.5842372f, 0.2320754f, 1.0f)};
        targetColors["PaintLevel3(Clone)"] = new List<Color> { Color.yellow, Color.green, Color.blue, Color.red};

        
    }

    void Update()
    {

        originalPanelColors = new Color[blackPanels.Length];
        for (int i = 0; i < blackPanels.Length; i++)
        {
            Renderer panelRenderer = blackPanels[i].GetComponent<Renderer>();
            if (panelRenderer != null)
            {
                originalPanelColors[i] = Color.black; // Ensures true black for reset
            }
        }

        if (cubeToRotate == null || cylinders == null) return;

        if (Input.touchCount == 1)
        {
            Touch screenTouch = Input.GetTouch(0);
            Ray ray = cam.ScreenPointToRay(screenTouch.position);
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
                        Vector3 worldPosition = cam.ScreenToWorldPoint(touchPos);

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

                        CheckLevelCompletion();
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

    public void CheckLevelCompletion()
    {
        string objectName = cubeToRotate.name;
        if (!targetColors.ContainsKey(objectName))
        {
            Debug.LogError("No target colors assigned for " + objectName);
            return;
        }

        List<Color> requiredColors = targetColors[objectName];
        List<Color> currentColors = new List<Color>();

        foreach (GameObject panel in blackPanels)
        {
            Renderer panelRenderer = panel.GetComponent<Renderer>();
            if (panelRenderer != null)
            {
                currentColors.Add(panelRenderer.material.color);
            }
        }

        if (AreListsEqual(requiredColors, currentColors))
        {
            Debug.Log("Level Completed!");
            if (statusText != null)
            {
                statusText.text = "Done!";
            }
            /*
            foreach (GameObject cylinder in cylinders)
            {
                cylinder.SetActive(false);  // Hide cylinders
            }
            */
            teleportTo.GetComponent<AssemblyStation>().AddPart(cubeToRotate);
            cubeToRotate.transform.position = paintPos.position;
            cubeToRotate.transform.parent = teleportTo.transform;
        }

        // If all required panels have the correct color, show level complete panel
        //LevelComplete();
    }
    /*
    public void LevelComplete()
    {
        
    }
    */

    private bool AreListsEqual(List<Color> list1, List<Color> list2)
    {
        if (list1.Count != list2.Count) return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (!ApproximatelyEqual(list1[i], list2[i]))
            {
                return false;
            }
        }

        return true;
    }

    private bool ApproximatelyEqual(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.1f &&
               Mathf.Abs(a.g - b.g) < 0.1f &&
               Mathf.Abs(a.b - b.b) < 0.1f;
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