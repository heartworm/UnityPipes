using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    
    public float hp;
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<Player>().HitPlayer(hp);
        } else if (other.CompareTag("Kill")) {
            Destroy(gameObject);
        }
    }

}
