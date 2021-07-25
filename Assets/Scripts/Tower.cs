using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    // Tower Component
    [SerializeField] private SpriteRenderer _towerBase;
    [SerializeField] private SpriteRenderer _towerHead;

    // Tower Properties
    [SerializeField] private int _shootPower = 1;
    [SerializeField] private float _shootDistance = 1f;
    [SerializeField] private float _shootDelay = 5f;
    [SerializeField] private float _bulletSpeed = 1f;
    [SerializeField] private float _bulletSplashRadius = 0f;
    [SerializeField] private int _turretPrice = 50;

    [SerializeField] private Bullet _bulletPrefab;

    private float _runningShootDelay;
    private Enemy _targetEnemy;
    private Quaternion _targetRotation;

    private ParticleSystem particle;

    private TowerPlacement _towerPlace;

    // Digunakan untuk menyimpan posisi yang akan ditempati selama di drag
    public Vector2? PlacePosition { get; private set; }
    internal bool isBought = false;

    public void SetTowerPlacement(TowerPlacement towerPlace)
    {
        _towerPlace = towerPlace;
        if (_towerPlace == null)
        {
            PlacePosition = null;
        }
        else
        {
            PlacePosition = _towerPlace.transform.position;
        }
    }

    public void LockPlacement()
    {
        transform.position = (Vector2)PlacePosition;
        _towerPlace.PlacedTower = this;
    }

    // Mengubah order in layer pada tower yang sedang di drag
    public void ToggleOrderInLayer(bool toFront)
    {
        int orderInLayer = toFront ? 99 : 1;
        _towerBase.sortingOrder = orderInLayer - 1;
        _towerHead.sortingOrder = orderInLayer;
    }


    // Fungsi yang digunakan untuk mengambil sprite pada Tower Head
    public Sprite GetTowerHeadIcon()
    {
        return _towerHead.sprite;
    }

    // Mengecek musuh terdekat
    public void CheckNearestEnemy(List<Enemy> enemies)
    {

        if (_targetEnemy != null)
        {
            if (!_targetEnemy.gameObject.activeSelf ||
                Vector3.Distance(transform.position, _targetEnemy.transform.position) > _shootDistance)
            {
                _targetEnemy = null;
            }
            else
            {
                return;
            }
        }

        float nearestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in enemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance > _shootDistance)
            {
                continue;
            }

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        _targetEnemy = nearestEnemy;
    }

    // Menembak musuh yang telah disimpan sebagai target
    public void ShootTarget()
    {
        if (_targetEnemy == null)
        {
            return;
        }

        _runningShootDelay -= Time.unscaledDeltaTime;

        if (_runningShootDelay <= 0f)
        {
            bool headHasAimed = Mathf.Abs(_towerHead.transform.rotation.eulerAngles.z - _targetRotation.eulerAngles.z) < 10f;

            if (!headHasAimed)
            {
                return;
            }

            transform.GetChild(1).gameObject.SetActive(true);
            particle.Play();

            StartCoroutine(WaitToShootCoroutine());

            Bullet bullet = LevelManager.Instance.GetBulletFromPool(_bulletPrefab);

            bullet.transform.position = transform.position;
            bullet.SetProperties(_shootPower, _bulletSpeed, _bulletSplashRadius);
            bullet.SetTargetEnemy(_targetEnemy);
            bullet.gameObject.SetActive(true);

            _runningShootDelay = _shootDelay;
        }
    }

    // Membuat tower selalu melihat ke arah musuh
    public void SeekTarget()
    {
        if (_targetEnemy == null)
        {
            return;
        }

        Vector3 direction = _targetEnemy.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _targetRotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));
        _towerHead.transform.rotation = Quaternion.RotateTowards(_towerHead.transform.rotation, _targetRotation, Time.deltaTime * 180f);
    }

    public int getTurretPrice()
    {
        return _turretPrice;
    }

    IEnumerator WaitToShootCoroutine()
    {
        yield return new WaitForSeconds(1);
        particle.Stop();
        transform.GetChild(1).gameObject.SetActive(false);
    }

    private TowerPlacement _lastTriggeredTowerPlace;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "TowerPlace")
        {
            _lastTriggeredTowerPlace = other.GetComponent<TowerPlacement>();
            if (_lastTriggeredTowerPlace.PlacedTower == null)
            {
                if (_turretPrice <= GameResources.Coin)
                {
                    SetTowerPlacement(_lastTriggeredTowerPlace);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (GameObject.ReferenceEquals(other.gameObject, _lastTriggeredTowerPlace.gameObject))
        {
            if (other.tag == "TowerPlace")
            {
                TowerPlacement towerPlacement = other.GetComponent<TowerPlacement>();
                if (towerPlacement.PlacedTower == null)
                {
                    SetTowerPlacement(null);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
