using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class InteractionController : MonoBehaviour
{
    InputAction interact;
    [SerializeField] float interactCooldown;
    [SerializeField] float interactRange;
    private bool canInteract = true;
    [SerializeField] private LayerMask ignoreLayer;

    [Header("Pickup Prompt")]
    [SerializeField] private TextMeshProUGUI promptText;

    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckLook();

        if (interact.WasPressedThisFrame() && canInteract)
            Interact();
    }

    private void CheckLook()
    {
        if (Physics.Raycast(
            CameraSingleton.instance.transform.position,
            CameraSingleton.instance.transform.forward,
            out RaycastHit hit,
            interactRange,
            ~ignoreLayer))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                string label = hit.transform.gameObject.name;

                LootPickup loot = hit.transform.GetComponent<LootPickup>();
                if (loot != null)
                    label = loot.lootType.ToString();

                
                WeaponPickup weapon = hit.transform.GetComponent<WeaponPickup>();
                if (weapon != null)
                    label = weapon.GetComponent<Gun>().weaponName;

                
                WaterGunPickup waterGunPickup = hit.transform.GetComponentInParent<WaterGunPickup>();
                if (waterGunPickup != null)
                    label = waterGunPickup.GetComponent<WaterGun>().weaponName;

                ShowPrompt(label);
                return;
            }
        }

        HidePrompt();
    }

    private void Interact()
    {
        if (Physics.Raycast(
            CameraSingleton.instance.transform.position,
            CameraSingleton.instance.transform.forward,
            out RaycastHit hitData,
            interactRange,
            ~ignoreLayer))
        {
            IInteractable interactable = hitData.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interacted();
                StartCoroutine(InteractDelay());
            }
        }
    }

    private void ShowPrompt(string label)
    {
        if (promptText == null) return;
        promptText.text = $"{label}\n<size=14>[E] Pick Up</size>";
        promptText.gameObject.SetActive(true);
    }

    private void HidePrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(false);
    }

    private IEnumerator InteractDelay()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactCooldown);
        canInteract = true;
    }
}