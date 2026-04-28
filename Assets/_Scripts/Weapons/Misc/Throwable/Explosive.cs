using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.GrenadeSystem.HiEx
{
    public class HiExGrenade : MonoBehaviour
    {
        // Explosion effects
        [Header("Explosion Effects")]
        [SerializeField] GameObject explosionEffectPrefab;
        [SerializeField] Vector3 explosionParticleOffset = new Vector3(0, 1, 0);


        //explosion settings
        [Header("Explosion Settings")]
        [SerializeField] float explosionDelay = 3f;
        [SerializeField] float explosionForce = 1000f;
        [SerializeField] float explosionForceRadius = 5f;

        // Damage settings
        [Header("Damage Settings")]
        [SerializeField] float closeRadius = 1f;
        [SerializeField] float nearRadius = 5f;
        [SerializeField] float farRadius = 7f;

        [SerializeField] float closeDam = 10f;
        [SerializeField] float nearDam = 5f;
        [SerializeField] float farDam = 1f;


        // Audio effects
        [Header("Audio Effects")]
        [SerializeReference] GameObject audioSourcePrefab;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip impact;
        [SerializeField] AudioClip[] explosionSounds;

        // internal variables
        float countdown;
        bool hasexploded = false;

        private void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }


        private void Start()
        {
            // set the timer
            countdown = explosionDelay;
        }

        private void Update()
        {
            // if the grenade hasn't exploded, reduce the timer
            if (!hasexploded)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0)
                {
                    Explode();
                    hasexploded = true;
                }
            }


        }


        //explode function - what happens when the timer reaches 0
        void Explode()
        {

            // instantiate explosion effect at this game object
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position + explosionParticleOffset, Quaternion.identity);

            Destroy(explosionEffect, 1.9f);

            PlaySoundAtPosition();

            ApplyExplosiveForce();

            ApplyDamage();

            Destroy(gameObject);
        }


        //Function to apply damage to the player or to enemies
        void ApplyDamage()
        {
            Collider[] closecolliders = Physics.OverlapSphere(transform.position, closeRadius);
            Collider[] nearbycolliders = Physics.OverlapSphere(transform.position, nearRadius);
            Collider[] farcolliders = Physics.OverlapSphere(transform.position, farRadius);

            foreach (Collider farobject in farcolliders)
            {
                if (farobject.tag == "Player" || farobject.tag == "Enemy")
                {
                    HealthManager healthobject = farobject.GetComponent<HealthManager>();
                    if (healthobject)
                    {
                        // Check closest radius first, then near, then far
                        if (Physics.CheckSphere(farobject.transform.position, closeRadius))
                            healthobject.TakeDamage(closeDam);
                        else if (Physics.CheckSphere(farobject.transform.position, nearRadius))
                            healthobject.TakeDamage(nearDam);
                        else
                            healthobject.TakeDamage(farDam);
                    }
                }
            }
        }


        //Function to apply physics explosive force to objects near the explosion
        void ApplyExplosiveForce()
        {
            //Create a list of all colliders of objects within the radius of the explosion force
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionForceRadius);

            //for every collider collected, apply an explosive force originating from the position of the explosion
            foreach (Collider nearbyobject in colliders)
            {
                Rigidbody rb = nearbyobject.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionForceRadius);
                }
            }
        }

        //Function to play explosion sound effect by instantiating a new object to play that sound at the explosion
        void PlaySoundAtPosition()
        {
            GameObject audiosourceObject = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);

            int rand = Random.Range(0, explosionSounds.Length);

            AudioSource instantiatedAudioSource = audiosourceObject.GetComponent<AudioSource>();

            instantiatedAudioSource.spatialBlend = 1;
            instantiatedAudioSource.clip = explosionSounds[rand];
            instantiatedAudioSource.Play();

            Destroy(audiosourceObject, instantiatedAudioSource.clip.length);


        }

        //Function to play an impact sound effect if the thrown grenade hits something, but has not exploded yet
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag != "Player")
            {
                audioSource.clip = impact;

                audioSource.spatialBlend = 1;

                audioSource.Play();
            }

        }

    }

}
