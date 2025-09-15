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

        // Verberg bij start
        gameOverLabel.style.display = DisplayStyle.None;
        restartButton.style.display = DisplayStyle.None;
    }

    public void UpdateScore(int newScore)
    {
        score = newScore;
        scoreLabel.text = $"Score: {score}";
    }

    public void GameOver()
    {
        // Show Game Over
        gameOverLabel.style.display = DisplayStyle.Flex;

        // Check & update high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        highScoreLabel.text = $"High Score: {highScore}";
        highScoreLabel.style.display = DisplayStyle.Flex;
        restartButton.style.display = DisplayStyle.Flex;

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
}
