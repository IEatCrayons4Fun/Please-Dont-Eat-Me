using UnityEngine;
using System.Collections.Generic;

public class Flashlight : MonoBehaviour, IInteractable
{

    private Light flashlightLight;





    private void Awake()
    {
        flashlightLight = GetComponent<Light>();
        if (flashlightLight == null){
            Debug.Log("Need a light on this Game Object");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && flashlightLight != null)
        {
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }

    public void Interacted()
    {
        //
    }

    private void Start()
    {
        //
    }









}