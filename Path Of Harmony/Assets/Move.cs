using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Move : MonoBehaviour
{
  public Rigidbody rb;
  public Transform cam;
  public Transform PlayerModel;
  public SteelIronAllomancy script;
  public Animator anim;

  public float speed = 6;
  public float jumpTime = 0.5f;
  public float landingTime = 0.2f;
  public float offset = 0.2f;
  public float gravScale = -26.21f;
  public float gravity = -9.81f;
  public float jumpHeight = 3;
  public float airMultiplier = 0.4f;
  public float groundDrag = 6f;
  public float airDrag = 2f;
  public bool isGrounded;
  public bool groundedLastFrame;
  public bool isJumping;

  public Transform groundCheck;
  public float groundDistance = 0.4f;
  public LayerMask groundMask;

  public float sprintSpeed = 25f;
  public float origSpeed = 15f;

  Vector3 prevMovement;
  Vector3 moveDir;

  float turnSmoothVelocity;
  public float turnSmoothTime = 0.1f;

  public float maxSlopeAngle;
  public float playerHeight = 1f;
  RaycastHit slopeHit;

  public bool onSlope(){
    if(Physics.Raycast(transform.position, Vector3.down, out slopeHit,  playerHeight * 0.5f + 0.3f)){
      float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
      return angle < maxSlopeAngle && angle != 0;
    }

    return false;
  }

  private Vector3 getSlopeAngle(){
    return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
  }

// Update is called once per frame
  void Update(){
    groundedLastFrame = isGrounded;
    Cursor.lockState = CursorLockMode.Locked;
    PlayerModel.position = transform.position;
    if(!script.isPushing){
      anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x + rb.velocity.z));
      gravity = gravScale;
    }else{
      gravity = 0f;
    }
    //jump
    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    if(Input.GetKey(KeyCode.LeftShift) && isGrounded){
      speed = sprintSpeed;
    }else{
      speed = origSpeed;
    }

    if (Input.GetButtonDown("Jump") && isGrounded){
      rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
      isJumping = true;
      anim.SetBool("isJumping", true);
      Invoke(nameof(jumpEnd), jumpTime);
    }

    if(groundedLastFrame == false && isGrounded == true){
      anim.SetBool("hasLanded", true);
      Invoke(nameof(resetLand), landingTime);
    }

    if(!isGrounded) anim.SetBool("inAir", true);
    if(isGrounded) anim.SetBool("inAir", false);

    if(Input.GetKeyUp(KeyCode.Space)){
      isJumping = false;
    }
    //extra gravity
    rb.AddForce(transform.up * gravity);
    //walk
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");
    Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

    if(direction.magnitude >= 0.1f){
        float targetAngle = Mathf.Atan2(direction.x + offset, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        if(Time.timeScale > 0.5f) transform.rotation = Quaternion.Euler(0f, angle, 0f);

        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        if(isGrounded && !onSlope() && Time.timeScale > 0.5f)
          rb.AddForce(moveDir.normalized * speed);
        else if(!isGrounded && Time.timeScale > 0.5f)
          rb.AddForce(moveDir.normalized * speed * airMultiplier);
        else if(isGrounded && onSlope() && Time.timeScale > 0.5f)
          rb.AddForce(getSlopeAngle() * speed);
          rb.useGravity = false;
    }
    if(isGrounded)
      rb.drag = groundDrag;
    else
      rb.drag = airDrag;
  }

  void jumpEnd(){
    anim.SetBool("isJumping", false);
  }

  void resetLand(){
    anim.SetBool("hasLanded", false);
  }


}
