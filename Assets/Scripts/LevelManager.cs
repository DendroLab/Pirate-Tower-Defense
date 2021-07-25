using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public bool IsOver { get; private set; }

    // Fungsi Singleton
    private static LevelManager _instance = null;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
            }

            return _instance;
        }
    }

    [SerializeField] private Transform _towerUIParent;
    [SerializeField] private GameObject _towerUIPrefab;

    [SerializeField] private Tower[] _towerPrefabs;
    [SerializeField] private Enemy[] _enemyPrefabs;
    [SerializeField] private EnemyPath[] _enemyPaths;
    [SerializeField] private float _spawnDelay = 5f;

    [SerializeField] private int _maxLives = 3;
    [SerializeField] private int _totalEnemy = 15;
    [SerializeField] private GameObject _panel;
    //[SerializeField] private Text _statusInfo;
    [SerializeField] private Text _livesInfo;
    [SerializeField] private Text _totalEnemyInfo;

    [SerializeField] private Text _coinInfo;
    [SerializeField] private int _coin = 100;
    [SerializeField] private SpriteRenderer _youWin;
    [SerializeField] private SpriteRenderer _youLose;

    [SerializeField] private Button _Next;

    private List<Tower> _spawnedTowers = new List<Tower>();
    private List<Enemy> _spawnedEnemies = new List<Enemy>();
    private List<Bullet> _spawnedBullets = new List<Bullet>();

    private float _runningSpawnDelay;
    private int _currentLives;
    private int _enemyCounter;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentLives(_maxLives);
        SetTotalEnemy(_totalEnemy);
        GameResources.Coin = _coin;

        InstantiateAllTowerUI();
    }

    // Update is called once per frame
    private void Update()
    {
        SetTotalCoin();

        // Jika menekan tombol R, fungsi restart akan terpanggil
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (IsOver)
        {
            return;
        }

        // Counter untuk spawn enemy dalam jeda waktu yang ditentukan
        // Time.unscaledDeltaTime adalah deltaTime yang independent, tidak terpengaruh oleh apapun kecuali game object itu sendiri,
        // jadi bisa digunakan sebagai penghitung waktu
        _runningSpawnDelay -= Time.unscaledDeltaTime;

        if (_runningSpawnDelay <= 0f)
        {
            SpawnEnemy();
            _runningSpawnDelay = _spawnDelay;
        }

        foreach (Tower tower in _spawnedTowers)
        {
            tower.CheckNearestEnemy(_spawnedEnemies);
            //Debug.Log(tower.gameObject.name);
            if (tower.gameObject.name != "Tower3(Clone)")
            {
                tower.SeekTarget();
            }
            tower.ShootTarget();
        }

        foreach (Enemy enemy in _spawnedEnemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                continue;
            }

            // Kenapa nilainya 0.1? Karena untuk lebih mentoleransi perbedaan posisi,
            // akan terlalu sulit jika perbedaan posisinya harus 0 atau sama persis
            if (Vector2.Distance(enemy.transform.position, enemy.TargetPos) < 0.1f)
            {
                enemy.SetCurrentPointInPath(enemy.CurrentPointInPath + 1);
                List<Transform> currentEnemyPath = _enemyPaths[enemy.ChoosenPathIndex].Points;
                if (enemy.CurrentPointInPath < currentEnemyPath.Count)
                {
                    enemy.SetTargetPos(currentEnemyPath[enemy.CurrentPointInPath].position);
                }
                else
                {
                    ReduceLives(1);
                    enemy.gameObject.SetActive(false);
                }
            }
            else
            {
                enemy.MoveToTarget();
            }
        }
    }

    // Menampilkan seluruh tower yang tersedia pada UI Tower Selection
    private void InstantiateAllTowerUI()
    {
        foreach (Tower tower in _towerPrefabs)
        {
            GameObject newTowerUIObj = Instantiate(_towerUIPrefab.gameObject, _towerUIParent);
            TowerUI newTowerUI = newTowerUIObj.GetComponent<TowerUI>();

            newTowerUI.SetTowerPrefab(tower);
            newTowerUI.transform.name = tower.name;
        }
    }

    // Mendaftarkan Tower yang di-spawn agar bisa dikontrol oleh LevelManager
    public void RegisterSpawnedTower(Tower tower)
    {
        _spawnedTowers.Add(tower);
    }

    private void SpawnEnemy()
    {
        SetTotalEnemy(--_enemyCounter);

        if (_enemyCounter < 0)
        {
            bool isAllEnemyDestroyed = _spawnedEnemies.Find(e => e.gameObject.activeSelf) == null;

            if (isAllEnemyDestroyed)
            {
                SetGameOver(true);
            }

            return;
        }

        int randomIndex = Random.Range(0, _enemyPrefabs.Length);
        string enemyIndexString = (randomIndex + 1).ToString();

        GameObject newEnemyObj = _spawnedEnemies.Find(
            e => !e.gameObject.activeSelf && e.name.Contains(enemyIndexString)
        )?.gameObject;

        if (newEnemyObj == null)
        {
            newEnemyObj = Instantiate(_enemyPrefabs[randomIndex].gameObject);
        }

        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();
        if (!_spawnedEnemies.Contains(newEnemy))
        {
            _spawnedEnemies.Add(newEnemy);
        }

        newEnemy.ChoosenPathIndex = Random.Range(0, _enemyPaths.Length);
        newEnemy.transform.position = _enemyPaths[newEnemy.ChoosenPathIndex].Points[0].position;
        newEnemy.SetTargetPos(_enemyPaths[newEnemy.ChoosenPathIndex].Points[1].position);
        newEnemy.SetCurrentPointInPath(1);
        newEnemy.gameObject.SetActive(true);
    }

    public Bullet GetBulletFromPool(Bullet prefab)
    {
        GameObject newBulletObj = _spawnedBullets.Find(
            b => !b.gameObject.activeSelf && b.name.Contains(prefab.name)
        )?.gameObject;

        if (newBulletObj == null)
        {
            newBulletObj = Instantiate(prefab.gameObject);
        }

        Bullet newBullet = newBulletObj.GetComponent<Bullet>();

        if (!_spawnedBullets.Contains(newBullet))
        {
            _spawnedBullets.Add(newBullet);
        }

        return newBullet;
    }

    public void ExplodeAt(Vector2 point, float radius, int damage)
    {
        foreach (Enemy enemy in _spawnedEnemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                if (Vector2.Distance(enemy.transform.position, point) <= radius)
                {
                    enemy.ReduceEnemyHealth(damage);
                }
            }
        }
    }

    public void ReduceLives(int value)
    {
        SetCurrentLives(_currentLives - value);

        if (_currentLives <= 0)
        {
            SetGameOver(false);
        }
    }

    public void SetCurrentLives(int currentLives)
    {
        // Mathf.Max fungsi nya adalah mengambil angka terbesar
        // sehingga _currentLives di sini tidak akan lebih kecil dari 0
        _currentLives = Mathf.Max(currentLives, 0);
        _livesInfo.text = $"Lives: {_currentLives}";
    }

    public void SetTotalEnemy(int totalEnemy)
    {
        _enemyCounter = totalEnemy;
        _totalEnemyInfo.text = $"Total Enemy: {Mathf.Max(_enemyCounter, 0)}";
    }

    public void SetTotalCoin()
    {
        _coinInfo.text = $"Coin: {GameResources.Coin}";
    }


    public void SetGameOver(bool isWin)
    {
        IsOver = true;

        _panel.gameObject.SetActive(true);

        if (!isWin)
        {
            _Next.gameObject.SetActive(false);
            _youWin.gameObject.SetActive(false);
            _youLose.gameObject.SetActive(true);
        }
        else
        {
            _youWin.gameObject.SetActive(true);
            _youLose.gameObject.SetActive(false);
            UnlockLevel();

            if (SceneManager.GetActiveScene().name == "Level 3")
            {
                _Next.gameObject.SetActive(false);
            }
        }
    }

    private void UnlockLevel()
    {
        int[] arrLevel = GameResources.Level;
        int index = arrLevel.Length;
        int heighestLevel = arrLevel[index - 1];

        string name = SceneManager.GetActiveScene().name;
        int currLevel = int.Parse(name.Substring(6, 1));

        if (heighestLevel < currLevel + 1)
        {
            int[] newArrLevel = Extension.Append(arrLevel, currLevel + 1);

            GameResources.Level = newArrLevel;
            //Debug.Log("Changed");
        }
    }
}
