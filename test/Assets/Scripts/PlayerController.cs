using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour {

    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    internal Transform playerHead;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private float groundDistance;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private float senitivityX;

    [SerializeField]
    private float senitivityY;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float walkSpeedDivisor;

    [SerializeField]
    private float airSpeedDivisor;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float stepHeight;

    [SerializeField]
    private float gravity = -9.81f;

    private float walkSpeed {
        get {
            return speed / walkSpeedDivisor;
        }
    }

    private float airSpeed {
        get {
            return speed / airSpeedDivisor;
        }
    }

    private Vector3 velocity;

    private bool isGrounded;

    private bool isRunning;

    private bool isAiming;

    private float headRotation = 0f;


    //Inputs
    private float xInput = 0f;

    private float yInput = 0f;

    private float mouseX = 0f;

    private float mouseY = 0f;

    private bool sprint;//Input.GetButton("Sprint")

    private bool aim;//Input.GetButton("Fire2")

    private bool jump;//Input.GetButtonDown("Jump")

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {

        Look();
        Move();

        Animate();

    }

    private void Look() {

        float mouseX = this.mouseX * senitivityX * Time.deltaTime;
        float mouseY = this.mouseY * senitivityY * Time.deltaTime;

        headRotation -= mouseY;
        headRotation = Mathf.Clamp(headRotation, -90f, 90f);

        playerHead.localRotation = Quaternion.Euler(headRotation, 0, 0);
        this.transform.Rotate(Vector3.up * mouseX);//rotate the whole player and everything in it (the body moves with the head, etc.)
        playerHead.Rotate(Vector3.forward * mouseY);

    }

    private void Move() {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            animator.SetBool("isJumping", !isGrounded);

        if (aim && isGrounded) {
            isAiming = true;
        }
        else {
            isAiming = false;
        }

        if (isGrounded && velocity.y < 0) {
            controller.slopeLimit = 45.0f;
            controller.stepOffset = stepHeight;
            velocity.y = -2f;
        }

        if (isRunning && isGrounded) {
            controller.Move(Vector3.Normalize(xInput * transform.right + yInput * transform.forward) * speed * Time.deltaTime);
        }
        else if (!isGrounded) {
            controller.Move(Vector3.Normalize(xInput * transform.right + yInput * transform.forward) * airSpeed * Time.deltaTime);
        }
        else {
            controller.Move(Vector3.Normalize(xInput * transform.right + yInput * transform.forward) * walkSpeed * Time.deltaTime);///Walk speed if in air or walking
        }

        if (sprint && xInput == 0 && yInput > 0 && !isAiming) {//if sprint button and we're not walking sideways, backwards or aiming
            isRunning = true;
        }
        else {
            isRunning = false;
        }

        if (jump && isGrounded) {
            controller.slopeLimit = 100.0f;
            controller.stepOffset = 0;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);//jump;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }

    private void Animate() {
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isAiming", isAiming);
        animator.SetFloat("Horizontal", xInput);
        animator.SetFloat("Vertical", yInput);
    }

    //all input is passed in via this funtion to make the PlayerController general enough to control players and bots
    internal void SetInput(float horizontal, float vertical, float mouseX, float mouseY, bool sprint, bool aim, bool jump) {
        xInput = horizontal;
        yInput = vertical;
        this.mouseX = mouseX;
        this.mouseY = mouseY;
        this.sprint = sprint;
        this.aim = aim;
        this.jump = jump;
    }

}
