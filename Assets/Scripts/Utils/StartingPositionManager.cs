using Unity.XR.CoreUtils;
using UnityEngine;

public class StartingPositionManager : MonoBehaviour
{
    [SerializeField] private Transform desiredPose;
    [SerializeField] private bool repositionOnStart = false; 

    [Header("Target")]
    [SerializeField] private CharacterController charController;
    [SerializeField] private Transform visorTransform;
    [SerializeField] private XROrigin XR_Origin;


    void Start()
    {
        if(repositionOnStart)
            SetCompletePose(desiredPose);
    }


    public void SetCompletePose(Transform targetTransform)
    {
        float xPos = targetTransform.position.x;
        float zPos = targetTransform.position.z;

        if(charController != null) {
            charController.enabled = false;
            charController.transform.position = new Vector3(xPos, charController.transform.position.y, zPos);
        }

        Vector3 cameraOffset = visorTransform.position - XR_Origin.transform.position;
        Vector3 newXRPosition = new Vector3(xPos, XR_Origin.transform.position.y, zPos) - new Vector3(cameraOffset.x, 0, cameraOffset.z);
        XR_Origin.transform.position = newXRPosition;
        visorTransform.rotation = Quaternion.Euler(targetTransform.rotation.eulerAngles);
        //print("XR_Camera:" + XR_Origin.Camera); //Center eye offset;
        if(charController != null) {
            charController.enabled = true;
        }

        //SaveChildrenPosition(visorTransform);
    }
}