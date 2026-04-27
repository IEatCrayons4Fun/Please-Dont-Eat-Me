using UnityEngine;
using UnityEngine.UI;


public class DetectionIndicator : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject indicatorRoot;

    private Zombie zombie;
   
    private void Start()
    {
        zombie = GetComponentInParent<Zombie>();
        indicatorRoot.SetActive(false);
        
        // Dim the background so fill stands out
        backgroundImage.color = new Color(1f, 0.3f, 0f, 0.3f);
    }

    private void Update()
    {
        float alertness = zombie.GetAlertness();

        if (alertness <= 0f)
        {
            indicatorRoot.SetActive(false);
            return;
        }

        indicatorRoot.SetActive(true);
        fillImage.fillAmount = alertness;

        // Pulse effect when nearly full
        if (alertness >= 0.8f)
        {
            float pulse = Mathf.PingPong(Time.time * 4f, 1f);
            fillImage.color = Color.Lerp(new Color(1f, 0.3f, 0f), Color.white, pulse * 0.3f);
        }
        else
        {
            fillImage.color = new Color(1f, 0.3f, 0f);
        }
    }
}