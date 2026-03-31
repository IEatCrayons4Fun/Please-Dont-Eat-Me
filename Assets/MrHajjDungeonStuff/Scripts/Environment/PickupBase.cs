using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBase : MonoBehaviour
{
    [Header("Pickup Base")]
    public float floatAmount = 0.5f;
    public float floatSpeed = 1.0f;
    public float rotationSpeed = 50.0f;

    private Vector3 startPos;

    [HideInInspector] public GameObject player;

    void Start()
    {
        startPos = this.gameObject.transform.position;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        this.gameObject.transform.position = new Vector3(startPos.x, startPos.y + newY, startPos.z);

        this.gameObject.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
    public void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            player = other.gameObject;
            PickupEffect();
        }
    }
    public virtual void PickupEffect(){
        this.gameObject.SetActive(false);
    }
}