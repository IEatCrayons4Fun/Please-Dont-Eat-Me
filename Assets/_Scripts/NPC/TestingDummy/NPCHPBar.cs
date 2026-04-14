using UnityEngine;
using UnityEngine.UI;

public class npcHPBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    private NPCHealth npcHealth;
    private Camera cam;

    void Start()
    {
        npcHealth = GetComponentInParent<NPCHealth>();
        cam = Camera.main;
    }

    void Update()
    {
        // Always face the camera
        transform.LookAt(transform.position + cam.transform.forward);

        // Update fill amount
        fillBar.fillAmount = npcHealth.currentHealth / npcHealth.maxHealth;
        
    }
}