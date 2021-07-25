using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField] private Image _towerIcon;
    [SerializeField] private Text _towerPriceText;

    private Tower _towerPrefab;
    private Tower _currentSpawnedTower;

    public void SetTowerPrefab(Tower tower)
    {
        _towerPrefab = tower;
        _towerIcon.sprite = tower.GetTowerHeadIcon();
        _towerPriceText.text = tower.getTurretPrice().ToString();
    }

    // Implementasi dari Interface IBeginDragHandler
    // Fungsi ini terpanggil sekali ketika pertama men-drag UI
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject newTowerObj = Instantiate(_towerPrefab.gameObject);
        _currentSpawnedTower = newTowerObj.GetComponent<Tower>();
        _currentSpawnedTower.ToggleOrderInLayer(true);
    }

    // Implementasi dari Interface IDragHandler
    // Fungsi ini terpanggil selama men-drag UI
    public void OnDrag(PointerEventData eventData)
    {
        Camera mainCamera = Camera.main;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);

        _currentSpawnedTower.transform.position = targetPos;
    }

    // Implementasi dari Interface IEndDragHandler
    // Fungsi ini terpanggil sekali ketika men-drop UI
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_currentSpawnedTower.PlacePosition == null)
        {
            Destroy(_currentSpawnedTower.gameObject);
            Debug.Log("abort");
        }
        else
        {
            Debug.Log("yes");
            _currentSpawnedTower.LockPlacement();
            _currentSpawnedTower.ToggleOrderInLayer(false);
            LevelManager.Instance.RegisterSpawnedTower(_currentSpawnedTower);
            GameResources.Coin -= _currentSpawnedTower.getTurretPrice();
            _currentSpawnedTower = null;
        }
    }
}
