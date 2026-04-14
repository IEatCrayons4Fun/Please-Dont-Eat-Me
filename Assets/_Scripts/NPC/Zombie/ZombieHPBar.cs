using UnityEngine;
using UnityEngine.UI;

public class ZombieHPBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    private ZombieHP zomHealth;
    private Camera cam;

    void Start()
    {
        zomHealth = GetComponentInParent<ZombieHP>();
        cam = Camera.main;
    }

    void Update()
    {
        // Always face the camera
        transform.LookAt(transform.position + cam.transform.forward);

        // Update fill amount
        fillBar.fillAmount = zomHealth.currentHealth / zomHealth.maxHealth;
        
    }
}