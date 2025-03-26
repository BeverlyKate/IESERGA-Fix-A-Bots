using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue[] dialogue;

    public void TriggerDialogue()
    {
        FindAnyObjectByType<DialogueSystem>().StartDialogue(dialogue);
    }
}
