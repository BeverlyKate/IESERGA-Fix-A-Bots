using UnityEngine;

public class WiringGame : MonoBehaviour
{
    public GameObject LeftWire;
    public GameObject RightWire;
    public GameObject Correct1;
    public GameObject Correct2;
    public GameObject targetObject;

    public GameObject Red;
    public GameObject Blue;
    public GameObject Yellow;

    public GameObject Red1;
    public GameObject Blue1;
    public GameObject Yellow1;

    public Camera raycastCamera;
    public float offsetY = 5f;
    private bool isDone = false;

    private string leftWireColor = "none";
    private string rightWireColor = "none";
    private bool patternMatched = false;

    void Start()
    {
        Correct1.GetComponent<MeshRenderer>().enabled = false;
        Correct2.GetComponent<MeshRenderer>().enabled = false;

        if (raycastCamera == null)
        {
            raycastCamera = Camera.main;
        }
    }

    void Update()
    {
        Vector3 inputPos = GetInputPosition();

        if (IsInputPressed())
        {
            Ray ray = raycastCamera.ScreenPointToRay(inputPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.gameObject.name)
                {
                    case "Red":
                        SetLeftWireColor("red");
                        break;
                    case "Blue":
                        SetLeftWireColor("blue");
                        break;
                    case "Yellow":
                        SetLeftWireColor("yellow");
                        break;
                    case "Red1":
                        SetRightWireColor("red");
                        break;
                    case "Blue1":
                        SetRightWireColor("blue");
                        break;
                    case "Yellow1":
                        SetRightWireColor("yellow");
                        break;
                }
            }
        }
    }

    void SetLeftWireColor(string color)
    {
        leftWireColor = color;
        UpdateWireColor(LeftWire, color);
        CheckPattern();
    }

    void SetRightWireColor(string color)
    {
        rightWireColor = color;
        UpdateWireColor(RightWire, color);
        CheckPattern();
    }

    void UpdateWireColor(GameObject wire, string color)
    {
        Color newColor = Color.white;

        switch (color)
        {
            case "red":
                newColor = Color.red;
                break;
            case "blue":
                newColor = Color.blue;
                break;
            case "yellow":
                newColor = Color.yellow;
                break;
        }

        wire.GetComponent<Renderer>().material.color = newColor;
    }

    void CheckPattern()
    {
        if ((leftWireColor == "blue" && rightWireColor == "yellow") ||
            (leftWireColor == "yellow" && rightWireColor == "blue"))
        {
            Correct1.GetComponent<MeshRenderer>().enabled = true;
            Correct2.GetComponent<MeshRenderer>().enabled = true;
            patternMatched = true;
            GetComponent<WiringTeleport>().TriggerTeleport();
        }
        else
        {
            Correct1.GetComponent<MeshRenderer>().enabled = false;
            Correct2.GetComponent<MeshRenderer>().enabled = false;
            patternMatched = false;
        }
    }

    private Vector3 GetInputPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        else
            return Input.mousePosition;
    }

    private bool IsInputPressed()
    {
        return Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }
}
