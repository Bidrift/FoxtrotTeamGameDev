using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 4f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public GameObject playerObject;
    public Camera playerCamera;
    public GameObject playerWeapon;
    public GameObject playerLeaner;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isMoving;
    public bool isCrouched = false;
    Vector3 velocity;

    bool isGrounded;

    public Animator cameraAnimator;
    private Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        lastPosition = transform.position;
        cameraAnimator = playerLeaner.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var movementSpeed = speed;
        if (isCrouched)
        {
            movementSpeed /= 5;
        }
        if (InputManager.instance.crouching)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                characterController.height = 2f;
                playerObject.transform.localScale = new Vector3(playerObject.transform.localScale.x, 1f, playerObject.transform.localScale.z);
                playerCamera.transform.position = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y - 1, playerCamera.transform.position.z);
                jumpHeight = 2f;
                isCrouched = true;
                InputManager.instance.toggleWalking();
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                characterController.height = 3.7f;
                playerObject.transform.localScale = new Vector3(playerObject.transform.localScale.x, 1f, playerObject.transform.localScale.z);
                playerCamera.transform.position = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y + 1, playerCamera.transform.position.z);
                jumpHeight = 3f;
                isCrouched = false;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) && InputManager.instance.walking)
        {
            movementSpeed /= 3;
        }

        // Fix jumping semophore and leaning desync
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (isGrounded && Input.GetKeyDown(KeyCode.E) && !Input.GetKey(KeyCode.Q))
        {
            cameraAnimator.SetTrigger("LookRight");
        }
        if (isGrounded && Input.GetKeyDown(KeyCode.Q) && !Input.GetKey(KeyCode.E))
        {
            cameraAnimator.SetTrigger("LookLeft");
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            cameraAnimator.SetTrigger("StopLookRight");
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            cameraAnimator.SetTrigger("StopLookLeft");
        }
        if (InputManager.instance.running)
        {
            var hor = Input.GetAxis("Horizontal");
            var ver = Input.GetAxis("Vertical");
            Vector3 move = transform.right * hor + transform.forward * ver;

            characterController.Move(move * movementSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && InputManager.instance.jumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            InputManager.instance.toggleJumping();
        }

        velocity.y += gravity * Time.deltaTime;   

        characterController.Move(velocity * Time.deltaTime);

        if (lastPosition != gameObject.transform.position && InputManager.instance.jumping)
        {
            isMoving = true;
        } else
        {
            isMoving = false;
        }
        lastPosition = gameObject.transform.position;

    }

    CharacterController characterController;

}
