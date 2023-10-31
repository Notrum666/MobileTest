using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField]
    public GameObject ObstaclePrefab;
    [SerializeField]
    private List<Mesh> meshes = new List<Mesh>();
    [SerializeField]
    public float FlySpeed, CurFlySpeed;
    [SerializeField]
    private float acceleration, spawnDistance, spawnInterval, maxSpeed;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject RedOverlay;
    [SerializeField]
    GameObject ScoreText, DeathScoreText;
    [SerializeField]
    GameObject HighscoreText, DeathHighscoreText;

    [SerializeField]
    GameObject MenuPanel, GamePanel, DeathPanel;

    int score = 0;
    int highscore = 0;
    bool isGameActive = false;
    public bool IsGameActive { get => isGameActive; }
    private List<GameObject> obstacles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore");
        HighscoreText.GetComponent<TextMeshProUGUI>().SetText("high score: " + highscore.ToString());
    }

    void startGame()
    {
        isGameActive = true;
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
        obstacles.Clear();
        CurFlySpeed = FlySpeed;
        score = 0;
        ScoreText.GetComponent<TextMeshProUGUI>().SetText(score.ToString());
    }
    void stopGame()
    {
        isGameActive = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isGameActive)
            return;

        CurFlySpeed = Mathf.Min(maxSpeed, CurFlySpeed + acceleration * Time.deltaTime);

        Ray ray = new Ray(player.transform.position + Vector3.up * 2.0f, Vector3.down);

        if (Physics.Raycast(ray, CurFlySpeed * Time.deltaTime + 2.0f))
        {
            Destroy(obstacles.First());
            obstacles.RemoveAt(0);
            OnDeath();
            return;
        }

        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            obstacles[i].transform.position += Vector3.up * CurFlySpeed * Time.deltaTime;
            if (obstacles[i].transform.position.y >= 0)
            {
                Destroy(obstacles[i]);
                obstacles.RemoveAt(i);
                score++;
                ScoreText.GetComponent<TextMeshProUGUI>().SetText(score.ToString());
            }
        }

        float spawnLocation = obstacles.Count > 0 ? obstacles.Last().transform.position.y - spawnInterval : - spawnDistance;
        Quaternion spawnRotation = obstacles.Count > 0 ? obstacles.Last().transform.rotation * new Quaternion(0, Mathf.Sin(Mathf.PI / 4.0f), 0, Mathf.Cos(Mathf.PI / 4.0f)) : Quaternion.identity;
        while (spawnLocation >= -spawnDistance)
        {
            GameObject newObstacle = Instantiate(ObstaclePrefab, new Vector3(0, spawnLocation, 0), spawnRotation);
            Mesh curMesh = meshes[Random.Range(0, meshes.Count)];
            newObstacle.GetComponent<MeshFilter>().mesh = curMesh;
            newObstacle.GetComponent<MeshCollider>().sharedMesh = curMesh;
            obstacles.Add(newObstacle);

            spawnLocation -= spawnInterval;
            spawnRotation *= new Quaternion(0, Mathf.Sin(Mathf.PI / 4.0f), 0, Mathf.Cos(Mathf.PI / 4.0f));
        }
    }

    public void OnStartClicked()
    {
        MenuPanel.SetActive(false);
        DeathPanel.SetActive(false);
        GamePanel.SetActive(true);

        startGame();
    }
    void OnDeath()
    {
        DeathScoreText.GetComponent<TextMeshProUGUI>().SetText("score: " + score.ToString());
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("highscore", highscore);
        }

        DeathHighscoreText.GetComponent<TextMeshProUGUI>().SetText("high score: " + highscore.ToString());

        GamePanel.SetActive(false);
        DeathPanel.SetActive(true);

        stopGame();
    }
}
