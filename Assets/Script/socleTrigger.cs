using UnityEngine;

public class SocleTrigger : MonoBehaviour
{
    public GameObject crystalBall; // Référence vers la boule de cristal
    public Transform door; // Référence vers la porte
    public float openAngle = 90f; // Angle d'ouverture de la porte
    public float openingSpeed = 2f; // Vitesse d'ouverture de la porte

    private bool isCrystalPlaced = false;
    private bool isDoorOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Définit les rotations initiales et finales de la porte
        closedRotation = door.rotation;
        openRotation = Quaternion.Euler(door.eulerAngles.x, door.eulerAngles.y + openAngle, door.eulerAngles.z);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");
        Debug.Log(other.gameObject);
        // Vérifie si la boule de cristal est placée sur le socle
        if (other.gameObject == crystalBall)
        {
            isCrystalPlaced = true;
            // debug print
            Debug.Log("Boule de cristal placée sur le socle");
        }
    }

    void Update()
    {
        // Ouvre la porte si la boule de cristal est placée
        if (isCrystalPlaced && !isDoorOpen)
        {
            door.rotation = Quaternion.Slerp(door.rotation, openRotation, Time.deltaTime * openingSpeed);

            // Vérifie si la porte est complètement ouverte
            if (Quaternion.Angle(door.rotation, openRotation) < 0.1f)
            {
                isDoorOpen = true;
                door.rotation = openRotation;
            }
        }
    }
}
