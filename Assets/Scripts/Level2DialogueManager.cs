using UnityEngine;

public class Level2DialogueManager : MonoBehaviour
{
    public DialogueTrigger level2Dialogue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        level2Dialogue.TriggerDialogue();
    }

    void Update()
    {
        if (level2Dialogue.DialogueIsDone())
        {
            level2Dialogue.gameObject.SetActive(false);
        }
    }
}
