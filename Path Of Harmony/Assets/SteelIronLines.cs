using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelIronLines : MonoBehaviour
{
  public float steelRange = 50f;
  public float offset = 0.2f;
  public LayerMask steelMask;
  public Material unusedLineMat;
  static LineRenderer lr;
  private Collider[] cols;
  Vector3[] positions;

  void Awake()
  {
    // Create the line renderer and set its material
    lr = new GameObject("Line").AddComponent<LineRenderer>();
    lr.material = unusedLineMat;
  }

  void Update()
  {
    cols = Physics.OverlapSphere(transform.position, steelRange, steelMask);

    // Set the position count and width of the line renderer
    lr.positionCount = cols.Length * 2;
    lr.startWidth = 0.025f;
    lr.endWidth = 0.025f;

    // Set the positions of the line renderer using the SetPositions method
    positions = new Vector3[cols.Length * 2];
    for(int i = 0; i < cols.Length; i++){
      positions[i * 2] = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
      positions[i * 2 + 1] = cols[i].transform.position;
    }
    lr.SetPositions(positions);
  }
}
