using System.Xml.Serialization;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue[] dialogue;
    public Animator animator;

    private int nextCharacter = 0;
    private bool hasTriggered = false; // Prevents repeated triggers

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player")) // Ensure the player is the one triggering it
        {
            hasTriggered = true; // Mark as triggered
            TriggerDialogue();
        }
    }
    public void TriggerDialogue()
    {
        if(nextCharacter< dialogue.Length)
        {
            animator.SetBool("isTalking", true);
            FindAnyObjectByType<DialogueSystem>().StartDialogue(dialogue[nextCharacter]);
            nextCharacter++;
        }
        else
        {
            animator.SetBool("isTalking", false);
            //Destroy(gameObject);
        }
    }
}
