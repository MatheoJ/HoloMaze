using UnityEngine;

public class ChestUnlock : MonoBehaviour
{
    [Header("Chest Components")]
    [SerializeField]
    private HingeJoint lidHingeJoint; // Hinge joint for the lid
    [SerializeField]
    private Collider chestBodyCollider; // Collider of the chest body
    [SerializeField]
    private Collider chestLidCollider;

    [Header("Key Settings")]
    [SerializeField]
    private GameObject correctKey; // Reference to the specific key object

    [Header("Hinge Joint Settings")]
    [SerializeField]
    private float unlockedMinAngle = 0f; // Minimum angle when unlocked
    [SerializeField]
    private float unlockedMaxAngle = 90f; // Maximum angle when unlocked

    private bool isUnlocked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isUnlocked) return;

        // Check if the correct key is inserted
        if (other.gameObject == correctKey)
        {
            UnlockChest();
        }
    }

    private void UnlockChest()
    {
        isUnlocked = true;
        Collider lockCollider = GetComponent<Collider>();

        // Allow the lid to open by updating the hinge joint limits
        if (lidHingeJoint != null)
        {
            JointLimits limits = lidHingeJoint.limits;
            limits.min = unlockedMinAngle;
            limits.max = unlockedMaxAngle;
            lidHingeJoint.limits = limits;
            lidHingeJoint.useLimits = true;
        }

        // Make the lock fall
        lockCollider.isTrigger = false;
        Rigidbody lockRigidbody = GetComponent<Rigidbody>();
        // give a slight push forward
        lockRigidbody.isKinematic = false;
        lockRigidbody.useGravity = true;
        lockRigidbody.AddForce(transform.forward * 2f, ForceMode.Impulse);
;
    }
}
