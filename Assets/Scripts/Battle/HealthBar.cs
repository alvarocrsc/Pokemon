using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBar;

    public Sprite healthySprite;
    public Sprite mediumSprite;
    public Sprite lowSprite;

    private Image barImage;

    private void Awake()
    {
        barImage = healthBar.GetComponent<Image>();
    }

    private Sprite BarSprite(float normalizedValue)
    {
        if (normalizedValue > 0.5f)
            return healthySprite;
        else if (normalizedValue > 0.15f)
            return mediumSprite;
        else
            return lowSprite;
    }

    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1f);
        barImage.sprite = BarSprite(normalizedValue);
    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        float currentScale = healthBar.transform.localScale.x;
        float updateQuantity = currentScale - normalizedValue;
        while (currentScale - normalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentScale, 1);
            barImage.sprite = BarSprite(currentScale);
            yield return null;
        }

        healthBar.transform.localScale = new Vector3(normalizedValue, 1);
        barImage.sprite = BarSprite(normalizedValue);
    }
}