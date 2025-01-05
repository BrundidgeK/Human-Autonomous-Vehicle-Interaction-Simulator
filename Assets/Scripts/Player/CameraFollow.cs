using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private float maxRotationDiff = 60;

    // Start is called before the first frame update
    void Start()
    {
        //maxRotationDiff *= Mathf.Deg2Rad;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 tarPos = (Vector2)target.position + offset;
        transform.position = new Vector3(tarPos.x, tarPos.y, -10);
        if(Mathf.Abs(transform.rotation.z-target.rotation.z) > maxRotationDiff)
        {
            float scalar = transform.rotation.z - (transform.rotation.z % 90);
            transform.rotation = new Quaternion(0, 0, scalar, 0);
        }
    }
}
