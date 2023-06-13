using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MathUtils
{
    public static float AdvanceTo(float value, float target, float rate, float dt)
    {
        float toTarget = (target - value);
        float change = rate * dt;
        if (Mathf.Abs(toTarget) < change)
        {
            return target;
        }
        else
        {
            return value + Mathf.Sign(toTarget) * change;
        }
    }
}
