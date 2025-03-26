using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    private Queue<string> sentences;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue[] dialogue)
    {
        Debug.Log("First Char is " + dialogue[0].name);

        sentences.Clear();

        foreach (Dialogue currentText in dialogue)
        {
            foreach (string sentence in currentText.sentences) 
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
        }
    }
}
