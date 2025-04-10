using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject victoryScreen;
    public GameObject loseScreen;

    private bool gameStarted = false;
    private bool gameEnded = false;
    private bool playerDead = false;
    private int enemiesLeft;

    void Start()
    {
        Time.timeScale = 0f;
        startScreen.SetActive(true);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesLeft = enemies.Length;
    }

    void Update()
    {
        if (!gameStarted && Input.anyKeyDown)
        {
            StartGame();
        }

        if (gameEnded && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Game exited");
        }

        if (playerDead && Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;
        startScreen.SetActive(false);
    }

    public void EnemyKilled()
    {
        enemiesLeft--;

        if (enemiesLeft <= 0)
        {
            Victory();
        }
    }

    void Victory()
    {
        gameEnded = true;
        Time.timeScale = 0f;
        victoryScreen.SetActive(true);
    }

    public void PlayerDied()
    {
        Debug.Log("Player died!");
        playerDead = true;
        gameEnded = true;

        Time.timeScale = 0f;
        loseScreen.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
