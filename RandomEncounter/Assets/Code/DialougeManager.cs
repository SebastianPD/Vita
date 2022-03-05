using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialougeManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialougeText;
    private Queue<string> sentences;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }
    public void StartDialouge(Dialouge dialouge) 
    {
        animator.SetBool("IsOpen", true);
        nameText.text = dialouge.name;
        sentences.Clear();
        foreach (string sentence in dialouge.sentences) 
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
       
    }

    public void StartRandomDialouge(Dialouge dialouge)
    {
        animator.SetBool("IsOpen", true);
        nameText.text = dialouge.name;
        sentences.Clear();
        int numOfSentences = dialouge.sentences.Length;
        int sentence = Random.Range(0, numOfSentences);

        sentences.Enqueue(dialouge.sentences[sentence]);
        
        DisplayNextSentence();

    }

    public void DisplayNextSentence() 
    {
        if (sentences.Count == 0) 
        {
            EndDialouge();
            return;
        }

        string DisplaySentence = sentences.Dequeue();
        dialougeText.text = DisplaySentence;
    }

    void EndDialouge() 
    {
        animator.SetBool("IsOpen", false);
    }

    // Update is called once per frame
}
