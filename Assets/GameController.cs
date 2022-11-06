using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public const int RowCount = 5;
    public int CurrentRow = 0;
    public Word[] _rows = new Word[RowCount];
    [SerializeField] TextMeshProUGUI[] _lettersText;
    [SerializeField] TextMeshProUGUI _inputField;

    void Start()
    {
        for (int i = 0; i < RowCount; i++)
        {
            _rows[i] = new Word();
        }

    }

    public void UpdateRow()
    {
        var word = _inputField.text;
        for (int i = 0; i < word.Length; i++)
        {
            _rows[CurrentRow].Letters[i].LetterValue = word[i];
        }
    }

    public void SubmitWord()
    {
        if (_inputField.text.Length < Word.WordLength) return;

        var word = _inputField.text;
        _inputField.text = "";
        Debug.Log(word);
    }
}
