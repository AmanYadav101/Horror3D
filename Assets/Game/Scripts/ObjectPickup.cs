using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class ObjectPickup : MonoBehaviour
{
    public Transform leftHandSocket; 
    public Transform rightHandSocket;

    public float pickupRange = 2f;
    public LayerMask pickupLayer;
    public Camera cam;
    
    public TwoBoneIKConstraint leftHandRigIK;
    public TwoBoneIKConstraint rightHandRigIK;
    public MultiAimConstraint chestRig;
    
    private GameObject _heldLeftObject;
    private Rigidbody _heldLeftRb;
    
    private GameObject _heldRightObject;
    private Rigidbody _heldRightRb;
    
    private List<GameObject> _heldObjects=new List<GameObject>();
    private Dictionary<GameObject, (Rigidbody, TwoBoneIKConstraint, Transform)> objectData=new Dictionary<GameObject, (Rigidbody, TwoBoneIKConstraint, Transform)>();

    
    void Update()
    {
        DrawDebugRay();
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            TryPickup();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropObject();
        }
    }

 
    void TryPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickupRange, pickupLayer))
        {
            if (hit.collider.CompareTag("Pickup_Lamp"))
            {
                if (_heldLeftObject == null)
                {
                    PickupObject(hit.collider.gameObject,  leftHandSocket, leftHandRigIK);
                }
                
            }
            else if (hit.collider.CompareTag("Pickup_Pistol"))
            {
                if (_heldRightObject == null)
                {
                   
                    PickupObject(hit.collider.gameObject, rightHandSocket, rightHandRigIK);
                }
                
            }
        }
    }

   
    
    void PickupObject(GameObject obj, Transform handSocket, TwoBoneIKConstraint handIK)
    {
        if(_heldObjects.Contains(obj))return;
        Rigidbody heldRb = obj.GetComponent<Rigidbody>();
        obj.GetComponent<BoxCollider>().isTrigger = true;

        heldRb.isKinematic = true;
        heldRb.useGravity = false;

        Debug.Log(handSocket.name);
        obj.transform.SetParent(handSocket);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        handIK.weight = 1f;
        chestRig.weight = 1f;
        
        _heldObjects.Add(obj);
        objectData[obj] = (heldRb, handIK, handSocket);
        Debug.Log($"Picked up {obj.name}");
    }

    void DropObject()
    {
        
        if (_heldObjects.Count == 0) return;
        GameObject objectToDrop = _heldObjects[_heldObjects.Count - 1];
        _heldObjects.RemoveAt(_heldObjects.Count - 1);
        
        
        if (objectData.TryGetValue(objectToDrop, out var data))
        {
            Rigidbody objRb = data.Item1;
            TwoBoneIKConstraint handIK = data.Item2;

            objRb.isKinematic = false;
            objRb.useGravity = true;
            objectToDrop.GetComponent<BoxCollider>().isTrigger = false;

            objectToDrop.transform.SetParent(null);

            handIK.weight = 0f;

            if (_heldObjects.Count == 0)
            {
                chestRig.weight = 0f;
            }

            objectData.Remove(objectToDrop);
            Debug.Log("Dropped " + objectToDrop.name);
        }
        
    }

    void DrawDebugRay()
    {
        Vector3 rayOrigin = cam.transform.position;
        Vector3 rayDirection = cam.transform.forward * pickupRange;
        Debug.DrawRay(rayOrigin, rayDirection, Color.red);
    }
}