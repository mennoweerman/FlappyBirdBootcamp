using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    private Label scoreLabel;
    private Label gameOverLabel;
    private Label highScoreLabel;
    private Button restartButton;

    [Header("Score Tracking")]
    private int score = 0;
    private int highScore;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        scoreLabel = root.Q<Label>("score-label");
        gameOverLabel = root.Q<Label>("gameover-label");
        highScoreLabel = root.Q<Label>("highscore-label");
        restartButton = root.Q<Button>("restart-button");

        // Load saved high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreLabel.text = $"High Score: {highScore}";

        // Connect restart button
        restartButton.clicked += RestartGame;

        // Reset UI elements on initialization
        ResetUI();
    }

    private void ResetUI()
    {
        // Reset score
        score = 0;
        if (scoreLabel != null) scoreLabel.text = $"Score: {score}";
        
        // Hide game over elements
        if (gameOverLabel != null) gameOverLabel.visible = false;
        if (restartButton != null) restartButton.visible = false;
        
        // Show score and high score
        if (scoreLabel != null) scoreLabel.visible = true;
        if (highScoreLabel != null) highScoreLabel.visible = true;
    }

    void Start()
    {
        // Reset UI on start
        ResetUI();
    }

    public void UpdateScore(int newScore)
    {
        score = newScore;
        if (scoreLabel != null)
        {
            scoreLabel.text = $"Score: {score}";
        }
    }

    public void GameOver()
    {
        // Show Game Over elements via visible property
        if (gameOverLabel != null)
        {
            gameOverLabel.visible = true;
        }
        
        if (restartButton != null)
        {
            restartButton.visible = true;
        }

        // Check & update high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        highScoreLabel.text = $"High Score: {highScore}";
        if (highScoreLabel != null)
        {
            highScoreLabel.visible = true;
        }

        // Pause game
        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        // Resume time
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void StartGame()
    {
        score = 0;
        
        // Update score display
        if (scoreLabel != null)
        {
            scoreLabel.text = $"Score: {score}";
        }
        
        // Hide game over elements
        if (gameOverLabel != null)
        {
            gameOverLabel.visible = false;
        }
        
        if (restartButton != null)
        {
            restartButton.visible = false;
        }
    }
}
