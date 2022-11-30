using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;

    private void Start()
    {
        //Hide Mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        //Quaternion property: A pack of x,y,z,w, one value change will affect the others
        //Unity rotate by Quaternion, then need to use eulerAngles to force Quaternion by x,y,z rotation
        //Otherwise, change rotation only by x,y,z cannot perform the true behavior because of Quaternion property
        transform.rotation =
            Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + mouseInput.x, transform.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

        if (invertLook)
        {
            viewPoint.rotation =
                Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y,
                    viewPoint.rotation.eulerAngles.z);
        }
        else
        {
            viewPoint.rotation =
                Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y,
                    viewPoint.rotation.eulerAngles.z);
        }
    }
}