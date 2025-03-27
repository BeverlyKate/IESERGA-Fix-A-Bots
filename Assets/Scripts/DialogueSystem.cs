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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueSentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue[] dialogue)
    {
        Debug.Log("First Char is " + dialogue[0].name);

        //characterImg.sprite = currentText.spriteImg.sprite;

        nameText.text = dialogue[0].name;

        dialogueSentences.Clear();

        foreach (string sentence in dialogue[0].sentences) 
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
        Debug.Log("End of Conversation");
    }


}
