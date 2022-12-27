using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveManager : MonoBehaviour
{
    public Transform objectFrontVector;

    private float h = 0.0f;
    private float v = 0.0f;
    public float moveSpeed = 10.0f;

    private float xRotate, yRotate, xRotateMove, yRotateMove;
    public float rotateSpeed = 500.0f;

    public Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        /* debug */
        Debug.DrawLine(transform.position, objectFrontVector.position, Color.red);

        /* player 키보드 조작 */
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);

        if(!(h==0 && v == 0))
        {
            transform.position += dir * moveSpeed * Time.deltaTime;          
        }

        /* player 마우스 조작 */
        yRotateMove = Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed;

        yRotate = transform.eulerAngles.y + yRotateMove;

        xRotate = xRotate + xRotateMove;

        xRotate = Mathf.Clamp(xRotate, -90, 90); // 위, 아래 최소 최대 각도값 설정

        cam.transform.eulerAngles = new Vector3(xRotate, yRotate, 0);

        transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = this.transform.position;
    }
}
