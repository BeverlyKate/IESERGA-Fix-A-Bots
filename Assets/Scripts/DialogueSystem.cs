using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image characterImg;

    private Queue<string> dialogueSentences;
    private DialogueTrigger currentTrigger;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueSentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        characterImg.sprite = dialogue.spriteImg;

        nameText.text = dialogue.name;

        dialogueSentences.Clear();

        foreach (string sentence in dialogue.sentences) 
        {
            dialogueSentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (dialogueSentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = dialogueSentences.Dequeue();
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        FindAnyObjectByType<DialogueTrigger>().TriggerDialogue();
        Debug.Log("End of Conversation");
    }


}
