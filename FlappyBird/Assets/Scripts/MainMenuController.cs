using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public ObstacleManager obstacleManager;
    public HUDController hudController;
    private UIDocument _uiDocument;
    private Button _startButton;
    private Button _howToPlayButton;
    private Button _returnButton;
    private Label _howToPlayLabel;
    private Label _titleLabel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        _startButton = root.Q<Button>("StartGameButton");
        _howToPlayButton = root.Q<Button>("HowToPlayButton");
        _returnButton = root.Q<Button>("ReturnButton");
        _howToPlayLabel = root.Q<Label>("HowToPlayText");
        _titleLabel = root.Q<Label>("GameTitle");

        _startButton.clicked += StartGame;
        _howToPlayButton.clicked += ShowHowToPlay;
        _returnButton.clicked += ReturnToMainMenu;

        // Verberg bij start
        _returnButton.style.display = DisplayStyle.None;
        _howToPlayLabel.style.display = DisplayStyle.None;
    }
    
    void Start()
    {
        player.SetActive(false);
        obstacleManager.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        gameObject.GetComponent<UIDocument>().enabled = false;
        StartCoroutine(ActivatePlayerWithDelay());
    }

    private void ShowHowToPlay()
    {
        _startButton.style.display = DisplayStyle.None;
        _howToPlayButton.style.display = DisplayStyle.None;

        _howToPlayLabel.style.display = DisplayStyle.Flex;
        _returnButton.style.display = DisplayStyle.Flex;
    }

    private void ReturnToMainMenu()
    {
        _startButton.style.display = DisplayStyle.Flex;
        _howToPlayButton.style.display = DisplayStyle.Flex;

        _howToPlayLabel.style.display = DisplayStyle.None;
        _returnButton.style.display = DisplayStyle.None;
    }

    private IEnumerator ActivatePlayerWithDelay()
    {
        yield return new WaitForSeconds(2f);
        player.SetActive(true);
        obstacleManager.gameObject.SetActive(true);
        hudController.gameObject.SetActive(true);
    }
}
