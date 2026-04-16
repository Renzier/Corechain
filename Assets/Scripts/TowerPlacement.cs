using UnityEngine;

// Uses vanilla Unity inputs to avoid any package dependency errors
public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;
    public LayerMask placeableLayer; // Assign "Floor" layer in inspector

    // Cooldown to prevent spamming towers instantly
    private float placementCooldown = 0.5f;
    private float lastPlaceTime = 0f;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.coreManager.currentHealth <= 0) return;

        // Check if enough time has passed since last placement
        if (Time.time - lastPlaceTime < placementCooldown) return;

        // 1. Check for trigger pull (Fire1 is the default Unity mapping for the primary trigger/click)
        // We also check for mouse click for easy testing in the editor
        if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))
        {
            // Try to cast a ray from the Main Camera first (if clicking with mouse)
            if (Input.GetMouseButtonDown(0) && Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, placeableLayer))
                {
                    PlaceTower(hit.point);
                    return;
                }
            }

            // Otherwise, shoot a standard physics ray forward from the VR Controller
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit vrHit, 100f, placeableLayer))
            {
                PlaceTower(vrHit.point);
            }
        }
    }

    private void PlaceTower(Vector3 position)
    {
        lastPlaceTime = Time.time;
        // Add an offset so the cylinder doesn't sink into the floor (assuming standard Unity cylinder height of 2)
        Vector3 spawnPosition = position + (Vector3.up * 1f);
        Instantiate(towerPrefab, spawnPosition, Quaternion.identity);
    }
}
