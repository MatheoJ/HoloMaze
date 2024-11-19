using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DisableTp : MonoBehaviour
{
    [SerializeField]
    private XRRayInteractor rayInteractor; // Ray Interactor for teleportation
    [SerializeField]
    private Transform headTransform; // Reference to the camera
    [SerializeField]
    private Transform controllerTransform; // Reference to the right controller
    [SerializeField]
    private LayerMask obstructionLayers; // Layers to check for obstructions

    private bool isObstructed = false;

    private void Update()
    {
        CheckForObstruction();
    }

    private void CheckForObstruction()
    {
        if (headTransform == null || controllerTransform == null || rayInteractor == null)
        {
            Debug.LogWarning("References are missing in DisableTeleportOnObstruction script.");
            return;
        }

        // Cast a ray from the head to the controller
        Vector3 direction = controllerTransform.position - headTransform.position;
        float distance = Vector3.Distance(headTransform.position, controllerTransform.position);

        Debug.DrawRay(headTransform.position, direction, Color.red); // Visualize the raycast

        // Perform the raycast
        if (Physics.Raycast(headTransform.position, direction.normalized, out RaycastHit hit, distance, obstructionLayers))
        {
            if (!isObstructed)
            {
                isObstructed = true;
                DisableTeleportation();
            }
        }
        else
        {
            if (isObstructed)
            {
                isObstructed = false;
                EnableTeleportation();
            }
        }
    }

    private void DisableTeleportation()
    {
        rayInteractor.enabled = false;
    }

    private void EnableTeleportation()
    {
        rayInteractor.enabled = true;
    }
}
