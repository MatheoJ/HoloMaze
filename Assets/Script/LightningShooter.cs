using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightningShooter : MonoBehaviour
{
    
    [SerializeField]
    InputActionReference shootLightning;
    
    [SerializeField]
    GameObject lightningPrefab;
    
    [SerializeField]
    Transform rightController;
    
    [SerializeField]
    ParticleSystem explosionEffect;
    
    [SerializeField]
    private float lightningDistance = 2.0f;
    
    [SerializeField]
    List<String> destroyableTags;

    private float lightningDuration = 0.8f;
    
    void Awake()
    {
        shootLightning.action.performed += ctx => ShootLightning();
    }
    
    void OnDestroy()
    {
        shootLightning.action.performed -= ctx => ShootLightning();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void ShootLightning()
    {
        Debug.Log("Shoot Lightning");
        StartCoroutine(CreateLightningEffectCoroutine());
        
        //Shoot a raycast from the right controller
        RaycastHit hit;
        //Filter out the Ignore RayCast layer
        int layerMask = ~(1 << 2);
        
        if (Physics.Raycast(rightController.position, rightController.forward, out hit,lightningDistance, layerMask))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            //Draw debug sphere at the hit point
            Debug.DrawRay(rightController.position, rightController.forward * hit.distance, Color.red, 10);
            CreateExplosionEffect(hit.point);
            
            Destroyable destroyable = hit.collider.gameObject.GetComponent<Destroyable>();
            if (destroyable != null)
            {
                destroyable.destroyObject();
            }
        }

    }
    
    IEnumerator CreateLightningEffectCoroutine()
    {
        // Create the lightning effects
        GameObject lightning = Instantiate(lightningPrefab, rightController.position, rightController.rotation);
        GameObject lightning2 = Instantiate(lightningPrefab, rightController.position, rightController.rotation);
        
        lightning.transform.Rotate(-90, 0, 0);
        lightning2.transform.Rotate(-90, 0, 0);
        lightning2.transform.Rotate(0, 90, 0);
        
        // Set the parent of the lightning to the right controller
        lightning.transform.parent = rightController;
        lightning2.transform.parent = rightController;

        // Translate the lighting by 0.5 in the z axis in the local space
        lightning.transform.localPosition = lightning.transform.localPosition + new Vector3(0, 0, 0.5f);
        lightning2.transform.localPosition = lightning2.transform.localPosition + new Vector3(0, 0, 0.5f);

        // Wait for the specified duration
        yield return new WaitForSeconds(lightningDuration);

        // Destroy the lightning effects
        if(lightning != null) Destroy(lightning);
        if(lightning2 != null) Destroy(lightning2);
    }
    
    void CreateExplosionEffect(Vector3 position)
    {
        ParticleSystem explosion = Instantiate(explosionEffect, position, Quaternion.identity);
        explosion.Play();
    }
    
    
}
