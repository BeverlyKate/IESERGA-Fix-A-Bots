using UnityEngine;
using System.Collections.Generic;

/* 
 *
 *    TODO:
 *    
 *      Add 2 kinds of zoom in features - 1 zooms into the pattern itself, the other zooms into the individual parts
 *      have option spawn rather than have it be dragged from the table (More convenient)
 *      have tools be icons that appear after option has been placed in the hole.
 * 
 */
public class ToolStation : MonoBehaviour
{
    public GameObject nailPrefab;

    public GameObject screwPrefab;

    private List<GameObject> missingLocations = new List<GameObject> ();

    private GameObject chosenBolt;

    private Camera cam;

    private bool draggingObj = false;

    private Vector3 originalPosition;

    private GameObject robotPart;

    Ray GetRay()=> cam.ScreenPointToRay(Input.GetTouch(0).position);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //missingLocations = GameObject.FindGameObjectsWithTag("Unknown");

        cam = gameObject.GetComponentInChildren<Camera>();

        robotPart = GameObject.Find("Robot Head");

        Debug.Log(robotPart.transform.name);

        foreach(Transform child in robotPart.transform)
        {
            if (child.CompareTag("Unknown"))
            {
                missingLocations.Add(child.gameObject);
            }
        }

        Debug.Log(missingLocations.Count);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!draggingObj)
                {
                    HandleClick(Input.GetTouch(0).position);
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (draggingObj)
                {
                    Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                    if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity))
                    {
                        if (rayHit.transform.gameObject.CompareTag("Unknown"))
                        {
                            chosenBolt.transform.position = rayHit.transform.gameObject.GetComponentInChildren<Transform>().position;
                            draggingObj = false;
                            Debug.Log(draggingObj);
                            chosenBolt = null;
                        }
                    }
                }
            }
            else
            {
                if (draggingObj)
                {
                    Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                    if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity))
                    {
                        if (rayHit.transform.gameObject.name == "Head")
                        {
                            chosenBolt.transform.position = new Vector3(rayHit.point.x, rayHit.point.y, rayHit.point.z);
                        }
                        else
                        {
                            chosenBolt.transform.position = new Vector3(rayHit.point.x, chosenBolt.transform.position.y, rayHit.point.z);
                        }
                    }
                }
            }
        }
    }

    private void HandleClick(Vector3 position)
    {
        GameObject prefabToInstantiate = null;
        //Ray ray = cam.ScreenPointToRay(position);
        if (Physics.Raycast(GetRay(), out RaycastHit rayHit, Mathf.Infinity))
        {
            //Debug.Log(rayHit.transform.name);
            if (rayHit.transform.gameObject.CompareTag("Option"))
            {
                Debug.Log("Chose " + rayHit.transform.name);

                if(rayHit.transform.name == "Nail Box")
                {
                    prefabToInstantiate = nailPrefab;
                }
                else if(rayHit.transform.name == "Screw Box")
                {
                    prefabToInstantiate = screwPrefab;
                }

                chosenBolt = (GameObject) Instantiate(prefabToInstantiate, rayHit.point, Quaternion.identity);
                originalPosition = chosenBolt.transform.position;
                draggingObj = true;
            }
        }
    }
}
