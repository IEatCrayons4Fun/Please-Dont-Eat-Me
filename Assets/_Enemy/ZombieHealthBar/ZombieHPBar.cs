using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    private ZombieHealth zombieHealth;
    private Camera cam;

    void Start()
    {
        zombieHealth = GetComponentInParent<ZombieHealth>();
        cam = Camera.main;
    }

    void Update()
    {
        // Always face the camera
        transform.LookAt(transform.position + cam.transform.forward);

        // Update fill amount
        fillBar.fillAmount = zombieHealth.currentHealth / zombieHealth.maxHealth;
    }
}