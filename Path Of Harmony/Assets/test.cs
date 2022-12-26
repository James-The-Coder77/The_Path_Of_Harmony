using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
  public Transform cam;
  public Rigidbody rb;
  public LayerMask metalMask;
  public Move script;
  public RaycastHit forwardHit;
  public RaycastHit downHit;
  public bool isPushing = false;
  public float massDiff = 0f;

  public float steelDist = 50f;
  public float steelStrength = 50f;
  public float capsuleRadius = 1f;
  public float coneAngle = 180f;
  public float pushForce = 0f;
  public float distModifier;

  public float maxSpeed;
  public float speedDamping = 5f;

  void Update()
  {
    ForwardPushing();
    DownWardsPushing();
    //forwardsPulling();
    float currentSpeed = rb.velocity.magnitude;
    float targetSpeed = Mathf.Min(currentSpeed, maxSpeed);
    rb.velocity = rb.velocity.normalized * Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedDamping);
  }

  void ForwardPushing(){
  if(Input.GetKey(KeyCode.Q)){
    Physics.CapsuleCast(cam.position, cam.position + Vector3.up * Mathf.Tan(coneAngle * Mathf.Deg2Rad) * steelDist, capsuleRadius, cam.forward, out forwardHit, steelDist, metalMask);
    if(forwardHit.transform != null && forwardHit.transform.tag == "Metal" && forwardHit.transform.GetComponent<Rigidbody>() != null){
      isPushing = true;
      massDiff = forwardHit.rigidbody.mass - rb.mass;
      pushForce = 0; // initialize pushForce to 0
      Vector3 pushDirection = transform.position - forwardHit.point;
      distModifier = Vector3.Distance(transform.position, forwardHit.point);
      distModifier /= 2;

      // calculate pushForce based on player and object masses
      if (rb.mass >= forwardHit.rigidbody.mass) {
        pushForce = steelStrength * massDiff / (rb.mass + forwardHit.rigidbody.mass);
      } else {
        pushForce = steelStrength * massDiff / (forwardHit.rigidbody.mass + rb.mass);
      }

      // apply push forces to player and object
      rb.AddForce(pushDirection * pushForce, ForceMode.Force);
      forwardHit.rigidbody.AddForce(-pushDirection * pushForce, ForceMode.Force);
    }else{
      isPushing = false;
    }
  }
}

  void DownWardsPushing(){
    if(!script.isGrounded && Input.GetKey(KeyCode.Space) && !script.isJumping){
      Physics.Raycast(transform.position, -transform.up, out downHit, steelDist, metalMask);
      if(downHit.transform != null && downHit.transform.tag == "Metal" && downHit.transform.GetComponent<Rigidbody>() != null){
        isPushing = true;
        massDiff = downHit.rigidbody.mass - rb.mass;
        pushForce = 0; // initialize pushForce to 0
        Vector3 pushDirection = transform.position - downHit.point;
        distModifier = Vector3.Distance(transform.position, downHit.point);
        distModifier /= 2;

        // calculate pushForce based on player and object masses
        if (rb.mass >= downHit.rigidbody.mass) {
          pushForce = steelStrength * massDiff / (rb.mass + downHit.rigidbody.mass);
        } else {
          pushForce = steelStrength * massDiff / (downHit.rigidbody.mass + rb.mass);
        }

        // apply push forces to player and object
        rb.AddForce(pushDirection * pushForce, ForceMode.Force);
        downHit.rigidbody.AddForce(-pushDirection * pushForce, ForceMode.Force);
      }else{
        isPushing = false;
      }
    }
  }
}
