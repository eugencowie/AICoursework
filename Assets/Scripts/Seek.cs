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
    private Rigidbody agent;
    
    private void Start()
    {
        collider = GetComponent<Collider>();
        agent = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Set the target position above the actual target to take into account the agent's distance from the floor
        target.position += new Vector3(0, collider.bounds.extents.y, 0);

        // Calculate the distance from the agent to the target
        float distance = Vector3.Magnitude(target.position - agent.position);

        // Perform the steering behaviour
        UpdateSteering();

        // Undo the change to the target position
        target.position -= new Vector3(0, collider.bounds.extents.y, 0);

        // Disable the target when we get close
        if (distance <= 1.01f)
        {
            onArrival?.Invoke();
        }
    }

    private void UpdateSteering()
    {
        // Calculate the distance from the agent to the target
        float distance = Vector3.Magnitude(target.position - agent.position);

        // Set the desired speed to max speed initially
        float desiredSpeed = maxSpeed;

        // If within the arrival distance, reduce the desired speed
        if (distance < arrivalRadius)
        {
            desiredSpeed = maxSpeed * (distance / arrivalRadius);
        }

        // Calculate the desired velocity (from the agent toward the target)
        Vector3 desiredVelocity =
            Vector3.Normalize(target.position - agent.position) * desiredSpeed;

        // Calculate steering velocity
        Vector3 steeringVelocity = desiredVelocity - agent.velocity;

        // Apply steering velocity
        agent.velocity += steeringVelocity;
    }
}
