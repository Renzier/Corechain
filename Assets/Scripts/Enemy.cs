using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 20f;
    public float speed = 2f;
    public Transform target;
    public float damageToCore = 10f;

    void Update()
    {
        if (target == null) return;

        // Move towards core
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotate for bug effect
        transform.Rotate(Vector3.up * 100f * Time.deltaTime);

        // Check distance to core
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            CoreManager core = target.GetComponent<CoreManager>();
            if (core != null)
            {
                core.TakeDamage(damageToCore);
            }
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
