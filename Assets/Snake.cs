
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Snake : MonoBehaviour {

    public Transform bodyPrefab;
    public Transform foodPrefab;
    public Transform wallPrefab;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI gameOverText;

    List<Transform> body = new List<Transform>();
    List<Transform> food = new List<Transform>();
    List<Transform> wall = new List<Transform>();

    Vector2 direction;
    public float speed    = 10.0f; 
    public float cellSize = 0.3f;

    public int initialFoods = 10;

    public float spawnFoodIntervalMin = 2.0f;
    public float spawnFoodIntervalMax = 4.0f;

    public int maxFoods = 20;

    Vector2 cellIndex = Vector2.zero;

    float spawnFoodTime  = 0;
    float changeCellTime = 0;

    int score = 0;
    int highScore = 0;

    bool gameOver = false;

    void Start() {

        gameOverText.gameObject.SetActive(false);

        direction = Vector2.up;

        CreateWalls();

        for(int i = 0; i < initialFoods; i++) SpawnFood();

    }

    void Update() {

        if(gameOver) {
            if(Input.GetKeyDown(KeyCode.R)) Restart();
            return;
        }

        ChangeDirection();

        Move();

        EatFood();

        CheckBodyCollisions();
        CheckWallCollision();

        SpawnFoodByTime();

    }

    void ChangeDirection() {

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(input.y == -1) {
            direction = Vector2.down;
        } else if(input.y == 1) {
            direction = Vector2.up;
        } else if(input.x == -1) {
            direction = Vector2.left;
        } else if(input.x == 1) {
            direction = Vector2.right;
        }

    }

    void Move() {

        if(Time.time > changeCellTime) {

            for(int i = body.Count - 1; i > 0; i--) {
                body[i].position = body[i - 1].position;
            }
            if(body.Count > 0)
                body[0].position = (Vector2)transform.position;

            transform.position += (Vector3)direction * cellSize;

            changeCellTime = Time.time + 1 / speed;

            cellIndex = transform.position / cellSize;

        }

    }

    void GrowBody() {

        Vector2 position = transform.position;
        if(body.Count != 0) 
            position = body[body.Count - 1].position;

        body.Add(Instantiate(bodyPrefab, position, Quaternion.identity).transform);

    }

    void EatFood() {
        for(int i = 0; i < food.Count; ++i) {
            Vector2 index = food[i].position / cellSize;
            if(Mathf.Abs(index.x - cellIndex.x) < 0.00001f && Mathf.Abs(index.y - cellIndex.y) < 0.00001f) { 
                Destroy(food[i].gameObject);
                food.Remove(food[i]);
                GrowBody();
                UpdateScore();
            }
        }
    }

    void SpawnFood() {
        float x = Random.Range(-23, 23) * cellSize;
        float y = Random.Range(-13, 11) * cellSize;
        Vector2 randomPosition = new Vector2(x, y);
        food.Add(Instantiate(foodPrefab, randomPosition, Quaternion.identity).transform);
    }

    void SpawnFoodByTime() {

        if(food.Count == maxFoods) return;

        if(Time.time > spawnFoodTime) {
            SpawnFood();
            spawnFoodTime = Time.time + Random.Range(spawnFoodIntervalMin, spawnFoodIntervalMax);
        }

    }

    void UpdateScore() {
        score++;
        scoreText.text = "SCORE: " + score.ToString();
    }

    void CreateWalls() {

        int cellX = -24;
        int cellY = 11;

        int height = 25;

        float horizontal = cellX * cellSize;
        float vertical   = cellY * cellSize;
        
        for(int i = 0; i < (int)Mathf.Abs((horizontal * 2) / cellSize)+1; ++i) {
            Vector2 top    = new Vector3(horizontal + cellSize * i, vertical);
            Vector2 bottom = new Vector3(horizontal + cellSize * i, vertical - height * cellSize);
            wall.Add(Instantiate(wallPrefab, top, Quaternion.identity).transform);
            wall.Add(Instantiate(wallPrefab, bottom, Quaternion.identity).transform);
        }

        for(int i = 0; i < height; ++i) {
            Vector2 right = new Vector3(horizontal, vertical - cellSize * i);
            Vector2 left  = new Vector3(-horizontal, vertical - cellSize * i);
            wall.Add(Instantiate(wallPrefab, right, Quaternion.identity).transform);
            wall.Add(Instantiate(wallPrefab, left, Quaternion.identity).transform);
        }

    }

    void CheckBodyCollisions() {

        if(body.Count < 3) return;

        for(int i = 0; i < body.Count; ++i) {
            Vector2 index = body[i].position / cellSize;
            if(Mathf.Abs(index.x - cellIndex.x) < 0.00001f && Mathf.Abs(index.y - cellIndex.y) < 0.00001f) {
                GameOver();
                break;
            }
        }
    }

    void CheckWallCollision() {
        for(int i = 0; i < wall.Count; ++i) {
            Vector2 index = wall[i].position / cellSize;
            if(Mathf.Abs(index.x - cellIndex.x) < 0.00001f && Mathf.Abs(index.y - cellIndex.y) < 0.00001f) {
                GameOver();
                break;
            }
        }
    }

    void GameOver() {
        gameOver = true;
        gameOverText.gameObject.SetActive(true);
    }

    void Restart() {

        gameOver = false;
        gameOverText.gameObject.SetActive(false);

        // Update high score and reset score
        if(score > highScore) highScore = score;
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
        score = 0;
        scoreText.text = "SCORE: " + score.ToString();

        // Destroy foods and snake body
        for(int i = 0; i < body.Count; ++i) {
            Destroy(body[i].gameObject);
        }
        body.Clear();

        for(int i = 0; i < food.Count; ++i) {
            Destroy(food[i].gameObject);
        }
        food.Clear();

        // Reset snake position
        transform.position = Vector3.zero;

        // Spawn new foods
        for(int i = 0; i < initialFoods; i++) SpawnFood();

    }

}
