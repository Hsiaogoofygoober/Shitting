using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    private string weaponName;
    private int amount;

    public Weapon(string weaponName, int amount)
    {
        this.weaponName = weaponName;
        this.amount = amount;
    }

    public int getAmount() 
    {
        return amount;
    }

    public string getWeaponName() 
    {
        return weaponName;
    }

    public void decreaceAmount() 
    {
        amount -= 1;
    }
}
