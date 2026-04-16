using UnityEngine;
using UnityEngine.InputSystem;

// Requires Unity's Input System (default in VR Template)
public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;
    public LayerMask placeableLayer; // Assign "Floor" layer in inspector

    [Header("Input Setup")]
    public InputActionProperty triggerAction; // Assign the trigger pull action in Inspector

    void Update()
    {
        // 1. Check if user pulled the trigger this frame
        if (triggerAction.action != null && triggerAction.action.WasPressedThisFrame())
        {
            if (GameManager.Instance != null && GameManager.Instance.coreManager.currentHealth <= 0) return;

            // 2. Shoot a standard physics ray forward from the controller
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f, placeableLayer))
            {
                PlaceTower(hit.point);
            }
        }

        // Fallback for testing in editor with a mouse click
        if (Input.GetMouseButtonDown(0) && Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, placeableLayer))
            {
                PlaceTower(hit.point);
            }
        }
    }

    private void PlaceTower(Vector3 position)
    {
        // Add an offset so the cylinder doesn't sink into the floor (assuming standard Unity cylinder height of 2)
        Vector3 spawnPosition = position + (Vector3.up * 1f);
        Instantiate(towerPrefab, spawnPosition, Quaternion.identity);
    }
}
