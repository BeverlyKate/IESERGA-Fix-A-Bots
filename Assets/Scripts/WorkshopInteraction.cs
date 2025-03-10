using UnityEngine;

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

    private void HandleClick(Vector3 position) 
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        if(Physics.Raycast(ray.origin, ray.direction, out RaycastHit rayHit))
        {
            if(rayHit.transform.tag == "Station")
            {
                clickedObject = rayHit.transform;
                Debug.Log("Interacting with " + clickedObject.name);
                current_cam = clickedObject.Find("Camera").gameObject;

                current_cam.SetActive(true);
                inStationUI.SetActive(true);
                workshop_camera.SetActive(false);
            }
            else if(clickedObject != null)
            {
                Debug.Log("In a station");
            }
        }
    }

    public void LeaveStation()
    {
        inStationUI.SetActive(false);
        current_cam.SetActive(false);
        workshop_camera.SetActive(true);

        clickedObject = null;
    }
}
