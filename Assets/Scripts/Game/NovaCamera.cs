using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaCamera : MonoBehaviour
{
    public new Transform transform;
    public Vector3 basePos;

    public float zRotation;

    private float rotationSpeed = 360f;

    void Start()
    {
        transform = GetComponent<Transform>();
        basePos = transform.localPosition;

        zRotation = 0f;
    }

    void Update()
    {
        transform.localPosition = basePos + (Vector3) (Random.insideUnitCircle * NovaGame.GetShakeMagnitude());

        float zAngle = transform.eulerAngles.z;
        float diff = zRotation - zAngle;
        float delta = (diff < 0 ? -1 : 1) * Mathf.Min(rotationSpeed * Time.deltaTime, Mathf.Abs(diff));

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zAngle + delta);
    }

    public void Flip()
    {
        zRotation += 180;

        if(zRotation >= 360)
        {
            zRotation -= 360;
        }
    }

    public void OnReset()
    {
        zRotation = 0f;
        transform.eulerAngles = Vector3.zero;
    }
}
