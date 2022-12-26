using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCoins : MonoBehaviour
{
  //public GameObject coin;
  public GameObject combatCam;
  public GameObject normalCam;
  public GameObject coin;
  public Transform coinPos;
  public SteelIronAllomancy script;

  void Update()
  {
    if(Input.GetKey(KeyCode.Mouse1)){
      normalCam.SetActive(false);
      combatCam.SetActive(true);
    }else{
      normalCam.SetActive(true);
      combatCam.SetActive(false);
    }
    combatCam.transform.position = normalCam.transform.position;
  }

  void SummonCoin(){
    GameObject currentCoin = Instantiate(coin, coinPos.position, Quaternion.identity);
    //currentCoin.GetComponent<Rigidbody>().AddForce(transform.forward, )
  }
}
