using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Aegis.GrenadeSystem.HiEx
{
    public class Flashbang : MonoBehaviour
    {
        [Header("Flashbang Effects")]
        [SerializeField] private GameObject explosionEffectPrefab;
        [SerializeField] private Vector3 explosionParticleOffset = new Vector3(0, 1, 0);

        [Header("Flashbang Settings")]
        [SerializeField] private float explosionDelay = 2.5f;
        [SerializeField] private float flashRadius = 6f;
        [SerializeField] private float flashDuration = 2f;
        [SerializeField] private float explosionForce = 250f;
        [SerializeField] private float explosionForceRadius = 4f;

        [Header("Audio Effects")]
        [SerializeField] private GameObject audioSourcePrefab;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip impact;
        [SerializeField] private AudioClip[] explosionSounds;

        private float countdown;
        private bool hasExploded = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            countdown = explosionDelay;
        }

        private void Update()
        {
            if (!hasExploded)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0f)
                {
                    Explode();
                    hasExploded = true;
                }
            }
        }

        private void Explode()
        {
            if (explosionEffectPrefab != null)
            {
                GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position + explosionParticleOffset, Quaternion.identity);
                Destroy(explosionEffect, 1.9f);
            }

            PlaySoundAtPosition();
            ApplyFlashEffect();
            DisableAfterExplosion();
        }

        private void ApplyFlashEffect()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, flashRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    PlayerMovement playerMovement = hitCollider.GetComponent<PlayerMovement>();
                    if (playerMovement != null)
                        StartCoroutine(FlashPlayer(playerMovement));
                }
                else if (hitCollider.CompareTag("Enemy"))
                {
                    if (hitCollider.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
                        StartCoroutine(FreezeNavMeshAgent(agent, flashDuration));
                }
            }
        }


        private void PlaySoundAtPosition()
        {
            if (audioSourcePrefab == null || explosionSounds == null || explosionSounds.Length == 0)
                return;

            GameObject audioSourceObject = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
            AudioSource instantiatedAudioSource = audioSourceObject.GetComponent<AudioSource>();
            if (instantiatedAudioSource == null)
            {
                Destroy(audioSourceObject);
                return;
            }

            int rand = Random.Range(0, explosionSounds.Length);
            instantiatedAudioSource.spatialBlend = 1;
            instantiatedAudioSource.clip = explosionSounds[rand];
            instantiatedAudioSource.Play();
            Destroy(audioSourceObject, instantiatedAudioSource.clip.length);
        }

        private IEnumerator FlashPlayer(PlayerMovement playerMovement)
        {
            if (playerMovement == null)
                yield break;

            Camera cam = Camera.main;
            float originalFov = cam != null ? cam.fieldOfView : 60f;
            float targetFov = Mathf.Min(originalFov * 1.8f, 120f);

            playerMovement.enabled = false;
            if (cam != null)
                yield return StartCoroutine(LerpCameraFov(cam, originalFov, targetFov, 0.12f));

            yield return new WaitForSeconds(flashDuration);

            if (cam != null)
                yield return StartCoroutine(LerpCameraFov(cam, targetFov, originalFov, 0.2f));

            playerMovement.enabled = true;
        }

        private IEnumerator LerpCameraFov(Camera cam, float startFov, float endFov, float duration)
        {
            if (cam == null || duration <= 0f)
                yield break;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                cam.fieldOfView = Mathf.Lerp(startFov, endFov, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            cam.fieldOfView = endFov;
        }

        private IEnumerator FreezeNavMeshAgent(NavMeshAgent agent, float duration)
        {
            if (agent == null)
                yield break;

            bool originalStopped = agent.isStopped;
            agent.isStopped = true;
            yield return new WaitForSeconds(duration);
            agent.isStopped = originalStopped;
        }

        private void DisableAfterExplosion()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
                renderer.enabled = false;

            Destroy(gameObject, flashDuration + 0.2f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                return;

            if (audioSource != null && impact != null)
            {
                audioSource.clip = impact;
                audioSource.spatialBlend = 1;
                audioSource.Play();
            }
        }
    }
}
