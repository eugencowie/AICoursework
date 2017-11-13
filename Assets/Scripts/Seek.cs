using UnityEngine;

public class Seek : MonoBehaviour
{
    public Vector3 Target;

    [SerializeField]
    private float m_speed = 4;

    private void Start()
    {
        if (Target == Vector3.zero)
        {
            Target = transform.position;
        }
    }

    private void Update()
    {
        Vector3 difference = Target - transform.position;
        Vector3 step = difference.normalized * m_speed * Time.deltaTime;
        if (difference.magnitude > step.magnitude)
        {
            transform.position += step;
        }
    }
}
