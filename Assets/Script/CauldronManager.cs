using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronManager : MonoBehaviour
{
    [Header("Recipe Settings")]
    [SerializeField]
    private List<GameObject> requiredIngredients; // List of ingredients required for the recipe
    private List<GameObject> addedIngredients = new List<GameObject>(); // Track added ingredients

    [Header("Key Settings")]
    [SerializeField]
    private GameObject key; // Prefab of the key

    [SerializeField]
    private Transform keyFinalPosition; // Final position where the key should appear

    [Header("Feedback")]
    [SerializeField]
    ParticleSystem successEffect;
    [SerializeField]
    private AudioSource successSound;
    [SerializeField]
    private AudioSource dropSound;
    [SerializeField]
    private Transform EffectSpawnPoint;
    [SerializeField]
    ParticleSystem dropEffect;

    private bool isRecipeComplete = false;


    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is in the required ingredients list
        if (requiredIngredients.Contains(other.gameObject))
        {
            Debug.Log($"{other.gameObject.name} added to the cauldron!");

            // Add the ingredient to the added list and destroy the object
            addedIngredients.Add(other.gameObject);
            Destroy(other.gameObject);

            if (dropEffect != null) { 
                ParticleSystem dropEffectInstance = Instantiate(dropEffect, EffectSpawnPoint.position, Quaternion.Euler(-90, 0, 0));
            }
                
            if (dropSound != null)
            {
                dropSound.Play();
            }

            // Check if all ingredients are added
            if (!isRecipeComplete && IsRecipeComplete())
            {
                CompleteRecipe();
            }
        }
        else
        {
            Debug.Log($"{other.gameObject.name} is not part of the recipe.");
        }
    }

    private bool IsRecipeComplete()
    {
        // Check if all required ingredients have been added
        foreach (var ingredient in requiredIngredients)
        {
            if (!addedIngredients.Contains(ingredient))
            {
                return false;
            }
        }
        return true;
    }

    private void CompleteRecipe()
    {
        Debug.Log("Recipe complete!");
        isRecipeComplete = true;

        // Play success effect
        if (successEffect != null)
        {
            ParticleSystem successEffectInstance = Instantiate(successEffect, EffectSpawnPoint.position, Quaternion.identity);
        }

        // Play success sound
        if (successSound != null)
        {
            successSound.Play();
        }

        // Move the key to its final position
        if (key != null && keyFinalPosition != null)
        {
            Rigidbody keyRigidbody = key.GetComponent<Rigidbody>();
            if (keyRigidbody != null)
            {
                keyRigidbody.useGravity = true; // Enable gravity
                keyRigidbody.isKinematic = false; // Allow physics interactions
            }

            // Move the key to the final position
            key.transform.position = keyFinalPosition.position;
            key.transform.rotation = keyFinalPosition.rotation;

            Debug.Log("Key moved to final position!");
        }
    }
}
