using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraOrbit : MonoBehaviour
{
    public Transform target; // The point around which the camera will rotate
    public float rotationSpeed = 10f;

    void Update()
    {
        // Rotate the camera around the target point
        transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
