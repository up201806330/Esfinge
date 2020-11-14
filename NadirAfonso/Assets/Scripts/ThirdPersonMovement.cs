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

    int wlkHash;
    int rngHash;

    private void Start() {
        animator = GetComponentInChildren<Animator>();

        wlkHash = Animator.StringToHash("Walking");
        rngHash = Animator.StringToHash("Running");
    }

    // Update is called once per frame
    void Update()
    {
        bool shift = Input.GetKey(KeyCode.LeftShift);
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        animationStateMachine(direction.magnitude >= 0.1f, shift);

        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime * (shift ? runSpeed: 1));

            controller.Move(new Vector3(0, -gravity, 0));
        }
    }

    private void animationStateMachine(bool walkingNow, bool runningNow) {
        bool isWalking = animator.GetBool(wlkHash);
        bool isRunning = animator.GetBool(rngHash);

        if (!isWalking && walkingNow) animator.SetBool(wlkHash, true);
        if (isWalking && !walkingNow) animator.SetBool(wlkHash, false);

        if (!isRunning && (walkingNow && runningNow)) animator.SetBool(rngHash, true);
        if (isRunning && (!walkingNow || !runningNow)) animator.SetBool(rngHash, false);
    }
}
