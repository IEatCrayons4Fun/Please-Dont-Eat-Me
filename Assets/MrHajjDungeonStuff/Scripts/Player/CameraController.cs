using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    InputAction look;
    [Range(0f, 90f)]public float lookXLimit = 45.0f;
    [Range(0.1f, 12f)]public float lookSens = 2.0f;
    private GameObject player;
    private float rotationX = 0;
    private float rotationY = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        look = InputSystem.actions.FindAction("Look");
        player = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        rotationX += (look.ReadValue<Vector2>().y * lookSens) * Time.deltaTime;
        rotationY +=  (look.ReadValue<Vector2>().x * lookSens) * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        transform.localRotation = Quaternion.Euler(-rotationX, this.transform.rotation.y, this.transform.rotation.z);
        player.transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }
}
