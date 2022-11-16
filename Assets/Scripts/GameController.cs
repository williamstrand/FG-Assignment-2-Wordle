using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    const string KeyboardLetters = "qwertyuiopasdfghjklzxcvbnm";
    const int RowCount = 5;

    public event Action<bool, string> OnGameEnd
    {
        add { _onGameEnd += value; }
        remove { _onGameEnd -= value; }
    }
    event Action<bool, string> _onGameEnd;

    Dictionary<char, (LetterColor, Button)> _letterDictionary = new Dictionary<char, (LetterColor, Button)>();
    [SerializeField] TextAsset _wordsText;

    [SerializeField] Word[] _rows;
    [SerializeField] Button[] _keyboardButtons;
    [SerializeField] string[] _words;
    [SerializeField] string _word;

    int CurrentRow = 0;
    List<char> _input = new List<char>();
    bool _isGameOver = false;

    #region Initialization

    void Start()
    {
        if (_words.Length > 0)
        {
            BuildWordDatabase();
        }
        BuildKeyboard();
        _word = _words[UnityEngine.Random.Range(0, _words.Length)];
    }

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

    #endregion

    #region Submit Word

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

        var guess = new string(_input.ToArray());

        if (guess.Length < Word.WordLength) return;
        if (!ContainsWord(_words, guess)) return;

        _input.Clear();

        UpdateLetterColors(_rows[CurrentRow], _word);

        if (CheckIfWon(guess))
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

    bool CheckIfWon(string guess) => string.Compare(guess, _word, true) == 0;

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

    #endregion

    #region Help Functions

    /// <summary>
    /// Checks if an array of words contain a specific word.
    /// </summary>
    /// <param name="words">the array of words.</param>
    /// <param name="word">the word to check for.</param>
    /// <returns>true if array contains the word.</returns>
    static bool ContainsWord(string[] words, string word)
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
    static bool ContainsLetter(string word, char letter)
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

    #endregion

    #region End

    /// <summary>
    /// Triggers a win.
    /// </summary>
    void Win()
    {
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
        StartCoroutine(EndSequence(false));
    }

    /// <summary>
    /// End game sequence.
    /// </summary>
    /// <param name="won">if player won.</param>
    IEnumerator EndSequence(bool won)
    {
        _isGameOver = true;
        yield return new WaitForSeconds(2);

        _onGameEnd?.Invoke(won, _word);
    }

    #endregion

    #region Input

    /// <summary>
    /// Adds a character to the end of the input list.
    /// </summary>
    /// <param name="character">the character to add.</param>
    public void AddCharacter(char character)
    {
        if (_isGameOver) return;
        if (_input.Count >= Word.WordLength) return;

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

    #endregion
}
