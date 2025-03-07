using UnityEngine;

public class WorkshopInteraction : MonoBehaviour
{
    private Transform clickedObject;

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
                //ray = new Ray();
                //rayHit = new RaycastHit();
            }else if(clickedObject != null)
            {
                Debug.Log("In a station");
            }
        }
    }
}
