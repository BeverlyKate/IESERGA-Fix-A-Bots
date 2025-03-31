using UnityEngine;
using UnityEngine.UI;

public class TutorialDialogueManager : MonoBehaviour
{
    public GameObject dayStart;
    public GameObject TaskTutorial;

    [SerializeField]
    private Button afterTask;

    private bool afterTutorial = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dayStart.GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    void Update()
    {
        afterTask.onClick.AddListener(AfterTask);
        
    }

    public void AfterTask()
    {
        if (!afterTutorial)
        {
            afterTutorial = true;
            TaskTutorial.SetActive(true);
            TaskTutorial.GetComponent<DialogueTrigger>().TriggerDialogue();
        }
    }
}
