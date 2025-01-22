using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    public GameObject archer;

    [SerializeField]
    private float minSpawnTime;

    [SerializeField]
    private float maxSpawnTime;

    private float timeUntilSpawn;

    private float timer = 0f;

    void Awake()
    {
        SetTimeUntilSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        timeUntilSpawn -= Time.deltaTime;

        if (timeUntilSpawn <= 0)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            SetTimeUntilSpawn();
        }

        if (timer >= 10)
        {
            Instantiate(archer, transform.position, Quaternion.identity);
            timer = 0f;
        }
    }

    void SetTimeUntilSpawn()
    {
        timeUntilSpawn = Random.Range(minSpawnTime, maxSpawnTime);
    }
}
