using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance;
    public static Manager Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<Manager>();
            }

            return instance;
        }
    }

    float scores;
    int idPlayer;
    int healthCount;
    bool gameStarted;

    bool IsPause;

    float currentSpeed;
    float targetSpeed;
    float velocity = 0.0f;

    float currentRoadSpeed;
    float targetRoadSpeed;
    float roadVelocity = 0.0f;

    const int maxLines = 8;

    const float xSpeedMultiplayer = 0.4f;
    const float initOffsetY = 12.0f;
    const float obstacleOffsetY = 12.0f;
    const float roadOffsetY = 13.64f;
    const float speedMultiplayer = 0.08f;
    const float stopSmoothTime = 1;

    const string savekey = "leaders";

    [Space(10)]
    [SerializeField] Transform obstacles;
    [SerializeField] Transform road;

    [Space(10)]
    [SerializeField] Text timerText;
    [SerializeField] Text scoresText;

    [Space(10)]
    [SerializeField] GameObject menu;
    [SerializeField] GameObject game;
    [SerializeField] GameObject results;

    [Space(10)]
    [SerializeField] GameObject shop;
    [SerializeField] GameObject leaderboard;

    [Space(10)]
    [SerializeField] Transform healthBar;

    [Space(10)]
    [SerializeField] Sprite full;
    [SerializeField] Sprite empty;

    [Space(10)]
    [SerializeField] Text leadersText;

    [Space(10)]
    [SerializeField] Player player;

    const int maxAcceleration = 10;
    const int maxHandling = 10;

    [Space(10)]
    [SerializeField] Image playerIcon;
    [SerializeField] Text playerNameText;
    [SerializeField] Text playerMaxSpeed;
    [SerializeField] Text playerAcceleration;
    [SerializeField] Text playerHandling;
    [SerializeField] Image menuPlayerIcon;

    [Space(10)]
    [SerializeField] PlayerData[] playerDatas;

    [Space(10)]
    [SerializeField] Leaderboard_data leaderboard_Data;

    private void Start()
    {
        idPlayer = 0;
        SetPlayer(0);

        road.gameObject.SetActive(false);

        menuPlayerIcon.sprite = playerDatas[0].playerSprite;
        menuPlayerIcon.SetNativeSize();

        LoadResults();

        game.SetActive(false);
        results.SetActive(false);
        shop.SetActive(false);
        leaderboard.SetActive(false);
        menu.SetActive(true);

        player.SetAlive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(game.activeSelf && gameStarted)
            {
                road.gameObject.SetActive(false);

                gameStarted = false;
                ResetObstacles();

                player.SetAlive(false);
                game.SetActive(false);
                menu.SetActive(true);

                SaveResult();
                return;
            }
        }

        if(!gameStarted)
        {
            return;
        }

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocity, stopSmoothTime);
        currentRoadSpeed = Mathf.SmoothDamp(currentRoadSpeed, targetRoadSpeed, ref roadVelocity, stopSmoothTime);

        UpdateScore();

        UpdateObstaclePositions();
        UpdateRoadPositions();
    }

    void ResetObstacles()
    {
        foreach(Transform t in obstacles)
        {
            t.localPosition = new Vector2(0, initOffsetY + (obstacleOffsetY * t.GetSiblingIndex()));
        }
    }

    void ResetRoad()
    {
        foreach (Transform t in road)
        {
            t.localPosition = new Vector2(0,  roadOffsetY * t.GetSiblingIndex());
        }
    }

    void LoadResults()
    {
        leaderboard_Data = PlayerPrefs.HasKey(savekey) ? JsonUtility.FromJson<Leaderboard_data>(PlayerPrefs.GetString(savekey)) : new Leaderboard_data(maxLines);
        var newlist = leaderboard_Data.result.OrderByDescending(i => i);
        leaderboard_Data.result = newlist.ToList();

        leadersText.text = string.Empty;
        for(int i = 0; i < maxLines; i++)
        {
            leadersText.text += string.Format("{0}.  {1}  SECONDS", i + 1, leaderboard_Data.result[i]);
            if (i < maxLines - 1)
            {
                leadersText.text += "\n";
            }
        }
    }

    void SaveResult()
    {
        leaderboard_Data.result.Add(Mathf.FloorToInt(scores));

        var newlist = leaderboard_Data.result.OrderByDescending(i => i);
        leaderboard_Data.result = newlist.ToList();

        string data = JsonUtility.ToJson(leaderboard_Data);
        PlayerPrefs.SetString(savekey, data);
        PlayerPrefs.Save();
        
    }

    float GetRandY()
    {
        return Random.Range(-2.68f, 2.68f);
    }

    void UpdateObstaclePositions()
    {
        foreach(Transform t in obstacles)
        {
            t.Translate(speedMultiplayer * currentSpeed * Time.deltaTime * Vector2.down);
            if(t.position.y < -initOffsetY)
            {
                foreach(Transform car in t)
                {
                    if(!car.gameObject.activeSelf)
                    {
                        car.gameObject.SetActive(true);
                    }
                }

                t.localPosition = new Vector2(0, obstacleOffsetY + GetLastObstacleSetPos().y);
                t.SetAsLastSibling();
            }
        }
    }

    void UpdateRoadPositions()
    {
        foreach (Transform t in road)
        {
            t.Translate(speedMultiplayer * currentRoadSpeed * Time.deltaTime * Vector2.down);
            if (t.position.y < -initOffsetY)
            {
                t.localPosition = new Vector2(0, roadOffsetY + GetLastRoadPos().y);
                t.SetAsLastSibling();
            }
        }
    }

    Vector3 GetLastObstacleSetPos()
    {
        return obstacles.GetChild(obstacles.childCount - 1).localPosition;
    }

    Vector3 GetLastRoadPos()
    {
        return road.GetChild(road.childCount - 1).localPosition;
    }

    public void StartGame()
    {
        scores = 0;
        UpdateScore();

        SetPlayer(0);

        currentSpeed = 0;
        targetSpeed = playerDatas[idPlayer].maxSpeed;

        currentSpeed = 0.0f;
        targetRoadSpeed = playerDatas[idPlayer].maxSpeed;

        road.gameObject.SetActive(true);

        healthBar.gameObject.SetActive(false);
        scoresText.gameObject.SetActive(false);

        foreach(Transform t in healthBar)
        {
            t.GetComponent<Image>().sprite = full;
        }

        healthCount = healthBar.childCount;

        results.SetActive(false);
        menu.SetActive(false);
        game.SetActive(true);
        StartCoroutine(nameof(Timer));
    }

    public void Pause()
    {
        IsPause = !IsPause;
        Time.timeScale = IsPause ? 0 : 1;
    }

    public void OpenShop()
    {
        menu.SetActive(false);
        shop.SetActive(true);
    }

    public void OpenLeaderboard()
    {
        LoadResults();

        menu.SetActive(false);
        leaderboard.SetActive(true);
    }

    public void Back()
    {
        if(shop.activeSelf)
        {
            shop.SetActive(false);
            menu.SetActive(true);
            return;
        }
        else if(leaderboard.activeSelf)
        {
            leaderboard.SetActive(false);
            menu.SetActive(true);
            return;
        }
    }

    public void UpdateScore()
    {
        scores += Time.deltaTime;
        scoresText.text = string.Format("TIME:{0:000} SECONDS", scores);
    }

    public void SetPlayer(int dir)
    {
        idPlayer += dir;
        if(idPlayer < 0)
        {
            idPlayer = playerDatas.Length - 1;
        }
        else if(idPlayer > playerDatas.Length - 1)
        {
            idPlayer = 0;
        }

        playerNameText.text = playerDatas[idPlayer].playerName;
        playerIcon.sprite = playerDatas[idPlayer].playerSprite;
        playerIcon.SetNativeSize();

        playerMaxSpeed.text = string.Format("MAX.SPEED:    {0}KMH", playerDatas[idPlayer].maxSpeed);
        playerAcceleration.text = string.Format("ACCELERATION:   {0}/{1}", playerDatas[idPlayer].acceleration, maxAcceleration);
        playerAcceleration.text = string.Format("HANDLING:       {0}/{1}", playerDatas[idPlayer].handling, maxHandling);
    }

    public void MovePlayer(int dir)
    {
        if(!gameStarted)
        {
            return;
        }

        float dX = xSpeedMultiplayer * dir * playerDatas[idPlayer].handling * Time.deltaTime;
        player.Move(dX);
    }

    public void Stopping(bool _isStoping)
    {
        if (!gameStarted)
        {
            return;
        }

        targetSpeed = _isStoping ? playerDatas[idPlayer].maxSpeed / 2.0f : playerDatas[idPlayer].maxSpeed;
        targetRoadSpeed = _isStoping ? 0 : playerDatas[idPlayer].maxSpeed;
    }

    public void Confirm()
    {
        player.SetSprite(playerDatas[idPlayer].playerSprite);

        menuPlayerIcon.sprite = playerDatas[idPlayer].playerSprite;
        menuPlayerIcon.SetNativeSize();
    }

    public void TakeDamage()
    {
        healthCount--;
        if(healthCount <= 0)
        {
            SaveResult();

            gameStarted = false;
            game.SetActive(false);
            results.SetActive(true);

            player.SetAlive(false);
            ResetObstacles();
        }

        healthBar.GetChild(healthCount).GetComponent<Image>().sprite = empty;
    }

    IEnumerator Timer()
    {
        timerText.gameObject.SetActive(true);
        timerText.text = "GET READY";
        yield return new WaitForSeconds(0.25f);

        int timerCount = 3;

        for(int i = timerCount; i > 0; i--)
        {
            timerText.text = i.ToString();
            yield return new WaitForSeconds(0.25f);
        }

        timerText.text = "GO!";
        yield return new WaitForSeconds(0.25F);
        timerText.gameObject.SetActive(false);

        player.SetAlive(true);
        ResetObstacles();

        healthBar.gameObject.SetActive(true);
        scoresText.gameObject.SetActive(true);

        gameStarted = true;
    }

    [System.Serializable]
    public class Leaderboard_data
    {
        public List<int> result = new List<int>();

        public Leaderboard_data(int maxCount)
        {
            for(int i = 0; i < maxCount; i++)
            {
                result.Add(0);
            }
        }
    }
}
