using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.GrenadeSystem.HiEx
{
    public class ThrowablePickup : MonoBehaviour, IInteractable
    {

        // this script handles picking up a grenade, and should be attatched to the grenade pickup
        // You can duplicate it for different types of grenade, to add them to an inventory system

        [SerializeField] AudioClip grenadePickupSound;

        private GameObject player;

        private void Start()
        {
            player = PlayerSingleton.instance.gameObject;
        }
        //this logic is what happens when a palyer picks up a grenade with this script attatched
        public void Interacted()
        {

                player.GetComponent<GrenadeSystem>().PickupGrenade();

                //play pickup sound
                AudioSource soundSource = player.GetComponent<AudioSource>();
                soundSource.clip = grenadePickupSound;
                soundSource.Play();

                //destory the pickup object
                Destroy(gameObject);

            
        }

    }
}