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

    private void StartGame()
    {
        this.gameObject.SetActive(false);
        hudController.gameObject.SetActive(true);
        ActivatePlayerWithDelay();
    }

    private void ShowHowToPlay()
    {

    }

    private void ReturnToMainMenu()
    {

    }

    void Start()
    {
        hudController.gameObject.SetActive(false);
        player.SetActive(false);
        obstacleManager.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ActivatePlayerWithDelay()
    {
        yield return new WaitForSeconds(2f);
        player.SetActive(true);
        obstacleManager.gameObject.SetActive(true);
    }
}
