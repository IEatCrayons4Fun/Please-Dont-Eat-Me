using UnityEngine;
using System.Collections.Generic;

public class Flashlight : MonoBehaviour, IInteractable
{

    private Light flashlightLight;





    private void Awake()
    {
        flashlight = GetComponent<Light>();
        if (flashlight == null){
            Debug.Log("Need a light on this Game Object")
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) && flashlight != null
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

    public void Interacted()
    {

    }

    private void Start()
    {

    }









}