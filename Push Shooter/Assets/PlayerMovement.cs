using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float maxSpeed = 10;
    [SerializeField] float jumpForce = 15;

    Animator anim;
    CharacterController controller;

    Transform mainCamera;

    float speedY;
    [SerializeField] float gravity = -9.81f;

    bool isAiming = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        VerticalMovement();

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            isAiming = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            isAiming = false;
        }
    }


    void MovePlayer()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        float input = new Vector2(inputX, inputZ).magnitude;
        if (input > 1) input = 1;
        anim.SetFloat("speed", input);

        Movement(inputX, inputZ, input);
        
    }

    void Movement(float inputX, float inputZ, float input)
    {
        
        Vector3 direction = new Vector3(inputX, 0, inputZ).normalized;
        
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angleWithCamera = targetAngle + mainCamera.rotation.eulerAngles.y;

        anim.SetFloat("rotation", targetAngle);

        transform.rotation = Quaternion.Euler(0, mainCamera.rotation.eulerAngles.y, 0);

        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDir = Quaternion.Euler(0, angleWithCamera, 0) * Vector3.forward;
            controller.Move(moveDir * maxSpeed * input * Time.deltaTime / 2);
        }
    }


    void VerticalMovement()
    {
        speedY += gravity * Time.deltaTime;

        LayerMask groundLayer = LayerMask.GetMask("Ground");
        bool isGrounded = Physics.CheckSphere(transform.position, 0.25f, groundLayer);
        anim.SetBool("falling", !isGrounded);

        if (isGrounded && speedY < jumpForce - 1)
        {
            speedY = -6;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                speedY = jumpForce;
                anim.SetTrigger("jump");
            }
        }

        Vector3 velocityDown = new Vector3(0, speedY, 0);
        controller.Move(velocityDown * Time.deltaTime);
    }

}
