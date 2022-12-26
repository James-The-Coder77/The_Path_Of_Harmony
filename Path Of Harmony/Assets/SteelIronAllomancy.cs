using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelIronAllomancy : MonoBehaviour

{
  public Transform cam;
  public GameObject debugCircle;
  public Rigidbody rb;
  public LayerMask metalMask;
  public Move script;
  public RaycastHit forwardHit;
  public RaycastHit downHit;
  public RaycastHit backHit;
  public bool isPushing = false;
  public float massDiff = 0f;
  public float slowModeSpeed = 30f;

  public float metalDist = 50f;
  public float metalStrength = 50f;
  public float aimAssistSize = 10f;
  public float backSize = 30f;
  public float pushForce = 0f;
  public float distModifier;
  public float maxSpeed = 50f;

  private Collider[] cols;
  private Vector3 toTarget;

  void Update()
  {
    ForwardPushing();
    DownWardsPushing();
    ForwardsPulling();
    cols = Physics.OverlapSphere(transform.position, metalDist, metalMask);
    if(!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Space)) isPushing = false;
    if(!script.isGrounded) if(Input.GetKey(KeyCode.Mouse1) && rb.velocity.magnitude < slowModeSpeed) Time.timeScale = 0.25f;
    else Time.timeScale = 1f;
  }

  void ForwardPushing(){
    if(Input.GetKey(KeyCode.Q)){
      Physics.SphereCast(transform.position, aimAssistSize, cam.forward, out forwardHit, metalDist, metalMask);
      ActivateAllomancy(KeyCode.Q, forwardHit, 1f, 1f);
    }
  }

  void ForwardsPulling(){
    if(Input.GetKey(KeyCode.E)){
      Physics.SphereCast(transform.position, aimAssistSize, cam.forward, out forwardHit, metalDist, metalMask);
      ActivateAllomancy(KeyCode.E, forwardHit, -1f, -1f);
    }
  }

  void DownWardsPushing(){
    if(!script.isGrounded && Input.GetKey(KeyCode.Space) && !script.isJumping){
      Physics.Raycast(transform.position, -transform.up, out downHit, metalDist, metalMask);
      ActivateAllomancy(KeyCode.Space, downHit, 1f, 1f);
      for(int i = 0; i < cols.Length; i++){
        toTarget = (cols[i].transform.position - transform.position).normalized;
        if(Vector3.Dot(toTarget, (-transform.forward + -transform.up).normalized) > 0.75f) {
          Physics.Raycast(transform.position, (cols[i].transform.position - transform.position).normalized, out backHit, metalDist, metalMask);
          ActivateAllomancy(KeyCode.Space, backHit, 1f, 1f);
        }
      }
    }
  }

  void ActivateAllomancy(KeyCode activationKey, RaycastHit hit, float PlayerDir, float objDir){
    if(hit.transform != null && hit.transform.tag == "Metal" && hit.transform.GetComponent<Rigidbody>() != null && Mathf.Abs(rb.velocity.x + rb.velocity.z) + rb.velocity.y < maxSpeed){
      isPushing = true;
      massDiff = hit.rigidbody.mass - rb.mass;
      pushForce = metalStrength - Mathf.Abs(massDiff);
      Vector3 pushDirection = transform.position - hit.point;
      distModifier = Vector3.Distance(transform.position, forwardHit.point);
      distModifier /= 2;
      rb.AddForce((pushDirection * PlayerDir * Mathf.Abs(massDiff)/distModifier), ForceMode.Force);
      if(pushForce > 0){
          hit.rigidbody.AddForce((-pushDirection * objDir * Mathf.Abs(pushForce)/distModifier), ForceMode.Force);
      }
    }
  }
}
