using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public float hp;
    public int grazePoints;

    private Player player;
    private bool hit = false;
    private bool grazed = false;

    private void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            hit = true;
            player.Hit(hp);
        } else if (other.CompareTag("Kill")) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Graze") && !hit && !grazed) {
            player.Graze(grazePoints);
            grazed = true;
        }
    }
    
}
