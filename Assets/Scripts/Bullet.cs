using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public TriggerTurret parentTrap;
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
        if(!other.gameObject.CompareTag("TurretTrap")){
            if(other.gameObject.CompareTag("Player")){
                other.GetComponent<HealthManager>().TakeDamage(damage);
            }
            this.gameObject.SetActive(false);
            parentTrap.bullets.Add(this.gameObject);
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
