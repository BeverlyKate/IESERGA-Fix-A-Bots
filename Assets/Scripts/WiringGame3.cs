using UnityEngine;
using TMPro;

public class WiringGame3 : MonoBehaviour
{
    public GameObject LeftWire1;
    public GameObject LeftWire2;
    public GameObject LeftWire3;
    public GameObject RightWire;

    public GameObject Correct1;
    public GameObject Correct2;
    public GameObject Correct3;
    public GameObject Correct4;

    public GameObject targetObject;
    public float offsetY = 1.0f;
    public TextMeshPro statusText;
    public Camera raycastCamera;

    private string leftWire1Color = "none";
    private string leftWire2Color = "none";
    private string leftWire3Color = "none";
    private string rightWireColor = "none";

    void Start()
    {
        Correct1.GetComponent<MeshRenderer>().enabled = false;
        Correct2.GetComponent<MeshRenderer>().enabled = false;
        Correct3.GetComponent<MeshRenderer>().enabled = false;
        Correct4.GetComponent<MeshRenderer>().enabled = false;

        if (statusText != null)
        {
            statusText.text = "";
        }

        if (raycastCamera == null)
        {
            raycastCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
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

                    case "Red3": SetWireColor(LeftWire3, "red", ref leftWire3Color); break;
                    case "Blue3": SetWireColor(LeftWire3, "blue", ref leftWire3Color); break;
                    case "Yellow3": SetWireColor(LeftWire3, "yellow", ref leftWire3Color); break;

                    case "Red4": SetWireColor(RightWire, "red", ref rightWireColor); break;
                    case "Blue4": SetWireColor(RightWire, "blue", ref rightWireColor); break;
                    case "Yellow4": SetWireColor(RightWire, "yellow", ref rightWireColor); break;
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
        if (leftWire1Color == "red" && leftWire2Color == "red" && leftWire3Color == "red" && rightWireColor == "yellow")
        {
            Correct1.GetComponent<MeshRenderer>().enabled = true;
            Correct2.GetComponent<MeshRenderer>().enabled = true;
            Correct3.GetComponent<MeshRenderer>().enabled = true;
            Correct4.GetComponent<MeshRenderer>().enabled = true;
            MoveToTargetPosition(Correct1);
            MoveToTargetPosition(Correct2);
            MoveToTargetPosition(Correct3);
            MoveToTargetPosition(Correct4);

            if (statusText != null)
            {
                statusText.text = "Done!";
            }
        }
        else
        {
            Correct1.GetComponent<MeshRenderer>().enabled = false;
            Correct2.GetComponent<MeshRenderer>().enabled = false;
            Correct3.GetComponent<MeshRenderer>().enabled = false;
            Correct4.GetComponent<MeshRenderer>().enabled = false;

            if (statusText != null)
            {
                statusText.text = "";
            }
        }
    }

    void MoveToTargetPosition(GameObject correctWire)
    {
        if (targetObject != null)
        {
            Vector3 targetPosition = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + offsetY, targetObject.transform.position.z);
            correctWire.transform.position = targetPosition;
        }
    }
}
