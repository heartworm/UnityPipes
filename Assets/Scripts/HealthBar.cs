using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Text healthText;
    public Color healthyColour;
    public Color deadColour;

    private float initWidth;
    private RectTransform rt;
    private Image img;

    public void Awake() {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        initWidth = rt.sizeDelta.x;
        SetHealth(100);
    }

    public void SetHealth(float hp) {
        float hpNormalised = hp / 100.0f;
        healthText.text = PlayerUI.FloatToUIString(hp);
        rt.sizeDelta = new Vector2(Mathf.Lerp(0, initWidth, hpNormalised), rt.sizeDelta.y);
        img.color = Color.Lerp(deadColour, healthyColour, hpNormalised);
    }

}
