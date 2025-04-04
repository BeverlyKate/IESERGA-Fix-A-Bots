using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WorkshopInteraction : MonoBehaviour
{
    private Transform clickedObject;

    private GameObject current_cam;

    private GameObject inStationUI;

    private GameObject workshop_camera;


    void Start()
    {
        inStationUI = GameObject.FindGameObjectWithTag("StationUI");
        workshop_camera = GameObject.FindGameObjectWithTag("MainCamera");

        inStationUI.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (workshop_camera.activeSelf)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    HandleClick(Input.GetTouch(0).position);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                HandleClick(Input.mousePosition);
            }
        }
        
    }

    private void HandleClick(Vector3 position) 
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit rayHit))
        {
            if (rayHit.transform.gameObject.CompareTag("Station"))
            {
                clickedObject = rayHit.transform;
                Debug.Log("Interacting with " + clickedObject.name);

                Transform cameraTransform = clickedObject.Find("Camera");
                if (cameraTransform != null)
                {
                    current_cam = cameraTransform.gameObject;
                    current_cam.SetActive(true);
                    inStationUI.SetActive(true);
                    workshop_camera.SetActive(false);
                    ActivateScript();
                }
                else
                {
                    Debug.LogError("Camera not found in " + clickedObject.name);
                }

                
                DialogueTrigger dialogueTrigger = clickedObject.GetComponent<DialogueTrigger>();
                if (dialogueTrigger != null)
                {
                    dialogueTrigger.TriggerDialogue(); // Start dialogue after zooming
                }
                
            }
        }
            /*
            Ray ray = Camera.main.ScreenPointToRay(position);
            if(Physics.Raycast(ray.origin, ray.direction, out RaycastHit rayHit))
            {
                if(rayHit.transform.gameObject.CompareTag("Station"))
                {
                    clickedObject = rayHit.transform;
                    Debug.Log("Interacting with " + clickedObject.name);
                    current_cam = clickedObject.Find("Camera").gameObject;


                    current_cam.SetActive(true);
                    inStationUI.SetActive(true);
                    workshop_camera.SetActive(false);
                    ActivateScript();
                }
                else if(clickedObject != null)
                {
                    Debug.Log("In a station");
                }
            }
            */
    }

    public void LeaveStation()
    {
        inStationUI.SetActive(false);
        current_cam.SetActive(false);
        workshop_camera.SetActive(true);
        DeactivateScript();

        clickedObject = null;
    }

    private void ActivateScript()
    {
        if(clickedObject.name == "Tool")
        {
            clickedObject.GetComponent<ToolStation>().enabled = true;
            clickedObject.GetComponent<BoxCollider>().enabled = false;
        }
        else if (clickedObject.name == "Paint")
        {
            clickedObject.GetComponent<PaintStation>().enabled = true;
            clickedObject.GetComponent<BoxCollider>().enabled = false;
            clickedObject.GetComponent<PaintResetManager>().EnableBtn();
        }
        else if (clickedObject.name == "Wiring")
        {
            clickedObject.GetComponent<Wiring>().enabled = true;
            clickedObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void DeactivateScript()
    {
        if (clickedObject.name == "Tool")
        {
            clickedObject.GetComponent<ToolStation>().enabled = false;
            clickedObject.GetComponent<BoxCollider>().enabled = true;
        }
        else if (clickedObject.name == "Paint")
        {
            clickedObject.GetComponent<PaintStation>().enabled = false;
            clickedObject.GetComponent<BoxCollider>().enabled = true;
            clickedObject.GetComponent<PaintResetManager>().DisableBtn();
        }
        else if (clickedObject.name == "Wiring")
        {
            clickedObject.GetComponent<Wiring>().enabled = false;
            clickedObject.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
