using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int pHealth = 100;
    public Animator anim;
    public bool dead;
    public BoxCollider BC;

    public bool Damage(int amt)
    {
        pHealth -= amt;
        CheckHealth();        
        return pHealth <= 0;
    }

    public void CheckHealth()
    {
        if (pHealth <= 0 && !dead)
        {
            anim.SetTrigger("Death");
            BC.enabled = false;
        }
    }

    public int GetHealth()
    {
        return this.pHealth;
    }
}
