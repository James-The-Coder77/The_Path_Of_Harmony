using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testDot : MonoBehaviour
{
  public Transform testCube;

  void Update()
  {
    Debug.Log(Vector3.Dot((testCube.position - transform.position).normalized, transform.forward));
  }
}
