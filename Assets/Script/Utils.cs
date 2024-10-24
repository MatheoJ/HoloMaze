using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 scaleVector3(Vector3 v, float scale)
    {
        return new Vector3(v.x * scale, v.y * scale, v.z * scale);
    }
    
    public static Vector3 scaleInvVector3(Vector3 v, float scale)
    {
        return new Vector3(v.x / scale, v.y / scale, v.z / scale);
    }
    
    public static GameObject createDuplicate(GameObject orginalObject, GameObject parentOfDuplicate, float scale)
    {
        GameObject newChild = GameObject.Instantiate(orginalObject, Vector3.zero, Quaternion.identity);
        newChild.transform.parent = parentOfDuplicate.transform;
        Vector3 childPosition = orginalObject.transform.localPosition;
        Vector3 childScale = orginalObject.transform.localScale;
        newChild.transform.localScale = Utils.scaleVector3(childScale, scale);
        newChild.transform.localPosition = Utils.scaleVector3(childPosition, scale);
        newChild.transform.localRotation = orginalObject.transform.localRotation;
        return newChild;
    }
}
