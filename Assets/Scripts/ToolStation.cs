using UnityEngine;

public class ToolStation : MonoBehaviour
{
    private GameObject[] missingLocation;

    private Transform chosenBolt;

    private Camera cam;

    private bool draggingObj = false;

    private float deltaPosition;

    private Vector3 originalPosition;

    private Vector3 offset;

    Ray GetRay()=> cam.ScreenPointToRay(Input.mousePosition);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        missingLocation = GameObject.FindGameObjectsWithTag("Screw");

        cam = gameObject.GetComponentInChildren<Camera>();
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
            if (!draggingObj) {
                HandleClick(Input.mousePosition);
            }

            if (draggingObj)
            {
                Debug.Log(draggingObj);
                if(Physics.Raycast(GetRay(), out RaycastHit rayHit))
                {
                    chosenBolt.position = rayHit.point;
                }
            }
            //Debug.Log("Clicking in station");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (draggingObj)
            {
                draggingObj = false;
                Debug.Log(draggingObj);
                chosenBolt.position = originalPosition;
            }
        }
    }

    private void HandleClick(Vector3 position)
    {
        Ray ray = cam.ScreenPointToRay(position);
        if (Physics.Raycast(GetRay(), out RaycastHit rayHit, Mathf.Infinity))
        {
            //Debug.Log(rayHit.transform.name);
            if (rayHit.transform.gameObject.CompareTag("Option"))
            {
                Debug.Log("Chose " + rayHit.transform.name);
                chosenBolt = rayHit.transform;
                originalPosition = chosenBolt.position;
                draggingObj = true;
            }
        }
    }
}
