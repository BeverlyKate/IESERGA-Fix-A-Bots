using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using TMPro;


public class ToolStation : MonoBehaviour
{
    public GameObject nailPrefab;

    public GameObject screwPrefab;

    private GameObject chosenBolt;

    private GameObject chosenTool;

    private Vector3 chosenToolPos;

    private Vector3 chosenToolRot;

    private Camera cam;

    private bool draggingObj = false;

    private int choseNum;

    private int toolNum;

    private float hitCounter = 0;

    private bool onBolt=false;

    private GameObject boltToEdit;

    private int boltPos;

    private RobotHead roboPart;

    public TextMeshPro statusText;
    public GameObject targetObject;
    public GameObject objectToTeleport;
    public float offsetY = 1.0f;

    Ray GetRay()=> cam.ScreenPointToRay(Input.GetTouch(0).position);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //missingLocations = GameObject.FindGameObjectsWithTag("Unknown");

        cam = gameObject.GetComponentInChildren<Camera>();

        roboPart = gameObject.GetComponentInChildren<RobotHead>();

        Debug.Log(roboPart.transform.gameObject.name);
        if (statusText != null)
        {
            statusText.text = "Not done";
        }
    }

    // Update is called once per frame
    void Update()
    {
        int placement = 0;
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
                            //call function from robothead
                            placement = roboPart.locationPlacement(rayHit.transform.gameObject);
                            Debug.Log(placement);
                            Debug.Log(draggingObj);
                            if (roboPart.checkLocation(placement, choseNum))
                            {
                                if (choseNum == 0)
                                {
                                    Vector3 sizeVec = chosenBolt.transform.GetComponent<Renderer>().bounds.size;
                                    Vector3 addedPos = rayHit.transform.gameObject.GetComponentInChildren<Transform>().position;
                                    float newY = addedPos.y + sizeVec.y;
                                    chosenBolt.transform.position = new Vector3(addedPos.x, newY, addedPos.z);
                                }
                                else
                                {
                                    chosenBolt.transform.position = rayHit.transform.gameObject.GetComponentInChildren<Transform>().position;
                                }
                                roboPart.setObjectPlacement(chosenBolt, placement);
                                chosenBolt = null;
                                draggingObj = false;
                            }
                            
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
                        if (rayHit.transform.gameObject.CompareTag("Part"))
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
            
            // triggers DONE sign
            if (roboPart.checkIncrement())
            {
                roboPart.triggerDone();
                if (statusText != null)
                {
                    statusText.text = "Done!";
                }
                MoveToTargetPosition();
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
                    choseNum = 0;
                }
                else if(rayHit.transform.name == "Screw Box")
                {
                    prefabToInstantiate = screwPrefab;
                    choseNum = 1;
                }

                chosenBolt = (GameObject) Instantiate(prefabToInstantiate, rayHit.point, Quaternion.identity);
                draggingObj = true;
            }

            if (rayHit.transform.gameObject.CompareTag("Tool"))
            {
                if (chosenTool == null)
                {
                    if (rayHit.transform.name == "Hammer")
                    {
                        toolNum = 0;
                    }
                    else
                    {
                        toolNum = 1;
                    }

                    chosenTool = rayHit.transform.gameObject;

                    chosenToolPos = rayHit.transform.position;

                    chosenToolRot = rayHit.transform.eulerAngles;

                    rayHit.transform.position = new Vector3(chosenToolPos.x, chosenToolPos.y + 0.05f, chosenToolPos.z);
                }
                else if(chosenTool!=null && !onBolt)
                {
                    chosenTool.transform.position = chosenToolPos;

                    chosenTool.transform.eulerAngles = chosenToolRot;

                    chosenTool = null;
                }

                if (onBolt)
                {
                    if (hitCounter < roboPart.checkCorrectPosition(boltPos))
                    {
                        if (boltToEdit.TryGetComponent(out Nail nail))
                        {
                            if(hitCounter < 2)
                            {
                                nail.transform.position = new Vector3(nail.transform.position.x, nail.transform.position.y - 0.02f, nail.transform.position.z);
                                hitCounter += 1f;
                            }
                            else
                            {
                                nail.transform.position = new Vector3(nail.transform.position.x, nail.transform.position.y - 0.007f, nail.transform.position.z);
                                hitCounter += 1f;
                            }
                        }
                        else if (boltToEdit.TryGetComponent(out Screw screw))
                        {
                            screw.transform.eulerAngles = new Vector3(screw.transform.eulerAngles.x, screw.transform.eulerAngles.y + 22.5f, screw.transform.eulerAngles.z);
                            hitCounter += 22.5f;
                        }
                    }
                    else
                    {
                        roboPart.incrementCorrect();
                        hitCounter = 0f;
                        chosenTool.transform.position = chosenToolPos;
                        chosenTool.transform.eulerAngles = chosenToolRot;
                        chosenTool = null;
                        boltToEdit = null;
                        onBolt = false;
                    }
                }
            }

            if (!onBolt)
            {
                if (rayHit.transform.gameObject.TryGetComponent(out Nail nail) && chosenTool.transform.name == "Hammer")
                {
                    Vector3 toolSize = chosenTool.transform.GetComponentInChildren<Renderer>().bounds.size;
                    Vector3 nailSize = nail.transform.GetComponent<Renderer>().bounds.size;
                    float newHeight = nailSize.y;
                    chosenTool.transform.position = new Vector3(nail.transform.position.x, nail.transform.position.y + newHeight, nail.transform.position.z);
                    chosenTool.transform.eulerAngles = new Vector3(chosenTool.transform.eulerAngles.x, chosenTool.transform.eulerAngles.y + 90f, chosenTool.transform.eulerAngles.z);
                    onBolt = true;
                    boltToEdit = nail.gameObject;
                    boltPos = roboPart.findFastenerPos(boltToEdit);
                }
                else if (rayHit.transform.gameObject.TryGetComponent(out Screw screw) && chosenTool.transform.name == "Screw Driver")
                {
                    Debug.Log("Interacting");
                    Vector3 toolSize = chosenTool.transform.GetComponentInChildren<Renderer>().bounds.size;
                    float newHeight = toolSize.z / 2;
                    chosenTool.transform.position = new Vector3(screw.transform.position.x, screw.transform.position.y + newHeight, screw.transform.position.z);
                    chosenTool.transform.eulerAngles = new Vector3(chosenTool.transform.eulerAngles.x, chosenTool.transform.eulerAngles.y, chosenTool.transform.eulerAngles.z + 90f);
                    onBolt = true;
                    boltToEdit = screw.gameObject;
                    boltPos = roboPart.findFastenerPos(boltToEdit);
                }
            }
        }
    }

    private void MoveToTargetPosition()
    {
        if (objectToTeleport != null && targetObject != null)
        {
            Vector3 targetPosition = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + offsetY, targetObject.transform.position.z);
            objectToTeleport.transform.position = targetPosition;
        }
    }

    public void clearHold()
    {
        Destroy(chosenBolt);
        choseNum = -1;
        draggingObj = false;
    }
}
