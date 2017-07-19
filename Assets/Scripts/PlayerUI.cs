using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HIT_TYPE { DAMAGE, HEAL, GRAZE }

public class PlayerUI : MonoBehaviour {
    public Light mainLight;
    public Text healthText;
    public Text scoreText;
    public Canvas deadMenu;
    public Image hurtPanel;
    public float hurtPanelFadeSpeed;
    public Color hurtPanelDamageColor;
    public Color hurtPanelHealColor;

    public Camera mainCamera;
    public Canvas mainUI;
    public HitText hitTextPrefab;

    public Color positiveColour;
    public Color negativeColour;


    public float Score {
        set {
            scoreText.text = FloatToUIString(value);
        }
    }

    public float Health {
        set {
            healthText.text = FloatToUIString(value);
        }
    }

    private void Update() {
        hurtPanel.color = Color.Lerp(hurtPanel.color, Color.clear, hurtPanelFadeSpeed * Time.deltaTime);
    }
    
    public void SetPositive(bool positive) {
        Color colour = positive ? positiveColour : negativeColour;
        ParticleSystem.ColorOverLifetimeModule psCol = GetComponent<ParticleSystem>().colorOverLifetime;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(colour, 0), new GradientColorKey(Color.white, 1) },
            new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
        psCol.color = new ParticleSystem.MinMaxGradient(grad);

        mainLight.color = colour;
    }

    public void OnDeath() {
        deadMenu.enabled = true;
    }

    public void OnStartClick() {
        deadMenu.enabled = false;
        GetComponent<Player>().StartGame();
    }
    
    public void OnGraze() {
        ShowHitText("Close!", Color.white);
    }

    public void OnHit(float hp) {
        bool heal = hp > 0;
        ShowHitText(FloatToUIString(Mathf.Abs(hp)), heal ? Color.green : Color.red);
        FlashHurtPanel(heal);
    }

    private static string FloatToUIString(float value) {
        return Mathf.CeilToInt(value).ToString();
    }

    private void ShowHitText(string text, Color colour) {
        HitText ht = Instantiate(hitTextPrefab, mainUI.transform);
        Vector2 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        screenPos.x /= mainCamera.pixelWidth;
        screenPos.y /= mainCamera.pixelHeight;
        ht.DisplayHitText(text, colour, screenPos);
    }

    private void FlashHurtPanel(bool heal) {
        if (heal) {
            hurtPanel.color = hurtPanelDamageColor;
        } else {
            hurtPanel.color = hurtPanelHealColor;
        }
    } 

}
