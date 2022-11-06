using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word
{
    public const int WordLength = 5;
    public Letter[] Letters = new Letter[WordLength];

    public Word(string word)
    {
        if(word.Length != WordLength) return;

        for (int i = 0; i < WordLength; i++)
        {
            Letters[i] = new Letter(word[i]);
        }
    }

    public Word()
    {

    }

}
