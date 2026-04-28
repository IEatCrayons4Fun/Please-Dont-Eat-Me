using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Aegis.GrenadeSystem.HiEx
{
    public class GrenadeSystem : MonoBehaviour
    {
        [Header("Grenade Throwing System Settings")]
        [SerializeField] GameObject player;
        [SerializeField] Transform throwPoint;
        [SerializeField] Transform cam;
        [SerializeField] GameObject hiexgrenade;
        [SerializeField] GameObject flashbangPrefab;
        [SerializeField] GameObject grenadecount;
        [SerializeField] AudioClip throwAudio;

        private enum ThrowableType { HiExGrenade, Flashbang }
        [SerializeField] private ThrowableType currentThrowable = ThrowableType.HiExGrenade;

        [Header("Throwing Settings")]
        [SerializeField] int grenadeCount = 0;
        [SerializeField] float throwDelay = 0.3f;
        [SerializeField] float throwForce = 4f;

        [Header("Grenade Cooldown")]
        [SerializeField] float grenadeCooldown = 3f;
        private float cooldownTimer = 0f;
        private bool onCooldown = false;
        [SerializeField] Image cooldownImage;

        [Header("UI")]
        [SerializeField] GameObject grenadeCountUI;
        private bool hasPickedUpGrenade = false;

        Coroutine throwGrenade = null;

        private void Start()
        {
            if (grenadeCountUI != null)
                grenadeCountUI.SetActive(false);

            if (cooldownImage != null)
                cooldownImage.fillAmount = 0f;

            UpdateGrenadeCount();
        }

        private void Update()
        {
            if (onCooldown)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownImage != null)
                    cooldownImage.fillAmount = cooldownTimer / grenadeCooldown;

                if (cooldownTimer <= 0f)
                {
                    onCooldown = false;
                    cooldownTimer = 0f;
                    UpdateGrenadeCount();
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (grenadeCount > 0 && throwGrenade == null && !onCooldown)
                {
                    throwGrenade = StartCoroutine(ThrowGrenade());
                }
            }
        }

        IEnumerator ThrowGrenade()
        {
            grenadeCount -= 1;
            UpdateGrenadeCount();

            onCooldown = true;
            cooldownTimer = grenadeCooldown;
            if (cooldownImage != null)
                cooldownImage.fillAmount = 1f;

            player.GetComponent<AudioSource>().clip = throwAudio;
            player.GetComponent<AudioSource>().Play();

            yield return new WaitForSeconds(throwDelay);

            GameObject selectedGrenade = GetSelectedGrenadePrefab();
            if (selectedGrenade == null)
                selectedGrenade = hiexgrenade;

            GameObject grenadeInstance = Instantiate(selectedGrenade, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = grenadeInstance.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(cam.forward * throwForce, ForceMode.Impulse);

            throwGrenade = null;
        }

        public void PickupGrenade()
        {
            grenadeCount += 1;
            UpdateGrenadeCount();

            if (!hasPickedUpGrenade)
            {
                hasPickedUpGrenade = true;
                if (grenadeCountUI != null)
                    grenadeCountUI.SetActive(true);
            }
        }

        public void HideGrenadeUI()
        {
            hasPickedUpGrenade = false;
            grenadeCount = 0;
            if (grenadeCountUI != null)
                grenadeCountUI.SetActive(false);
            UpdateGrenadeCount();
        }

        void UpdateGrenadeCount()
        {
            if (grenadecount != null)
            {
                    grenadecount.GetComponent<TMPro.TextMeshProUGUI>().text = grenadeCount.ToString();

                if (cooldownImage != null)
                {
                    if (grenadeCount > 0 && !onCooldown)
                        cooldownImage.fillAmount = 1f;
                    else if (grenadeCount <= 0 && !onCooldown)
                        cooldownImage.fillAmount = 0f;
                }
            }
        }

        private GameObject GetSelectedGrenadePrefab()
        {
            return currentThrowable == ThrowableType.Flashbang ? flashbangPrefab : hiexgrenade;
        }

        public void SetThrowableToFlashbang()
        {
            currentThrowable = ThrowableType.Flashbang;
        }

        public void SetThrowableToHiExGrenade()
        {
            currentThrowable = ThrowableType.HiExGrenade;
        }
    }
}