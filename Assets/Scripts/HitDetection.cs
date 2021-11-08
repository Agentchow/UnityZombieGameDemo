using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Zombie") {
            Debug.Log("ENTER");
        }
    }
    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Zombie") {
            Debug.Log("STAY");
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Zombie") {
            Debug.Log("EXIT");
        }
    }
}
