using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string questionText = "";
    public string answerText = "";

    public Question() { }

    public Question(string questionText, string answerText)
    {
        this.questionText = questionText;
        this.answerText = answerText;
    }
}
