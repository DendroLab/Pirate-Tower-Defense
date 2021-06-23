using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{

    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private SpriteRenderer _healthBar;
    [SerializeField] private SpriteRenderer _healthFill;

    [SerializeField] private int _EnemyCoin = 20;

    private int _currentHealth;

    public Vector3 TargetPos { get; private set; }
    public int CurrentPathIndex { get; private set; }

    private Entity entity;

    // Fungsi ini terpanggil sekali setiap kali menghidupkan game object yang memiliki script ini
    private void OnEnable() {
        _currentHealth = _maxHealth;
        _healthFill.size = _healthBar.size;
    }

    public void MoveToTarget() {
        transform.position = Vector3.MoveTowards(transform.position, TargetPos, _moveSpeed * Time.deltaTime);
    }

    public void SetTargetPos(Vector3 targetPos) {
        TargetPos = targetPos;
        _healthBar.transform.parent = null;

        // Mengubah rotasi dari enemy

        Vector3 distance = TargetPos - transform.position;

        if (Mathf.Abs(distance.y) > Mathf.Abs(distance.x)) {
            if (distance.y > 0) {
                // Menghadap atas
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
            }else {
                // Menghadap bawah
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
            }
        } else {
            if (distance.x > 0) {
                // Menghadap kanan (default)
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }else {
                // Menghadap kiri
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            }
        }

        _healthBar.transform.parent = transform;
    }

    public void ReduceEnemyHealth(int damage) {
        _currentHealth -= damage;

        AudioPlayer.Instance.PlaySFX("hit-enemy");

        if (_currentHealth <= 0) {
            _currentHealth = 0;
            gameObject.SetActive(false);

            AudioPlayer.Instance.PlaySFX("enemy-die");

            CoinIncrease(_EnemyCoin);
        }

        float healthPercentage = (float)_currentHealth / _maxHealth;

        _healthFill.size = new Vector2(healthPercentage * _healthBar.size.x, _healthBar.size.y);
    }

    // Menandai Indeks terakhir pada path
    public void SetCurrentPathIndex(int currIndex) {
        CurrentPathIndex = currIndex;
    }

    public void CoinIncrease(int value) {
        int _currCoin = entity.Coin;
        entity.Coin = (_currCoin + value);
    }

    // Start is called before the first frame update
    void Start(){
        entity = new Entity();
    }

    // Update is called once per frame
    void Update(){
        
    }
}
