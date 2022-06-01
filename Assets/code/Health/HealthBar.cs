using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public bool isMyHealth;
    public Gradient myGradient;
    public Gradient gradient;
    public Image fill;
    private Transform myGameTransform;

    public void setMyGamTransform(Transform g)
    {
        myGameTransform = g;
    }
    public void setIsMyHealth(bool b)
    {
        isMyHealth = b;
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        if (isMyHealth)
            fill.color = myGradient.Evaluate(1f);
        else
            fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        slider.value = health;

        if (isMyHealth)
            fill.color = myGradient.Evaluate(slider.normalizedValue);
        else
            fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    public void DestroyHealthBar()
    {
        DestroyImmediate(transform.parent.gameObject);
    }

    private void Update()
    {
        // transform.localPosition = new Vector3(-960, -540, 0);

        if (myGameTransform != null)
        {
            Vector3 monsterPosition = new Vector3(myGameTransform.position.x, myGameTransform.position.y, myGameTransform.position.z); // we need to correct the position of the bar
            slider.transform.position = Camera.main.WorldToScreenPoint(monsterPosition + new Vector3(0, 0.9f, 0)); // we say that the position of the bar is a conversion of the position of the monster in my UI units.
        }


    }
}