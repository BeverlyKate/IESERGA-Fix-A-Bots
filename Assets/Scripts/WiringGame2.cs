using UnityEngine;
using TMPro;

public class WiringGame2 : MonoBehaviour
{
    public GameObject LeftWire1;
    public GameObject LeftWire2;
    public GameObject RightWire;

    public GameObject Correct1;
    public GameObject Correct2;
    public GameObject Correct3;

    public GameObject Red1;
    public GameObject Blue1;
    public GameObject Yellow1;

    public GameObject Red2;
    public GameObject Blue2;
    public GameObject Yellow2;

    public GameObject Red3;
    public GameObject Blue3;
    public GameObject Yellow3;

    public GameObject targetObject;
    public float offsetY = 1.0f;
    private bool isDone = false;

    // public TextMeshPro statusText;
    public Camera raycastCamera;

    private string leftWire1Color = "none";
    private string leftWire2Color = "none";
    private string rightWireColor = "none";

    void Start()
    {
        Correct1.GetComponent<MeshRenderer>().enabled = false;
        Correct2.GetComponent<MeshRenderer>().enabled = false;
        Correct3.GetComponent<MeshRenderer>().enabled = false;

        /*
        if (statusText != null)
        {
            statusText.text = "";
        }
        */

        if (raycastCamera == null)
        {
            raycastCamera = Camera.main;
        }
    }

    void Update()
    {
        if (IsInputPressed())
        {
            Vector3 inputPosition = GetInputPosition();
            Ray ray = raycastCamera.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.gameObject.name)
                {
                    case "Red1": SetWireColor(LeftWire1, "red", ref leftWire1Color); break;
                    case "Blue1": SetWireColor(LeftWire1, "blue", ref leftWire1Color); break;
                    case "Yellow1": SetWireColor(LeftWire1, "yellow", ref leftWire1Color); break;

                    case "Red2": SetWireColor(LeftWire2, "red", ref leftWire2Color); break;
                    case "Blue2": SetWireColor(LeftWire2, "blue", ref leftWire2Color); break;
                    case "Yellow2": SetWireColor(LeftWire2, "yellow", ref leftWire2Color); break;

                    case "Red3": SetWireColor(RightWire, "red", ref rightWireColor); break;
                    case "Blue3": SetWireColor(RightWire, "blue", ref rightWireColor); break;
                    case "Yellow3": SetWireColor(RightWire, "yellow", ref rightWireColor); break;
                }

                CheckPattern();
            }
        }
    }

    void SetWireColor(GameObject wire, string color, ref string wireColor)
    {
        wireColor = color;
        Color newColor = Color.white;

        switch (color)
        {
            case "red": newColor = Color.red; break;
            case "blue": newColor = Color.blue; break;
            case "yellow": newColor = Color.yellow; break;
        }

        wire.GetComponent<Renderer>().material.color = newColor;
    }

    void CheckPattern()
    {
        if (leftWire1Color == "red" && leftWire2Color == "red" && rightWireColor == "yellow")
        {
            Correct1.GetComponent<MeshRenderer>().enabled = true;
            Correct2.GetComponent<MeshRenderer>().enabled = true;
            Correct3.GetComponent<MeshRenderer>().enabled = true;

            GetComponent<WiringTeleport>().TriggerTeleport();

            /*
            if (statusText != null)
            {
                statusText.text = "Done!";
            }
            */
        }
        else
        {
            Correct1.GetComponent<MeshRenderer>().enabled = false;
            Correct2.GetComponent<MeshRenderer>().enabled = false;
            Correct3.GetComponent<MeshRenderer>().enabled = false;

            /*
            if (statusText != null)
            {
                statusText.text = "";
            }
            */
        }
    }

    // Input handling for touch and mouse
    Vector3 GetInputPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        else
            return Input.mousePosition;
    }

    bool IsInputPressed()
    {
        return Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    /*
    void MoveToTargetPosition(GameObject correctWire)
    {
        if (targetObject != null)
        {
            Vector3 targetPosition = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + offsetY, targetObject.transform.position.z);
            correctWire.transform.position = targetPosition;
        }
    }
    */
}
