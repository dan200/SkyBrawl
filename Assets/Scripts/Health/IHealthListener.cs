using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IHealthListener
{
    void OnHealthChanged(Health health);
    void OnDamageReceived(Health health, in Damage damage);
}
