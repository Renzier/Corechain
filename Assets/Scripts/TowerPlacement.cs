using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Requires Unity's Input System and XR Interaction Toolkit (default in VR Template)
public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;
    public LayerMask placeableLayer; // Assign "Floor" layer in inspector

    [Header("Input Setup")]
    public InputActionProperty triggerAction; // Assign the trigger pull action in Inspector
    private XRRayInteractor rayInteractor;

    void Start()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    void Update()
    {
        // 1. Check if user pulled the trigger this frame
        if (triggerAction.action != null && triggerAction.action.WasPressedThisFrame())
        {
            if (GameManager.Instance.coreManager.currentHealth <= 0) return;

            // 2. Check if the XR Ray Interactor is pointing at the floor
            if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // Ensure we hit the designated floor layer
                if (((1 << hit.collider.gameObject.layer) & placeableLayer) != 0)
                {
                    PlaceTower(hit.point);
                }
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
