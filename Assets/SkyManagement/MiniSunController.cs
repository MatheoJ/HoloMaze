using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MiniSunController: MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform minimapCenter; // Is the center of the ellipse
    [SerializeField] private Material ellipseMaterial;

    // Children of MinimapCenter, used to calculate the miniSunAngle
    [SerializeField] private Transform XPointTransform;
    [SerializeField] private Transform ZPointTransform;


    [Header("Ellipse Settings")]
    [SerializeField] private int segmentsNumber = 100;
    [SerializeField] private float lineWidth = 0.02f;
    private bool ellipseVisible = true;


    [Header("Ellipse Clamping (0� at the top)")]
    [SerializeField] private float startAngle = -100f; // In Degrees
    [SerializeField] private float endAngle = 100f; // In Degrees


    [Header("Hinge Joint Clamping : A TESTER")]
    [SerializeField] private float minAngle = -100f;
    [SerializeField] private float maxAngle = 100f; 


    [Header("TESTS")]
    [SerializeField] private float testAngle = 0;



    // Other
    private Transform activeHand; // Hand with which the user is grabbing the sun on the minimap

    private bool isGrabbed = true; // M : should be false, true for tests


    // Sun Ellipse
    private float ellipseRadius;

    private LineRenderer lineRenderer;

    private float miniSunAngle;

    private GameManager gm;

    private HingeJoint hingeJoint;


    // Local coordinates
    private Vector3 XLocalVector;
    private Vector3 ZLocalVector;








    void Start()
    {

        // Make sure the references are given 
        if (ellipseMaterial == null || minimapCenter == null) throw new Exception("Reference(s) missing in the MiniSunController.cs script");


        // Get the Game Manager
        gm = GameManager.Instance;

        // Initialize HingeJoint
        hingeJoint = minimapCenter.GetComponent<HingeJoint>();
        if (hingeJoint == null) throw new Exception("HingeJoint not found on the GameObject.");
        hingeJoint.useLimits = true;
        JointLimits limits = hingeJoint.limits; // Retrieve current limits
        limits.min = minAngle;
        limits.max = maxAngle;
        hingeJoint.limits = limits; // Apply the new limits


        // Get or add LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Set up the LineRenderer properties
        lineRenderer.positionCount = segmentsNumber; // Number of segments in the ellipse
        lineRenderer.loop = false; // Not a closed shape
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth; 
        lineRenderer.material = ellipseMaterial;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = ellipseVisible;
        ellipseRadius = Vector3.Distance(this.transform.position, minimapCenter.position); // The ellipse radius adapts to the distance sun-ellipseCenter

        // Set the miniSun Position on the ellipse, according to the sun Start Angle defined in the GameManager
        float miniSunStartAngle = gm.GetSunAngle();
        transform.position = GetMinisunPositionFromAngle(miniSunStartAngle);
        Debug.Log(" ///////////////////////////////// miniSunStartAngle = " + miniSunStartAngle);
    }






    void Update()
    {
        if (lineRenderer.enabled) DrawEllipseTwo();
        //if (isgrabbed) SendNewSunAngleToGm(); // A METTRE QD TESTS AVEC CASQUE ==> call the function only when the minisun angle can change


        //TESTS
        transform.position = GetMinisunPositionFromAngle(testAngle);
        gm.ChangeSunAngle(testAngle);

        JointLimits limits = hingeJoint.limits; // Retrieve current limits
        limits.min = minAngle;
        limits.max = maxAngle;
        hingeJoint.limits = limits; // Apply the new limits
    }







    //private void DrawEllipse()
    //{
    //    lineRenderer.positionCount = segmentsNumber + 1; // +1 to close the loop

    //    Vector3[] ellipsePoints = new Vector3[segmentsNumber + 1]; // Array to hold the points

    //    // Calculate the local axes of the ellipse from the minimapCenter's transform
    //    Vector3 localY = minimapCenter.up;   // X-axis (right direction)
    //    Vector3 localX = minimapCenter.right;  // Z-axis (forward direction)

    //    // Generate points for the ellipse
    //    for (int i = 0; i <= segmentsNumber; i++) // Loop through each segment
    //    {
    //        float theta = 2.0f * Mathf.PI * i / segmentsNumber; // Angle for the current point

    //        // Calculate the position in local space
    //        Vector3 theLocalPosition = localY * (ellipseRadius * Mathf.Cos(theta)) + localX * (ellipseRadius * Mathf.Sin(theta));

    //        // Convert to world space and add to the center position
    //        ellipsePoints[i] = minimapCenter.position + theLocalPosition;
    //    }

    //    // Assign the points to the LineRenderer
    //    lineRenderer.SetPositions(ellipsePoints);
    //}



    private void DrawEllipseTwo()
    {
        // Ensure startAngle and endAngle are within a valid range
        float angleRange = Mathf.Clamp(endAngle - startAngle, 0, 360);

        // Calculate the number of points for the arc based on the angle range
        int pointCount = Mathf.CeilToInt(segmentsNumber * (angleRange / 360f)) + 1;
        lineRenderer.positionCount = pointCount;

        // Array to store the points of the partial ellipse
        Vector3[] ellipsePoints = new Vector3[pointCount];

        // Calculate the local axes of the ellipse based on the minimapCenter's transform
        Vector3 localY = minimapCenter.up;   // Y-axis (up direction)
        Vector3 localX = minimapCenter.right;  // X-axis (right direction)

        // Generate points for the partial ellipse
        for (int i = 0; i < pointCount; i++)
        {
            // Interpolate between the start and end angles
            float t = (float)i / (pointCount - 1);
            float theta = Mathf.Deg2Rad * Mathf.Lerp(startAngle, endAngle, t); // Convert the angle to radians

            // Calculate the position in local space
            Vector3 localPosition = localY * (ellipseRadius * Mathf.Cos(theta)) +
                                    localX * (ellipseRadius * Mathf.Sin(theta));

            // Convert the local position to world space
            ellipsePoints[i] = minimapCenter.position + localPosition;
        }

        // Assign the generated points to the LineRenderer
        lineRenderer.SetPositions(ellipsePoints);
    }





    public void OnGrab()
    {
        isGrabbed = true;
    }

    public void OnRelease()
    {
        isGrabbed = false;
    }



    public Vector3 GetMinisunPositionFromAngle(float angleInDegrees)
    {
        // Convert the angle to radians
        float angleInRadians = (angleInDegrees - 90) * Mathf.Deg2Rad; // -90 to match with the sky degrees range

        // Get the local axes of the ellipse from the minimapCenter
        Vector3 YDirection = minimapCenter.up;
        Vector3 XDirection = -minimapCenter.right;

        // Calculate the position in local space
        Vector3 localPosition = YDirection * (ellipseRadius * Mathf.Cos(angleInRadians)) +
                                XDirection * (ellipseRadius * Mathf.Sin(angleInRadians));

        // Convert the local position to world space
        Vector3 worldPosition = minimapCenter.position + localPosition;

        return worldPosition;
    }

    private float GetMinisunAngleFromPosition(Vector3 sunPosition)
    {
        // Calculate the miniSun angle (from -180� to 180�)
        Transform miniMap3D = this.transform.parent;
        Vector3 localMiniSunPosition = miniMap3D.InverseTransformPoint(transform.position);
        Vector3 localCenterPosition = miniMap3D.InverseTransformPoint(minimapCenter.position);

        Vector3 miniSunDirection = (localMiniSunPosition - localCenterPosition).normalized; // Direction of the miniSun towards the ellipse's center  (endpoint - startpoint)
        Vector3 ZWorldDirection = Vector3.forward; // We can keep the world vectors because the minimap always keeps the same orientation in the world
        Vector3 XWorldDirection = Vector3.right;

        Debug.Log("miniSunDirection: " + miniSunDirection);
        Debug.Log("ZWorldDirection: " + ZWorldDirection);
        Debug.Log("XWorldDirection: " + XWorldDirection);
        float calculatedAngle = Vector3.SignedAngle(XWorldDirection, miniSunDirection, ZWorldDirection);
        //float calculatedAngle = Vector3.SignedAngle(-XLocalDirection, miniSunDirection, ZLocalDirection);
        Debug.Log(" ++++++++++++++++  calculated angle  = " + calculatedAngle);

        return calculatedAngle;
    }


    public void SendNewSunAngleToGm()
    {
        float newMiniSunAngle = GetMinisunAngleFromPosition(transform.position);

        if (newMiniSunAngle != miniSunAngle) // If the angle has changed, notify the GameManager
        {
            Debug.Log(" ++++++++++++++++ miniSunAngle has changed ");
            Debug.Log(" ++++++++++++++++  newMiniSunAngle = " + newMiniSunAngle);
            Debug.Log(" ++++++++++++++++  miniSunAngle = " + miniSunAngle);
            miniSunAngle = newMiniSunAngle;
            gm.ChangeSunAngle(miniSunAngle); // MinisunAngle is always between -180 and 180�
        }
    }
}