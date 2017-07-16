using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    private float health = 100;
    public float healthDecay = 15;

    public float Health {
        get { return health; }
    }

    public void OnInit() {
        health = 100;
    }

    public void IncrementHealth(float hp) {
        health = Mathf.Clamp(health + hp, 0, 100);
    }

    public void DecayHealth() {
        IncrementHealth(-healthDecay * Time.deltaTime);
    }

}
