using UnityEngine;

public class Level3DialogueManager : MonoBehaviour
{
    public GameObject lvl3Start;
    public GameObject wireTutorial;
    public Camera cam;

    private bool startDone = false;
    private bool tutorialDone = false;
    Ray GetRay() => cam.ScreenPointToRay(Input.GetTouch(0).position);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lvl3Start.GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    void Update()
    {
        if (lvl3Start.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            lvl3Start.SetActive(false);
            startDone = true;
        }

        if (wireTutorial.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            wireTutorial.SetActive(false);
        }

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (Physics.Raycast(GetRay(), out RaycastHit rayHit, Mathf.Infinity))
                {
                    if (rayHit.transform.gameObject.CompareTag("Station"))
                    {
                        if (rayHit.transform.gameObject.name == "Wiring")
                        {
                            TriggerNewWireTutorial();
                        }
                    }
                }
            }
        }
    }

    private void TriggerNewWireTutorial()
    {
        if (!tutorialDone)
        {
            wireTutorial.SetActive(true);
            tutorialDone = true;
            wireTutorial.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
            
    }
}
