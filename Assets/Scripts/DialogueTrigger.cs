using System.Xml.Serialization;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue[] dialogue;
    public Animator animator;

    private int nextCharacter = 0;
    private bool isDone = false;

    public void TriggerDialogue()
    {
        if(nextCharacter < dialogue.Length)
        {
            animator.SetBool("isTalking", true);
            FindAnyObjectByType<DialogueSystem>().StartDialogue(dialogue[nextCharacter]);
            nextCharacter++;
        }
        else
        {
            animator.SetBool("isTalking", false);
            isDone = true;
        }
    }

    public bool DialogueIsDone()
    {
        return isDone;
    }
}
