using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ChestRigController : MonoBehaviour
{
    public Transform chestTarget; 
    public Transform cameraTransform; 

    public float followSpeed = 5f; 

    void Update()
    {
        Vector3 targetDirection = cameraTransform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        chestTarget.rotation = Quaternion.Slerp(chestTarget.rotation, targetRotation, followSpeed * Time.deltaTime);
    }
}