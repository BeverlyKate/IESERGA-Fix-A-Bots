using UnityEngine;

public class WiringGame : MonoBehaviour
{
    public GameObject LeftWire;
    public GameObject RightWire;
    public GameObject Correct1;
    public GameObject Correct2;

    public GameObject Red;
    public GameObject Blue;
    public GameObject Yellow;

    public GameObject Red1;
    public GameObject Blue1;
    public GameObject Yellow1;

    public Camera raycastCamera;

    private string leftWireColor = "none";
    private string rightWireColor = "none";

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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
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
        }
        else
        {
            Correct1.GetComponent<MeshRenderer>().enabled = false;
            Correct2.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
