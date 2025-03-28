using UnityEngine;
using System.Collections;

public class ToolGenerator : MonoBehaviour
{
    private SpriteRenderer genScreen;

    [SerializeField]
    private Sprite[] shapes;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private int increment;

    [SerializeField]
    private GameObject screenEdge;

    private int counter = 0;

    Ray GetRay() => cam.ScreenPointToRay(Input.GetTouch(0).position);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        genScreen = GetComponentInChildren<SpriteRenderer>();

        Debug.Log(cam.transform.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (Physics.Raycast(GetRay(), out RaycastHit rayHit, Mathf.Infinity))
                {
                    if (rayHit.transform.gameObject.CompareTag("Crank"))
                    {
                        counter++;
                        genScreen.sprite = shapes[counter];
                    }

                    if(rayHit.transform.gameObject.name == "Enter")
                    {
                        if(counter != increment)
                        {
                            counter = 0;
                            genScreen.sprite = shapes[counter];
                            Debug.Log("Wrong Answer");
                            StartCoroutine(Counter(3));
                                
                        }
                        else
                        {
                            screenEdge.GetComponent<Renderer>().material.color = Color.green;
                        }
                    }

                    if(rayHit.transform.gameObject.name == "Generator")
                    {
                        cam.transform.position = new Vector3(transform.position.x, transform.position.y + 0.175f, transform.position.z - transform.position.z / 9);
                        cam.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    }
                }
            }
        }
    }

    private IEnumerator Counter(int seconds)
    {
        screenEdge.GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSecondsRealtime(seconds);
        screenEdge.GetComponent<Renderer>().material.color = Color.white;
    }
}
