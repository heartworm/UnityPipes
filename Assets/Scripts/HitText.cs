using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitText : MonoBehaviour {

    public float verticalOffset = 0.1f;

    public void DisplayHitText(string text, Color colour, Vector2 position) {
        RectTransform rt = GetComponent<RectTransform>();
        Text hitText = GetComponent<Text>();

        hitText.text = text;
        hitText.color = colour;

        position.y += verticalOffset;

        rt.anchorMax = position;
        rt.anchorMin = position;

        gameObject.SetActive(true);
    }

    private void EndAnimation() {
        Destroy(gameObject);
    }
        
}
