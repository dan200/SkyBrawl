using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IProjectileListener
{
    void OnImpact(Collider hitObject, Vector3 hitPos, Vector3 hitNormal, Vector3 hitVelocity);
}
