using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour
{
    public Transform player; 
    public float rotationSpeed = 5f; 
    public float rotationRange = 10f; 

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= rotationRange)
        {
            RotateTowards();
        }
    }

    void RotateTowards()
    {
        Vector3 direction = (player.position - transform.position).normalized;
direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}