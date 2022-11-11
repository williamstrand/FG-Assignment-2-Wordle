using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    const string KeyboardLetters = "qwertyuiopasdfghjklzxcvbnm";
    const int RowCount = 5;
    int CurrentRow = 0;
    [SerializeField] Word[] _rows;
    [SerializeField] TextAsset _wordsText;
    [SerializeField] string[] _words;
    [SerializeField] string _word;
    [SerializeField] Button[] _keyboardButtons;
    Dictionary<char, (LetterColor, Button)> _letterDictionary = new Dictionary<char, (LetterColor, Button)>();
    List<char> _input = new List<char>();
    [SerializeField] GameObject _gameObject;
    [SerializeField] GameObject _gameOverObject;
    [SerializeField] TextMeshProUGUI _wonText;
    [SerializeField] TextMeshProUGUI _lostText;
    [SerializeField] TextMeshProUGUI _wordText;
    bool _isGameOver = false;

    /// <summary>
    /// Builds the word database.
    /// </summary>
    [ContextMenu("Build Database")]
    void BuildWordDatabase()
    {
        _words = _wordsText.text.Split('\n');
        for (int i = 0; i < _words.Length; i++)
        {
            _words[i] = _words[i].Trim();
        }
    }

    /// <summary>
    /// Builds the on screen keyboard.
    /// </summary>
    void BuildKeyboard()
    {
        for (int i = 0; i < _keyboardButtons.Length; i++)
        {
            var index = i;
            var button = _keyboardButtons[index];
            var letter = KeyboardLetters[index];

            _letterDictionary.Add(letter, (LetterColor.None, button));

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => AddCharacter(letter));
        }
    }

    void Start()
    {
        if (_words.Length > 0)
        {
            BuildWordDatabase();
        }
        BuildKeyboard();
        _word = _words[Random.Range(0, _words.Length)];

        _gameOverObject.SetActive(false);
        _gameObject.SetActive(true);
    }

    /// <summary>
    /// Updates the current row.
    /// </summary>
    public void UpdateRow()
    {
        var word = new string(_input.ToArray());
        _rows[CurrentRow].SetWord(word);
    }

    /// <summary>
    /// Submits the inputted word.
    /// </summary>
    public void SubmitWord()
    {
        if (_isGameOver) return;

        var word = new string(_input.ToArray());

        if (word.Length < Word.WordLength) return;


        if (!ContainsWord(_words, word)) return;

        _input.Clear();

        UpdateLetterColors(_rows[CurrentRow], _word);

        if (word.ToLower() == _word.ToLower())
        {
            Win();
            return;
        }

        CurrentRow++;

        if (CurrentRow >= RowCount)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Updates the color of the inputted letter and the keyboard letters.
    /// </summary>
    /// <param name="row">the current row.</param>
    /// <param name="input">the input string.</param>
    void UpdateLetterColors(Word row, string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            var letter = row.Letters[i];
            var button = _letterDictionary[letter.LetterValue].Item2;

            LetterColor color;
            Color imageColor;

            if (letter.LetterValue == input[i])
            {
                color = LetterColor.Green;
                imageColor = Color.green;

                _letterDictionary[letter.LetterValue] = (color, button);
                button.GetComponent<Image>().color = imageColor;
                letter.SetColor(color);
            }
            else if (_letterDictionary[letter.LetterValue] == (LetterColor.None, button))
            {
                if (ContainsLetter(input, letter.LetterValue))
                {
                    color = LetterColor.Yellow;
                    imageColor = Color.yellow;
                }
                else
                {
                    color = LetterColor.Red;
                    imageColor = Color.red;
                }

                _letterDictionary[letter.LetterValue] = (color, button);
                button.GetComponent<Image>().color = imageColor;
                letter.SetColor(color);
            }
            else
            {
                letter.SetColor(_letterDictionary[letter.LetterValue].Item1);
            }
        }
    }

    /// <summary>
    /// Checks if an array of words contain a specific word.
    /// </summary>
    /// <param name="words">the array of words.</param>
    /// <param name="word">the word to check for.</param>
    /// <returns>true if array contains the word.</returns>
    bool ContainsWord(string[] words, string word)
    {
        foreach (var w in words)
        {
            if (w.ToLower() == word.ToLower())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a word contains a certain letter.
    /// </summary>
    /// <param name="word">the word to check.</param>
    /// <param name="letter">the letter to check for.</param>
    /// <returns>true if word contains letter.</returns>
    bool ContainsLetter(string word, char letter)
    {
        foreach (var w in word)
        {
            if (char.ToLower(w) == char.ToLower(letter))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Triggers a win.
    /// </summary>
    void Win()
    {
        _isGameOver = true;
        Debug.Log("Win");
        foreach (Letter letter in _rows[CurrentRow].Letters)
        {
            letter.SetColor(LetterColor.Green);
        }
        StartCoroutine(EndSequence(true));
    }

    /// <summary>
    /// Triggers a game over.
    /// </summary>
    void GameOver()
    {
        _isGameOver = true;
        Debug.Log($"Lose. Word was: {_word.ToUpper()}");
        StartCoroutine(EndSequence(false));
    }

    IEnumerator EndSequence(bool won)
    {
        yield return new WaitForSeconds(2);

        _gameOverObject.SetActive(true);
        _gameObject.SetActive(false);

        _lostText.gameObject.SetActive(!won);
        _wonText.gameObject.SetActive(won);

        _wordText.gameObject.SetActive(true);
        _wordText.text = $"The word was:\n{_word.ToUpper()}";

    }

    /// <summary>
    /// Adds a character to the end of the input list.
    /// </summary>
    /// <param name="character">the character to add.</param>
    public void AddCharacter(char character)
    {
        if (_isGameOver) return;
        _input.Add(character);
        UpdateRow();
    }

    /// <summary>
    /// Removes the last character from the input list.
    /// </summary>
    public void RemoveCharacter()
    {
        if (_isGameOver) return;
        if (_input.Count <= 0) return;

        _input.RemoveAt(_input.Count - 1);
        UpdateRow();
    }
}
