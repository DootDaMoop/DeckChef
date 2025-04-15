using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageTextComponent;
    public float floatSpeed = 2f;
    public float fadeDuration = 1f;
    public float scaleMultiplier = 1.5f;

    private void Awake() {
        if (damageTextComponent == null) {
            damageTextComponent = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void SetValue(int value) {
        damageTextComponent.text = value.ToString();
        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy() {
        transform.localScale = Vector3.one;
        LeanTween.scale(gameObject, Vector3.one * scaleMultiplier, 0.2f).setEaseOutBack();

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * floatSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            Color color = damageTextComponent.color;
            color.a = Mathf.Lerp(1f, 0f, t);
            damageTextComponent.color = color;

            yield return null;
        }
        Destroy(gameObject);
    }
}
