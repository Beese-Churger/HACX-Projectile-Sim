using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 2.0f;

    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            // Basic movement
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            transform.Translate(new Vector3(horizontal, 0, vertical) * Time.deltaTime * movementSpeed);
        }
        // Rotation
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.Rotate(0, mouseX, 0, Space.World);
            transform.Rotate(mouseY, 0, 0);

            // E to move upwards
            if (Input.GetKey(KeyCode.E))
            {
                transform.Translate(Vector3.up * Time.deltaTime * movementSpeed, Space.World);
            }

            // Q to move downwards
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(Vector3.down * Time.deltaTime * movementSpeed, Space.World);
            }
            if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
}
