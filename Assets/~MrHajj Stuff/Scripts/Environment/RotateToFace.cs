using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFace : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float damping;

    private void FixedUpdate(){
        if(target != null){
            var lookPos = target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping); 
        }
        else{
            target = PlayerSingleton.instance.gameObject.transform;
        }
    }
}