using System.Xml.Serialization;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue[] dialogue;


    private int nextCharacter = 0;

    public void TriggerDialogue()
    {
        if(nextCharacter<= dialogue.Length)
        {
            FindAnyObjectByType<DialogueSystem>().StartDialogue(dialogue[nextCharacter]);
            nextCharacter++;
        }
    }
}
