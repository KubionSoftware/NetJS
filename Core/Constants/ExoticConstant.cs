using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public abstract class ExoticConstant : Constant{

        public abstract Constant GetProperty(Constant key, Agent agent);
        public abstract void SetProperty(Constant key, Constant value, Agent agent);
    }
}
