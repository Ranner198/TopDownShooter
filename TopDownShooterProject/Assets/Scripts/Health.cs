using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int pHealth = 100;

    public bool Damage(int amt)
    {
        pHealth -= amt;
        return pHealth <= 0;
    }

    public int GetHealth()
    {
        return this.pHealth;
    }
}
