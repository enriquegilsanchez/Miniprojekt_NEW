using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public int Score = 0;
    public float PlayerHealth;

    public Text TXTScore;
    public Text Timer_Display;
    public float Minutes;
    public float Seconds;
    public float Timer;
    public Slider HealthBar;
    private GameObject Player;
    private PlayerController pc;

    public GameObject GameOver;
    public GameObject Menu;

    public bool MenuIsOpen = false;
    public bool gameover = false;

    /* public AudioSource AudioSource;
    public AudioClip levelsound;
    public AudioClip gameOversound; */
    public GameObject music;
    public GameObject gameoverMusic;

    void Start()
    {
        Score = 0;
        GameOver.SetActive(false);
        Player = GameObject.FindGameObjectWithTag("Player");
        pc = Player.GetComponent<PlayerController>();
        PlayerHealth = 5;
        HealthBar.maxValue = PlayerHealth;
        HealthBar.value = PlayerHealth;
        Debug.Log("playerHealth:" + PlayerHealth);
        Debug.Log("healthbarvalue:" + HealthBar.value);
        Menu.SetActive(false);
        music.SetActive(true);
        gameover = false;
    }

    void Update()
    {
        PlayerHealth = pc.health;
        // Debug.Log("playerHealth:" + PlayerHealth);

        HealthBar.value = PlayerHealth;
        // Debug.Log("healthbarvalue:" + HealthBar.value);
        TXTScore.text = Score.ToString();
        Timer += Time.deltaTime;
        Minutes = Mathf.FloorToInt(Timer / 60);
        Seconds = Mathf.FloorToInt(Timer % 60);
        Timer_Display.text = string.Format("{0:00}:{1:00}", Minutes, Seconds);
        if (PlayerHealth <= 0)
        {
            GameOver.SetActive(true);
            gameover = true;
            Time.timeScale = 0;
            music.SetActive(false);
            gameoverMusic.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuIsOpen)
            {
                Menu.SetActive(false);
                Time.timeScale = 1f;
                MenuIsOpen = false;
            }
            else
            {
                Menu.SetActive(true);
                Time.timeScale = 0f;
                MenuIsOpen = true;
            }
        }
    }

    public void BTNRestart()
    {
        Time.timeScale = 1;
        Score = 0;
        PlayerHealth = 5;
        HealthBar.maxValue = PlayerHealth;
        HealthBar.value = PlayerHealth;
        gameoverMusic.SetActive(false);
        GameOver.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GetHp()
    {
        Debug.Log("Hp: " + PlayerHealth);
    }

    public void ChangeScore(int val)
    {
        Score += val;
    }
}
