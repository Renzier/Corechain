using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 5f;
    public float fireRate = 1f; // shots per second
    public float damage = 10f;

    private float fireTimer;
    public Transform ringToRotate; // Tron visual effect

    void Update()
    {
        if (ringToRotate != null)
        {
            ringToRotate.Rotate(Vector3.up * 120f * Time.deltaTime);
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate)
        {
            Enemy target = FindClosestEnemy();
            if (target != null)
            {
                Shoot(target);
                fireTimer = 0;
            }
        }
    }

    Enemy FindClosestEnemy()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        Enemy closest = null;
        float minDistance = range;

        foreach (Enemy e in allEnemies)
        {
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = e;
            }
        }
        return closest;
    }

    void Shoot(Enemy target)
    {
        // Apply damage
        target.TakeDamage(damage);

        // Visual laser effect using LineRenderer
        LineRenderer lr = gameObject.GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = gameObject.AddComponent<LineRenderer>();
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            lr.material.EnableKeyword("_EMISSION");
            lr.material.SetColor("_EmissionColor", Color.green * 2f);
        }

        lr.SetPosition(0, transform.position + Vector3.up * 1.5f); // Top of tower
        lr.SetPosition(1, target.transform.position);

        // Clear line after short delay
        Invoke(nameof(ClearLaser), 0.1f);
    }

    void ClearLaser()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, Vector3.zero);
        }
    }
}
