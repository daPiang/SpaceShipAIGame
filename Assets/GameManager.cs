using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float gameDuration = 30f;
    private float timeLeft;
    public Text timerText;
    public Text victoryText;
    public Text loseText;
    public Canvas regenCanvas;

    private void OnEnable() {
        Asteroid.LoseEvent += Lose;
    }

    private void OnDisable() {
        Asteroid.LoseEvent -= Lose;
    }

    void Start()
    {
        Time.timeScale = 1;
        timeLeft = gameDuration;
        victoryText.enabled = false;
        loseText.enabled = false;
        regenCanvas.enabled = false;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            Victory();
        }

        timerText.text = Mathf.Round(timeLeft) + "s";
    }

    void Victory()
    {
        ShowButton();
        victoryText.enabled = true;
        PauseGame();
    }

    void Lose()
    {
        ShowButton();
        loseText.enabled = true;
        PauseGame();
    }

    void ShowButton() {
        regenCanvas.enabled = true;
    }

    void PauseGame() {
        Time.timeScale = 0;  // Pause the game
    }

    public void Regenerate() {
        SceneManager.LoadScene("SampleScene");
    }
}
