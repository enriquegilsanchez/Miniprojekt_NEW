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

    public Room currentRoom;

    public bool[] connectedDoors = { false, false, false, false };

    public Room spawnRoom;
    public Room bossRoom;

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    public GameObject archerPrefab;

    [SerializeField]
    public GameObject bossPrefab;

    void Start()
    {
        Score = 0;
        GameOver.SetActive(false);
        Player = GameObject.FindGameObjectWithTag("Player");
        pc = Player.GetComponent<PlayerController>();
        PlayerHealth = 20;
        HealthBar.maxValue = PlayerHealth;
        HealthBar.value = PlayerHealth;
        Debug.Log("playerHealth:" + PlayerHealth);
        Debug.Log("healthbarvalue:" + HealthBar.value);
        Menu.SetActive(false);
        music.SetActive(true);
        gameover = false;
        currentRoom = null;
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
        PlayerHealth = 20;
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
        currentRoom.numberOfEnemies--;
        if (currentRoom.numberOfEnemies == 0)
        {
            currentRoom.isCleared = true;
            ActivateDoors();
        }
    }

    public void SetCurrentRoom(Room room)
    {
        currentRoom = room;
        Debug.Log("current room is: " + room.ToString());

        if (!currentRoom.isCleared)
        {
            DeactivateDoors();
            SpawnEnemies();
        }
    }

    public void SetSpawnRoom(Room room)
    {
        spawnRoom = room;
    }

    public void SetBossRoom(Room room)
    {
        bossRoom = room;
    }

    public void SpawnEnemies()
    {
        if (currentRoom == spawnRoom)
            return;
        if (currentRoom == bossRoom)
        {
            GameObject boss = Instantiate(bossPrefab, bossRoom.rect.center, Quaternion.identity);
            currentRoom.numberOfEnemies = 1;
        }
        else
        {
            // Set spawn range
            var spawnMinX = currentRoom.rect.x + 3;
            var spawnMaxX = currentRoom.rect.xMax - 3;
            var spawnMinY = currentRoom.rect.y + 3;
            var spawnMaxY = currentRoom.rect.yMax - 3;
            var xRange = 0f;
            var yRange = 0f;

            // Determine enemy number and archer number
            var numberOfEnemies = Random.Range(2, 6);
            var checkArcher = 50;
            var archerIncluded = Random.Range(0, 100) > checkArcher ? true : false;

            for (var i = 0; i < numberOfEnemies; i++)
            {
                // Determine Random spawn position in range
                xRange = Random.Range(spawnMinX, spawnMaxX);
                yRange = Random.Range(spawnMinY, spawnMaxY);
                // spawn archer, max 2
                if (archerIncluded)
                {
                    Instantiate(archerPrefab, new Vector2(xRange, yRange), Quaternion.identity);
                    checkArcher += 30;
                    archerIncluded = Random.Range(0, 100) > checkArcher ? true : false;
                    continue;
                }
                Instantiate(enemyPrefab, new Vector2(xRange, yRange), Quaternion.identity);
            }
            currentRoom.numberOfEnemies = numberOfEnemies;
        }
    }

    public void DeactivateDoors()
    {
        // Remember connected doors
        if (currentRoom.leftDoor.activeSelf)
            connectedDoors[0] = true;
        if (currentRoom.rightDoor.activeSelf)
            connectedDoors[1] = true;
        if (currentRoom.upperDoor.activeSelf)
            connectedDoors[2] = true;
        if (currentRoom.lowerDoor.activeSelf)
            connectedDoors[3] = true;

        // deactivate all doors
        currentRoom.leftDoor.SetActive(false);
        currentRoom.rightDoor.SetActive(false);
        currentRoom.upperDoor.SetActive(false);
        currentRoom.lowerDoor.SetActive(false);
    }

    public void ActivateDoors()
    {
        if (connectedDoors[0])
            currentRoom.leftDoor.SetActive(true);
        if (connectedDoors[1])
            currentRoom.rightDoor.SetActive(true);
        if (connectedDoors[2])
            currentRoom.upperDoor.SetActive(true);
        if (connectedDoors[3])
            currentRoom.lowerDoor.SetActive(true);
    }
}
