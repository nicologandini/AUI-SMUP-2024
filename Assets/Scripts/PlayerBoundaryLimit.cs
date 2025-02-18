using System;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.DebugTree;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBoundaryLimit : MonoBehaviour
{
    [Header ("Player script")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Transform visorTransform;
    [SerializeField] private XROrigin XR_Origin;



    [Header ("Limits")]
    [SerializeField] private GameObject xLimitPosObj;
    [SerializeField] private GameObject xLimitNegObj;
    [SerializeField] private GameObject zLimitPosObj;
    [SerializeField] private GameObject zLimitNegObj;
    [SerializeField] private float repositionOffset = 0.3f;

    private bool _isLimitValid = false;
    private Vector3 _prevCCposition;
    private float _deltaPosThreshold = 0.01f;

    //Debugging 
    private Dictionary<Transform, Vector3> prevPositions;

    void Start()
    {
        if(xLimitNegObj == null || xLimitPosObj == null || zLimitNegObj == null || zLimitPosObj == null || playerController == null) {
            _isLimitValid = false;
        } else { 
            _isLimitValid = true;
        }

        if (visorTransform == null && playerController != null) {
            visorTransform = playerController.transform;
        }

        if (XR_Origin == null) {
            XR_Origin = FindFirstObjectByType<XROrigin>(); // Trova XR Rig automaticamente
        }

        prevPositions = new Dictionary<Transform, Vector3>();
        _prevCCposition = playerController.transform.position;
    }

    private void Update() {
        if(!_isLimitValid) {return;}

        RepositionPlayer();
        _prevCCposition = playerController.transform.position;
    }


    private void RepositionPlayer() {
        if (playerController == null) { return; }

        if (visorTransform.transform.position.x < xLimitNegObj.transform.position.x)
        {
            SetCompletePosition(xLimitNegObj.transform.position.x + repositionOffset, playerController.transform.position.z);
        } else if (visorTransform.transform.position.x > xLimitPosObj.transform.position.x) {
            SetCompletePosition(xLimitPosObj.transform.position.x - repositionOffset, playerController.transform.position.z);
        }

        if (visorTransform.transform.position.z < zLimitNegObj.transform.position.z) {
            SetCompletePosition(playerController.transform.position.x, zLimitNegObj.transform.position.z + repositionOffset);
        } else if (visorTransform.transform.position.z > zLimitPosObj.transform.position.z) {
            SetCompletePosition(playerController.transform.position.x, zLimitPosObj.transform.position.z - repositionOffset);
        }
    }

    private void SetCompletePosition(float xPos, float zPos)
    {
        // Reposition only after the movemnt with controller
        float deltaPos = Vector3.Distance(_prevCCposition,  playerController.transform.position);
        if(deltaPos < _deltaPosThreshold) {return;}

        //Debug
        // DebugDialogue.Instance.AppendInfoText($"Repositioning del player da posizione: {visorTransform.position}");
        // DebugDialogue.Instance.AppendInfoText("");
        // CheckDeltaPositions(visorTransform);
        //END Debug

        playerController.enabled = false;
        playerController.transform.position = new Vector3(xPos, playerController.transform.position.y, zPos);
        // visorTransform.position = new Vector3(xPos, visorTransform.position.y, zPos);
        // XR_Origin.transform.position = new Vector3(xPos, XR_Origin.transform.position.y, zPos);

        Vector3 cameraOffset = visorTransform.position - XR_Origin.transform.position;
        Vector3 newXRPosition = new Vector3(xPos, XR_Origin.transform.position.y, zPos) - new Vector3(cameraOffset.x, 0, cameraOffset.z);
        XR_Origin.transform.position = newXRPosition;

        //print("XR_Camera:" + XR_Origin.Camera); //Center eye offset;
        playerController.enabled = true;

        //SaveChildrenPosition(visorTransform);
    }

    //Debug
    private void SaveChildrenPosition(Transform parent) {
        if (prevPositions == null) {return;}


        foreach (Transform child in parent) {
            if(prevPositions.ContainsKey(child)) {
                float deltaPos = Vector3.Distance(prevPositions[child],  child.localPosition);
                if(deltaPos > float.Epsilon) {
                    prevPositions[child] = child.localPosition;
                }
            } else {
                prevPositions.Add(child, child.localPosition);
            }

            SaveChildrenPosition(child);
        }
    }

    //Debug
    private void CheckDeltaPositions(Transform parent) {
        if (prevPositions == null || prevPositions.Count == 0) {return;}
        
        foreach (Transform child in parent) {
            if(prevPositions.ContainsKey(child)) {
                float deltaPos = Vector3.Distance(prevPositions[child],  child.localPosition);
                if(deltaPos > _deltaPosThreshold) {
                    DebugDialogue.Instance.AppendInLine($"{child.name} moved of {deltaPos}");
                }
            } else {
                continue;
            }

            CheckDeltaPositions(child);
        }
    }
}