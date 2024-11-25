using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LightAtNight : MonoBehaviour
{
    // SCRIPT TO PUT ON THE LIGHT GAMEOBJECT
    [Header("Non compulsory")]
    [SerializeField] private GameObject vfxObject; // Non compulsory


    // Other variables
    private Light lightComponent;
    private Behaviour haloComponent;
    private GameManager gm;


    // Start is called before the first frame update
    void Start()
    {
        // Get the Game Manager
        gm = GameManager.Instance;
        if (gm == null) throw new Exception("game manager not found");

        // Make sure there is a light component and initialize it
        lightComponent = GetComponent<Light>();
        if (lightComponent == null) throw new Exception("No Light component found on the GameObject " + gameObject.name);

        // Initialize the halo, if there is one
        haloComponent = (Behaviour)GetComponent("Halo");
        if (haloComponent != null) Debug.Log("a Halo component has been found on the GameObject "+ gameObject.name);



        UpdateLightAndEffects();
    }







    // Update is called once per frame
    void Update()
    {
        UpdateLightAndEffects();
    }






    void UpdateLightAndEffects()
    {
        if (gm.GetItIsNight()) // Enable light and VFX
        {
            if (lightComponent != null) lightComponent.enabled = true;
            if (haloComponent != null) haloComponent.enabled = true;
            if (vfxObject != null) vfxObject.SetActive(true);
           
            //Debug.Log("ooooooooo light, halo and effect should be on");
        }
        else // Disable light and VFX
        {
            if (lightComponent != null) lightComponent.enabled = false;
            if (haloComponent != null) haloComponent.enabled = false;
            if (vfxObject != null) vfxObject.SetActive(false);
            //Debug.Log("oooooooo light, halo and effect should be off");
        }
    }
}
