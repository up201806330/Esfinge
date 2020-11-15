using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Animator animator;

    public float speed = 3f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float gravity = 1f;
    public float runSpeed = 2f;

    public float jumpPower = 1f;
    float g = -9.81f;
    bool isGrounded;
    float verticalVelocity;

    int wlkHash;
    int rngHash;
    int jmpHash;

    private void Start() {
        animator = GetComponent<Animator>();

        wlkHash = Animator.StringToHash("Walking");
        rngHash = Animator.StringToHash("Running");
        jmpHash = Animator.StringToHash("Jumping");
    }

    // Update is called once per frame
    void Update()
    {
        bool shift = Input.GetKey(KeyCode.LeftShift);
        bool space = Input.GetKeyDown(KeyCode.Space);
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime * (shift ? runSpeed : 1) + Vector3.up * g * Time.deltaTime);
        }
        else controller.Move(Vector3.up * g * Time.deltaTime);

        isGrounded = controller.isGrounded;
        if (isGrounded) verticalVelocity = 0f;
        if (space && !animator.GetBool(jmpHash)) {
            verticalVelocity += Mathf.Sqrt(jumpPower * -3.0f * g);
        }

        Debug.Log(verticalVelocity + "     " + animator.GetBool(jmpHash));
        verticalVelocity += g * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime * Vector3.up);

        animationStateMachine(direction.magnitude >= 0.1f, shift, space);
    }

    private void animationStateMachine(bool walkingNow, bool runningNow, bool jumpingNow) {
        bool isWalking = animator.GetBool(wlkHash);
        bool isRunning = animator.GetBool(rngHash);
        bool isJumping = animator.GetBool(jmpHash);

        if (!isJumping && jumpingNow) animator.SetBool(jmpHash, true);

        if (!isWalking && walkingNow) animator.SetBool(wlkHash, true);
        if (isWalking && !walkingNow) animator.SetBool(wlkHash, false);

        if (!isRunning && (walkingNow && runningNow)) animator.SetBool(rngHash, true);
        if (isRunning && (!walkingNow || !runningNow)) animator.SetBool(rngHash, false);
    }

    public void endRunningAnim() {
        animator.SetBool(jmpHash, false);
    }
}
