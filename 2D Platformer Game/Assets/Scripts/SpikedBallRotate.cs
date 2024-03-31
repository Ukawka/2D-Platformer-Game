using System;
using UnityEngine;

public class SpikedBallRotate : MonoBehaviour
{
    [SerializeField] private float totalAngle = 0;
    [SerializeField] private float g = 9.8f;
    private int flag = -1;
    private float length = 0;
    private float currentAngle = 0;
    private float acceleratedSpeed = 0;
    private float speed = 0f;
    private void Start()
    {
        totalAngle = 90;
        length = -GameObject.Find("Spiked Ball").transform.position.y;
        Vector3 initRotation = new Vector3(0, 0, 0);
        transform.eulerAngles = initRotation;
        speed = -(float)Math.Sqrt(2 * g * length * Math.Cos(totalAngle / 180 * (float)Math.PI / 2)) / length;
    }
    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0,  speed / (float)Math.PI * 180) * Time.deltaTime);
        currentAngle += speed * Time.deltaTime;
        acceleratedSpeed = -(float)Math.Sin(currentAngle) * g;
        speed += acceleratedSpeed * Time.deltaTime / length;
        // Debug.Log("------");
        // Debug.Log(currentAngle);
        // Debug.Log(acceleratedSpeed);
        // Debug.Log(speed);
    }
}
