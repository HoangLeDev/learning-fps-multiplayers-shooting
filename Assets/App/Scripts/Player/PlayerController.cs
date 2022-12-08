using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform viewPoint;
    public float mouseSensitivity;
    private float verticalRotStore;
    private Vector2 mouseInput;
    private Vector3 moveDir, movement;
    private float walkSpeed, runSpeed, crunchSpeed, activeMoveSpeed;
    private float jumpForce, gravityMod;

    public bool invertLook;
    private bool isGrounded;
    private bool isCrunch;

    public CharacterController charCon;
    public Transform groundCheckPoint;
    public LayerMask groundLayer;

    [SerializeField] private PlayerShooting playerShootManager;

    private Camera cam;

    [Header("Guns")]
    //Guns
    public Gun[] allGuns;

    public int selectedGun;

    #region Main Function Calls

    private void Start()
    {
        Initialize();
    }

    private void LateUpdate()
    {
        MovingCamera();
    }

    private void Update()
    {
        PlayerView();
        PlayerMovement();
        SwitchGun();
        playerShootManager.ShootExecute();
    }

    private void FixedUpdate()
    {
        PlayerPhysics();
    }

    #endregion

    #region PlayerMovement

    private void Initialize()
    {
        //Setup Camera
        cam = Camera.main;

        //Hide Mouse cursor while playing
        Cursor.lockState = CursorLockMode.Locked;

        //Setup Player Properties
        mouseSensitivity = 5f;
        walkSpeed = 1f;
        runSpeed = 1.5f;
        jumpForce = 5f;
        gravityMod = 2f;
        crunchSpeed = 0.3f;

        ChangeGunModel();
    }

    private void MovingCamera()
    {
        cam.transform.position = viewPoint.transform.position;
        cam.transform.rotation = viewPoint.transform.rotation;
    }

    private void PlayerView()
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

    private void PlayerMovement()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        //Run or Walk
        if (Input.GetKey(KeyCode.LeftShift) && !isCrunch)
            activeMoveSpeed = runSpeed;
        else if (!isCrunch)
            activeMoveSpeed = walkSpeed;
        else
            activeMoveSpeed = crunchSpeed;


        //Crunch
        if (isGrounded && Input.GetKey(KeyCode.LeftControl))
        {
            isCrunch = true;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            activeMoveSpeed = crunchSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrunch = false;
            transform.localScale = Vector3.one;
        }

        /*Note
        * vector transform.forward and transform.right have y value is 0, then it will reset movement.y every call
        * need to create yVelocity to store previous call's movement.y
        */

        float yVelocity = movement.y;
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
        movement.y = yVelocity;


        //Reset movement.y (gravity) when player is on ground
        if (charCon.isGrounded)
        {
            movement.y = 0;
        }

        //Jump
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            movement.y = jumpForce;
        }


        charCon.Move(movement * Time.fixedDeltaTime);
    }


    private void PlayerPhysics()
    {
        //Ground check
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayer);
        //Gravity
        movement.y += Physics.gravity.y * Time.fixedDeltaTime * gravityMod;
    }

    #endregion

    #region Gun

    public void SwitchGun()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;
            if (selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }

            ChangeGunModel();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if (selectedGun <= 0)
            {
                selectedGun = allGuns.Length - 1;
            }

            ChangeGunModel();
        }
    }

    private void ChangeGunModel()
    {
        foreach (Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);
    }

    #endregion
}