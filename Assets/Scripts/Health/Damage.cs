using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct Damage
{
    public DamageType Type;
    public float Amount;
    public DamagePerpetrator Perpetrator;

    public Damage(DamageType type, float amount, DamagePerpetrator perpetrator)
    {
        Debug.Assert(amount >= 0.0f);
        Type = type;
        Amount = amount;
        Perpetrator = perpetrator;
    }
}
