using TMPro;
using UnityEngine;

public class EndScreenController : MonoBehaviour
{
    [SerializeField] GameObject _gameOverObject;
    [SerializeField] GameObject _gameObject;
    [SerializeField] TextMeshProUGUI _wonText;
    [SerializeField] TextMeshProUGUI _lostText;
    [SerializeField] TextMeshProUGUI _wordText;

    GameController _gameController;

    void Start()
    {
        _gameController = GetComponent<GameController>();
        _gameOverObject.SetActive(false);
        _gameObject.SetActive(true);
    }

    void OnEnable()
    {
        _gameController.OnGameEnd += OpenEndScreen;
    }

    void OnDisable()
    {
        _gameController.OnGameEnd -= OpenEndScreen;
    }

    void OpenEndScreen(bool won, string word)
    {
        _lostText.gameObject.SetActive(!won);
        _wonText.gameObject.SetActive(won);

        _gameOverObject.SetActive(true);
        _gameObject.SetActive(false);

        _wordText.gameObject.SetActive(true);
        _wordText.text = $"The word was:\n{word.ToUpper()}";
    }
}
