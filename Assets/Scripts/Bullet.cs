using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private int _bulletPower;
    private float _bulletSpeed;
    private float _bulletSplashRadius;

    private Enemy _targetEnemy;
    private ParticleSystem particle;

    // FixedUpdate adalah update yang lebih konsisten jeda pemanggilannya
    // cocok digunakan jika karakter memiliki Physic (Rigidbody, dll)
    private void FixedUpdate() {
        if (LevelManager.Instance.IsOver) {
            return;
        }

        if (_targetEnemy != null) {
            if (!_targetEnemy.gameObject.activeSelf) {
                gameObject.SetActive(false);
                _targetEnemy = null;

                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _targetEnemy.transform.position, _bulletSpeed * Time.fixedDeltaTime);

            Vector3 direction = _targetEnemy.transform.position - transform.position;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (_targetEnemy == null) {
            return;
        }

        if (collision.gameObject.Equals(_targetEnemy.gameObject)) {

            if (_bulletSplashRadius > 0f) {
                // Bullet yang memiliki efek splash area
                LevelManager.Instance.ExplodeAt(transform.position, _bulletSplashRadius, _bulletPower);
            } else {
                // Bullet yang hanya single-target
                _targetEnemy.ReduceEnemyHealth(_bulletPower);
            }

            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

            particle.Play();

            StartCoroutine(ExampleCoroutine());
            
            _targetEnemy = null;
        }
    }

    public void SetProperties(int bulletPower, float bulletSpeed, float bulletSplashRadius) {
        _bulletPower = bulletPower;
        _bulletSpeed = bulletSpeed;
        _bulletSplashRadius = bulletSplashRadius;
    }

    public void SetTargetEnemy(Enemy enemy) {
        _targetEnemy = enemy;
    }

    IEnumerator ExampleCoroutine() {
        yield return new WaitForSeconds(1);
        particle.Stop();
        transform.GetChild(0).gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start(){
        particle = GetComponentInChildren<ParticleSystem>();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update(){
        
    }
}
