using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowTrap : TrapBase
{
    [Header("Arrow Trap")]
    [SerializeField] [Range(0.1f, 10f)] private float fireRate;
    [SerializeField] [Range(1f, 100f)] private float damage;
    [SerializeField] [Range(1f, 100f)] private int trapDuration;
    [SerializeField] GameObject[] firePoints;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] private int maxArrowCount;
    [SerializeField] [Range(1f, 100f)] private float arrowSpeed;
    [HideInInspector] public List<GameObject> arrows = new List<GameObject>();
    public bool canFire = true;

    private void Start()
    {
        StartCoroutine(SpawnArrows());
    }

    private IEnumerator SpawnArrows()
    {
        while(arrows.Count < maxArrowCount)
        {
            GameObject arrow = Instantiate(arrowPrefab, this.transform);
            arrows.Add(arrow);
            arrow.SetActive(false);
        }
        yield return null;
    }
    public override void TriggerTrap()
    {
        StartCoroutine(Firing());
    }

    private IEnumerator Firing()
    {
        float elapsedTime = 0f;
        while(elapsedTime < trapDuration)
        {
            if(canFire && arrows.Count > 0){
                StartCoroutine(FireDelay());
                GameObject randomFirePoint = firePoints[Random.Range(0, firePoints.Length)];
                GameObject firedArrow = arrows[Random.Range(0, arrows.Count - 1)];
                firedArrow.transform.position = randomFirePoint.transform.position;
                firedArrow.SetActive(true);
                Arrow arrowScript = firedArrow.GetComponent<Arrow>();
                arrowScript.parentTrap = this;
                arrowScript.damage = damage;
                arrowScript.rb.AddForce(randomFirePoint.transform.forward * arrowSpeed, ForceMode.Impulse);
                arrows.Remove(firedArrow);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator FireDelay()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}
