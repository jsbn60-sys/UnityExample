using UnityEngine;
using System.Collections;
 
/// <summary>
/// Camera follower example. Not important for development. Just for reference.
/// </summary>
public class TransformFollower : MonoBehaviour
{

    private Transform target;
 
    [SerializeField]
    private Vector3 offsetPosition;

    public Transform Target
    {
        set => target = value;
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.TransformPoint(offsetPosition);
            transform.LookAt(target);   
        }
    }
}