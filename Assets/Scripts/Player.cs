using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public Pipe pipePrefab;
    public GameObject pipeSystemPrefab;
    private PipeSystem pipeSystem;

    public float floatingHeight;
    public float moveSpeed;
    public float maxPipeSpeed;
    public float minPipeSpeed;
    public float pipeSpeedIncreaseFactor;
    
    
    private float distance = 0;
    private float pipeSpeed;
    private bool dead = true;

    public float switchCooldown = 3;
    private float switchCooldownCounter;

    private bool positive = true;
    private float trackRadius;
    private float currentAngle = 0;

    private PlayerUI pUI;
    private PlayerHealth pHealth;

    private void Awake() {
        pUI = GetComponent<PlayerUI>();
        pHealth = GetComponent<PlayerHealth>();
        trackRadius = CalculateInnerRadius() - floatingHeight;
        Init();
    }

    private void Init() {
        if (pipeSystem != null)
            Destroy(pipeSystem.gameObject);
        GameObject dawr = Instantiate(pipeSystemPrefab);
        pipeSystem = dawr.GetComponent<PipeSystem>();

        SetPosition(0);
        positive = true;
        pHealth.OnInit();
        distance = 0;
        pUI.SetPositive(positive);
        switchCooldownCounter = switchCooldown;
        pipeSpeed = minPipeSpeed;
    }

    public void StartGame() {
        dead = false;
        Init();
    }

    private void Update() {
        if (!dead) {
            float newAngle = (currentAngle - Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime) % (2 * Mathf.PI);
            SetPosition(newAngle);

            if (Input.GetMouseButtonDown(0) && switchCooldownCounter == switchCooldown) {
                TogglePositive();
                switchCooldownCounter = 0;
            }

            if (!positive) {
                pHealth.DecayHealth();
            }
            
            pipeSpeed = Mathf.Clamp(pipeSpeed + (pipeSpeedIncreaseFactor * Time.deltaTime), minPipeSpeed, maxPipeSpeed);

            float movedDistance = pipeSpeed * Time.deltaTime;
            pipeSystem.MoveSystem(movedDistance);
            distance += movedDistance;

            switchCooldownCounter = Mathf.Clamp(switchCooldownCounter + Time.deltaTime, 0, switchCooldown);

            UpdateUI();

            if (pHealth.Health == 0) {
                dead = true;
                pUI.OnDeath();
            }
        }
    }

    private void UpdateUI() {
        pUI.Score = distance;
        pUI.Health = pHealth.Health;
    }

    private void SetPosition(float angle) {
        currentAngle = angle;
        transform.position = new Vector3(0, -Mathf.Cos(angle) * trackRadius, Mathf.Sin(angle) * trackRadius);
    }

    private void TogglePositive() {
        positive = !positive;
        pUI.SetPositive(positive);
    }

    private float CalculateInnerRadius() {
        float anglePerSegment = (2 * Mathf.PI) / pipePrefab.pipeSegmentCount;
        return pipePrefab.pipeRadius * Mathf.Cos(anglePerSegment / 2);
    }

    public void HitPlayer(float hp) {
        float dHp = positive ? hp : -hp;
        pHealth.IncrementHealth(dHp);
        pUI.FlashHurtPanel(dHp > 0);
    }


}
