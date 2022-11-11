using UnityEngine;

public class Word : MonoBehaviour
{
    public const int WordLength = 5;
    public Letter[] Letters;

    public string WordValue { get; private set; }

    void Start()
    {
        Letters = transform.GetComponentsInChildren<Letter>();
    }

    public void SetWord(string word)
    {
        Reset();
        WordValue = word;
        for (int i = 0; i < word.Length; i++)
        {
            Letters[i].SetLetter(word[i]);
        }
    }

    public void Reset()
    {
        foreach (Letter letter in Letters)
        {
            letter.Reset();
        }
    }
}
