using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDialogueManager : MonoBehaviour
{
    public GameObject dayStart;
    public GameObject TaskTutorial;
    public GameObject ToolTutorial;
    public GameObject ToolTutorial2;
    public GameObject ToolTutorial3;
    public GameObject PaintTutorial;
    public GameObject WireTutorial;
    public GameObject WireTutorial2;
    public GameObject AfterEverything;

    public Camera cam;

    [SerializeField]
    private Button afterTaskBtn;

    [SerializeField]
    private Button toolZoomBtn;

    [SerializeField]
    private Button toolRotateBtn;

    [SerializeField]
    private Wiring getWireGame;

    [SerializeField]
    private AssemblyStation completeTask;

    private bool afterTutorial = false;
    private bool itemsDistributed = false;
    private bool tutorialDone = false;
    private bool toolTriggered = false;
    private bool toolTriggered2 = false;
    private bool toolTriggered3 = false;
    private bool paintTriggered = false;
    private bool wireTriggered = false;
    private bool wireTriggered2 = false;

    private WireSlot wireSlotOfYellow;

    private Transform wireGame;

    private int noOfRotations = 0;

    Ray GetRay() => cam.ScreenPointToRay(Input.GetTouch(0).position);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dayStart.GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    void Update()
    {
        afterTaskBtn.onClick.AddListener(AfterTask);

        toolZoomBtn.onClick.AddListener(TriggerTool2Dialogue);

        toolRotateBtn.onClick.AddListener(TriggerTool3Dialogue);

        if (dayStart.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            dayStart.SetActive(false);
        }

        if (TaskTutorial.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            TaskTutorial.SetActive(false);
        }

        if (ToolTutorial.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            ToolTutorial.SetActive(false);
        }

        if (ToolTutorial2.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            ToolTutorial2.SetActive(false);
        }

        if (ToolTutorial3.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            ToolTutorial3.SetActive(false);
        }

        if (PaintTutorial.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            PaintTutorial.SetActive(false);
        }

        if (WireTutorial.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            WireTutorial.SetActive(false);
        }

        if (WireTutorial2.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            WireTutorial.SetActive(false);
        }

        if (AfterEverything.GetComponent<DialogueTrigger>().DialogueIsDone())
        {
            AfterEverything.SetActive(true);
        }

        if (afterTutorial)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (Physics.Raycast(GetRay(), out RaycastHit rayHit, Mathf.Infinity))
                    {
                        if (rayHit.transform.gameObject.CompareTag("Station"))
                        {
                            if (rayHit.transform.gameObject.name == "Tool")
                            {
                                TriggerToolDialogue();
                            }
                            else if(rayHit.transform.gameObject.name == "Paint")
                            {
                                TriggerPaintDialogue();
                            }
                            else if (rayHit.transform.gameObject.name == "Wiring")
                            {
                                TriggerWireDialogue();
                            }
                        }
                    }
                }
            }

            if(wireSlotOfYellow.isOccupied && !wireSlotOfYellow.isCorrect)
            {
                TriggerWire2Dialogue();
            }

            if (completeTask.missionDone)
            {
                FinalDialogue();
            }
        }
    }

    public void AfterTask()
    {
        if (!afterTutorial)
        {
            afterTutorial = true;
            TaskTutorial.SetActive(true);
            itemsDistributed = true;
            wireGame = getWireGame.wiringGame.transform.Find("Wireslot (3)");
            wireSlotOfYellow = wireGame.GetComponent<WireSlot>();
            TaskTutorial.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }

    private void FinalDialogue()
    {
        AfterEverything.SetActive(true);
        tutorialDone = true;
        AfterEverything.GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    private void TriggerToolDialogue()
    {
        if (!toolTriggered)
        {
            ToolTutorial.SetActive(true);
            toolTriggered = true;
            ToolTutorial.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }

    private void TriggerPaintDialogue()
    {
        if (!paintTriggered)
        {
            PaintTutorial.SetActive(true);
            paintTriggered = true;
            PaintTutorial.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }

    private void TriggerWireDialogue()
    {
        if (!wireTriggered)
        {
            WireTutorial.SetActive(true);
            wireTriggered = true;
            WireTutorial.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }

    private void TriggerTool2Dialogue()
    {
        if (!toolTriggered2)
        {
            ToolTutorial2.SetActive(true);
            toolTriggered2 = true;
            ToolTutorial2.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }

    private void TriggerTool3Dialogue()
    {
        noOfRotations++;
        if (!toolTriggered3 && noOfRotations >= 2)
        {
            ToolTutorial3.SetActive(true);
            toolTriggered3 = true;
            ToolTutorial3.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }

    private void TriggerWire2Dialogue()
    {
        if (!wireTriggered2)
        {
            WireTutorial2.SetActive(true);
            wireTriggered2 = true;
            WireTutorial2.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }
}
