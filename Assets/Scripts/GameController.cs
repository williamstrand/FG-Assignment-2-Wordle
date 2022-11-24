using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    const string KeyboardLetters = "qwertyuiopasdfghjklzxcvbnm";
    const int RowCount = 5;
    [SerializeField] Color CorrectColor;
    [SerializeField] Color WrongPosColor;
    [SerializeField] Color WrongColor;
    [SerializeField] Color CurrentRowColor;

    public event Action<bool, string> OnGameEnd
    {
        add { _onGameEnd += value; }
        remove { _onGameEnd -= value; }
    }
    event Action<bool, string> _onGameEnd;

    Dictionary<char, Button> _buttonDict = new Dictionary<char, Button>();
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

        if (_word.Length < Word.WordLength)
        {
            _word = _words[UnityEngine.Random.Range(0, _words.Length)];
        }

        foreach (var letter in _rows[CurrentRow].Letters)
        {
            letter.SetColor(CurrentRowColor);
        }
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

            _buttonDict.Add(letter, button);

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

        if (CheckIfValidWord(guess))
        { 
            StartCoroutine(InvalidWordAnimation());
            return;
        }

        _input.Clear();

        UpdateLetterColors(_rows[CurrentRow], _word);

        if (CheckIfWon(guess))
        {
            Win();
            return;
        }

        NextRow();
    }

    bool CheckIfValidWord(string word) => !ContainsWord(_words, word) || word.Length < Word.WordLength;

    IEnumerator InvalidWordAnimation()
    {
        var lerp = 0f;
        while (lerp < 1)
        {
            foreach (var letter in _rows[CurrentRow].Letters)
            {
                letter.SetColor(Color.Lerp(WrongColor, CurrentRowColor, lerp));
                lerp += Time.deltaTime;
            }
            yield return null;
        }
    }

    void NextRow()
    {
        CurrentRow++;

        if (CurrentRow >= RowCount)
        {
            GameOver();
            return;
        }

        foreach (var letter in _rows[CurrentRow].Letters)
        {
            letter.SetColor(CurrentRowColor);
        }
    }

    /// <summary>
    /// Checks if player has won.
    /// </summary>
    /// <param name="guess">the player's guess.</param>
    /// <returns>true if the player has won.</returns>
    bool CheckIfWon(string guess) => string.Compare(guess, _word, true) == 0;

    /// <summary>
    /// Updates the color of the inputted letter and the keyboard letters.
    /// </summary>
    /// <param name="row">the current row.</param>
    /// <param name="word">the word string.</param>
    void UpdateLetterColors(Word row, string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            var letter = row.Letters[i];
            var button = _buttonDict[letter.LetterValue];

            Color imageColor;

            if (letter.LetterValue == word[i])
            {
                imageColor = CorrectColor;

                button.GetComponent<Image>().color = imageColor;
                letter.SetColor(CorrectColor);
            }
            else
            {
                if (ContainsLetter(word, letter.LetterValue))
                {
                    imageColor = WrongPosColor;
                }
                else
                {
                    imageColor = WrongColor;
                }
                button.GetComponent<Image>().color = imageColor;
                letter.SetColor(imageColor);
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
            letter.SetColor(CorrectColor);
        }
        StartCoroutine(EndSequence(true));
    }

    /// <summary>
    /// Triggers a game over.
    /// </summary>
    public void GameOver()
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
