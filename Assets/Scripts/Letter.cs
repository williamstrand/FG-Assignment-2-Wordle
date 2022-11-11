using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    public char LetterValue => _letterValue;
    char _letterValue;

    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Image _background;

    public void SetLetter(char letter)
    {
        _letterValue = letter;
        _text.text = LetterValue.ToString().ToUpper();
    }

    public void Reset()
    {
        _letterValue = '\0';
        _text.text = "";
    }

    public void SetColor(LetterColor color)
    {
        switch(color)
        {
            case LetterColor.Red:
                _background.color = Color.red;
                break;

            case LetterColor.Green:
                _background.color = Color.green;
                break;

            case LetterColor.Yellow:
                _background.color = Color.yellow;
                break;
        }
    }
}

[Serializable]
public enum LetterColor
{
    Red,
    Green,
    Yellow,
    None
}
