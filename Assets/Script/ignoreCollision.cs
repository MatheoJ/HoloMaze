using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField]
    private Collider chestBodyCollider; // Collider du corps du coffre
    [SerializeField]
    private Collider chestLidCollider;  // Collider du couvercle

    private void Start()
    {
        if (chestBodyCollider != null && chestLidCollider != null)
        {
            Physics.IgnoreCollision(chestBodyCollider, chestLidCollider);
        }
    }
}
