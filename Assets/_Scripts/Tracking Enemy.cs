using UnityEngine;

public class TrackingEnemy : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float damping = 5f;
    [SerializeField] private float searchInterval = 0.25f;
    private float searchTimer = 0f;

    // Reference to the turret to access its in-range list
    private TriggerTurret turret;

    private void Start()
    {
        turret = GetComponentInParent<TriggerTurret>();
        // If TrackingEnemy is on a child object, use GetComponentInParent instead:
        // turret = GetComponentInParent<TriggerTurret>();
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0f;
            if (lookPos.sqrMagnitude > 0.0001f)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            }
        }
    }

    private void Update()
    {
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            searchTimer = searchInterval;
            FindClosestEnemy();
        }
    }

    private void FindClosestEnemy()
    {
        // If turret exists, only consider enemies inside its detection range
        if (turret != null && turret.enemiesInRange.Count > 0)
        {
            target = GetClosestFromList(turret.enemiesInRange);
            return;
        }

        // Fallback: no turret ref or no enemies in range, clear target
        target = null;
    }

    private Transform GetClosestFromList(System.Collections.Generic.List<GameObject> enemies)
    {
        GameObject closest = null;
        float closestDistSq = Mathf.Infinity;
        Vector3 myPos = transform.position;

        for (int i = 0; i < enemies.Count; i++)
        {
            // Skip destroyed enemies
            if (enemies[i] == null) continue;

            Vector3 diff = enemies[i].transform.position - myPos;
            float distSq = diff.sqrMagnitude;
            if (distSq < closestDistSq)
            {
                closest = enemies[i];
                closestDistSq = distSq;
            }
        }

        return closest != null ? closest.transform : null;
    }
}