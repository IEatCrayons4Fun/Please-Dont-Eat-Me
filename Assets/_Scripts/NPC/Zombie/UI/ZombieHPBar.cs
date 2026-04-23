using UnityEngine;
using UnityEngine.UI;

public class ZombieHPBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    private ZombieHP zomHealth;

    void Start()
    {
        zomHealth = GetComponentInParent<ZombieHP>();
    }

    void Update()
    {
        fillBar.fillAmount = zomHealth.currentHealth / zomHealth.maxHealth;
        
    }
}