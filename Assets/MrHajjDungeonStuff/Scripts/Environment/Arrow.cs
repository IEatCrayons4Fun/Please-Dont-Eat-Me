using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrow : MonoBehaviour
{
    public ArrowTrap parentTrap;
    public float damage;
    public Rigidbody rb;
    public float maxLifetime;
    private Coroutine lifetime;
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        if(lifetime == null){
            lifetime = StartCoroutine(MaxLifetime());
        }
        else
        {
            StopCoroutine(MaxLifetime());
            lifetime = StartCoroutine(MaxLifetime());
        }
    }
    private void OnTriggerEnter(Collider other){
        if(!other.gameObject.CompareTag("ArrowTrap")){
            if(other.gameObject.CompareTag("Player")){
                other.GetComponent<HealthManager>().TakeDamage(damage);
            }
            this.gameObject.SetActive(false);
            parentTrap.arrows.Add(this.gameObject);
            rb.linearVelocity = Vector3.zero;
        }
    }

    private IEnumerator MaxLifetime()
    {
        yield return new WaitForSeconds(maxLifetime);
        this.gameObject.SetActive(false);
        parentTrap.arrows.Add(this.gameObject);
        rb.linearVelocity = Vector3.zero;
    }
}
