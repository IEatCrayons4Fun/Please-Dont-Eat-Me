using UnityEngine;

public class TrackingEnemy : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float damping = 5f;

    // How often (seconds) to search for the nearest enemy. Reduces cost vs searching every frame.
    [SerializeField] private float searchInterval = 0.25f;
    private float searchTimer = 0f;

    private void FixedUpdate()
    {
        // Rotation logic: only runs when there is a target
        if (target != null)
        {
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0f; // keep turret level on Y
            if (lookPos.sqrMagnitude > 0.0001f)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            }
        }
    }

    private void Update()
    {
        // Update search timer on Update (not FixedUpdate) so it is in real time
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            searchTimer = searchInterval;
            FindClosestEnemy();
        }
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0)
        {
            target = null;
            return;
        }

        GameObject closest = null;
        float closestDistSq = Mathf.Infinity;
        Vector3 myPos = transform.position;

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 diff = enemies[i].transform.position - myPos;
            float distSq = diff.sqrMagnitude;
            if (distSq < closestDistSq)
            {
                closest = enemies[i];
                closestDistSq = distSq;
            }
        }

        target = (closest != null) ? closest.transform : null;
    }
}