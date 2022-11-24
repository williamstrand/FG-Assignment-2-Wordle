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

    public void SetColor(Color color)
    {
        _background.color = color;
    }
}
