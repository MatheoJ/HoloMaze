using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DisableTeleportOnClipping : MonoBehaviour
{
    [SerializeField]
    private XRRayInteractor rayInteractor; // Ray Interactor for teleportation
    [SerializeField]
    private LayerMask clippingLayers; // Layers that should block teleportation

    private bool isClipping = false;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Clipping Layers: " + clippingLayers.value);
        Debug.Log("Object Layer: " + (1 << collision.gameObject.layer));
        Debug.Log("Clipping Layer Name: " + LayerMask.LayerToName(collision.gameObject.layer));
        Debug.Log("Result: " + (clippingLayers & (1 << collision.gameObject.layer)));

        if ((clippingLayers.value & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log("Clipping Layer Detected");
            isClipping = true;
            DisableTeleportation();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((clippingLayers.value & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log("Exiting Clipping Layer");
            isClipping = false;
            EnableTeleportation();
        }
    }

    private void DisableTeleportation()
    {
        if (rayInteractor != null)
        {
            Debug.Log("Disabling Teleportation");
            rayInteractor.enabled = false; // Disable the teleportation ray
        }
    }

    private void EnableTeleportation()
    {
        if (rayInteractor != null && !isClipping)
        {
            Debug.Log("Enabling Teleportation");
            rayInteractor.enabled = true; // Re-enable the teleportation ray
        }
    }
}
