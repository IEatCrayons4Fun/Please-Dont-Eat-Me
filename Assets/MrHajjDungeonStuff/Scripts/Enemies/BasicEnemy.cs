using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicEnemy : BaseEnemy
{
    [SerializeField] float damage;
    [SerializeField] float attackRate;

    private bool attackCooldown;
    public void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Player") && !attackCooldown){
            StartCoroutine(AttackCooldown());
            other.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
        }
    }

    private IEnumerator AttackCooldown(){
        attackCooldown = true;
        Debug.Log("Attack Cooldown Started");
        yield return new WaitForSeconds(attackRate);
        attackCooldown = false;
        Debug.Log("Can attack again");
    }
}
