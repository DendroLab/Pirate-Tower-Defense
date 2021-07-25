using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{

    private Tower _placedTower;
    private Entity entity;

    // Fungsi yang terpanggil sekali ketika ada object Rigidbody yang menyentuh area collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_placedTower != null)
        {
            return;
        }

        if (collision.tag == "Tower")
        {
            Tower tower = collision.GetComponent<Tower>();

            int turretPrice = tower.getTurretPrice();
            bool isBought = CoinDecrease(turretPrice);
            if (isBought)
            {
                if (tower != null)
                {
                    tower.SetPlacePosition(transform.position);
                    _placedTower = tower;
                }
            }
            else
            {
                return;
            }
        }
    }

    // Kebalikan dari OnTriggerEnter2D, fungsi ini terpanggil sekali ketika object tersebut meninggalkan area collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_placedTower == null)
        {
            return;
        }

        _placedTower.SetPlacePosition(null);
        _placedTower = null;
    }

    public bool CoinDecrease(int value)
    {
        int _currCoin = entity.Coin;
        if (_currCoin < value) return false;
        else
        {
            int coin = _currCoin - value;
            entity.Coin = coin;
            return true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        entity = new Entity();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
