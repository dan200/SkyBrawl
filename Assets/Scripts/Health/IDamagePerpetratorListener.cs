using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDamagePerpetratorListener
{
    void OnDamageDealt(Health victim, in Damage damage);
}
