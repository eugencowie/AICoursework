using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Seek : MonoBehaviour
{
    public Transform Target;
    public Action OnArrival;

    [SerializeField]
    private float m_speed = 4;

    [SerializeField]
    private float m_arrivalRadius = 2.0f;

    private Collider m_collider;
    private Rigidbody m_rigidbody;
    
    private void Start()
    {
        m_collider = GetComponent<Collider>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Calculate the target position on the ground
        Vector3 targetPosition = Target.position + new Vector3(0, m_collider.bounds.extents.y, 0);

        // Get the offset from the agent to the target
        Vector3 offset = targetPosition - transform.position;

        // Initially set the desired speed to max speed
        float desiredSpeed = m_speed;

        // If within the arrival distance
        if (offset.magnitude < m_arrivalRadius)
        {
            desiredSpeed = m_speed * (offset.magnitude / m_arrivalRadius);
        }

        // Get the desired velocity for seek and limit to desired speed
        Vector3 desiredVelocity = offset.normalized * desiredSpeed;

        // Calculate steering velocity and limit it according to how much it can turn
        Vector3 steeringVelocity = desiredVelocity - m_rigidbody.velocity;

        // Apply steering velocity
        m_rigidbody.velocity += steeringVelocity;

        // Disable the target when we get close
        if (offset.magnitude <= 1.01f)
        {
            OnArrival?.Invoke();
        }
    }
}
