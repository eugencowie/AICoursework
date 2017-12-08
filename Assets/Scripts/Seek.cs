using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Seek : MonoBehaviour
{
    public Transform target;
    public Action onArrival;
    
    public float maxSpeed = 4;
    public float arrivalRadius = 2;

    private new Collider collider;
    private new Rigidbody rigidbody;
    
    private void Start()
    {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Get the position and velocity of the agent
        Vector3 agentPosition = transform.position;
        Vector3 agentVelocity = rigidbody.velocity;

        // Calculate the target position on the ground
        Vector3 targetPosition = target.position + new Vector3(0, collider.bounds.extents.y, 0);

        // Calculate the offset from the agent to the target
        Vector3 offset = targetPosition - agentPosition;

        // Initially set the desired speed to max speed
        float desiredSpeed = maxSpeed;

        // If within the arrival distance, reduce the desired speed
        if (offset.magnitude < arrivalRadius)
        {
            desiredSpeed = maxSpeed * (offset.magnitude / arrivalRadius);
        }

        // Set the desired velocity by taking the direction from agent
        // to target multiplied by the desired speed
        Vector3 desiredVelocity = offset.normalized * desiredSpeed;

        // Calculate steering velocity
        Vector3 steeringVelocity = desiredVelocity - agentVelocity;

        // Apply steering velocity
        agentVelocity += steeringVelocity;

        // Disable the target when we get close
        if (offset.magnitude <= 1.01f)
        {
            onArrival?.Invoke();
        }

        // Apply velocity change
        rigidbody.velocity = agentVelocity;
    }
}
