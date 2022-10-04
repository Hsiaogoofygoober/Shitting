using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo
{
    private string AmmoName;
    private int amount;

    public Ammo(string AmmoName, int amount)
    {
        this.AmmoName = AmmoName;
        this.amount = amount;
    }

    public int getAmount()
    {
        return amount;
    }

    public string getAmmoName()
    {
        return AmmoName;
    }

    public void decreaceAmount()
    {
        amount -= 1;
    }
}
