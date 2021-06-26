using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float minHealth;

    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // get current health
    public float CurrentHealth => currentHealth;

    // decrease current health 
    public void IncreaseHealth(float amount)
    {
        if ((currentHealth + amount) >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }
    }

    // decrease current health 
    public bool DecreaseHealth(float amount)
    {
        if((currentHealth - amount)<=minHealth)
        {
            currentHealth = minHealth;
            return true;
        }
        else
        {
            currentHealth -= amount;
            return false;
        }
    }
}
