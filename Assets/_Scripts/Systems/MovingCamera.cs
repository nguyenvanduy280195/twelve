using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    [SerializeField]
    private float _smoothSpeed;

    public Transform Target { private get; set; }

    void Update()
    {
        if (Target != null)
        {
            var delta = transform.position - Target.position;
            if (delta.sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x, Target.position.y, transform.position.z), _smoothSpeed);
            }
        }
    }
}
