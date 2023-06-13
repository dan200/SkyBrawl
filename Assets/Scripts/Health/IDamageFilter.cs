using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDamageFilter
{
    bool FilterDamage(ref Damage io_damage);
}
