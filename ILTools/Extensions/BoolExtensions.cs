using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools.Extensions
{
    public static class BoolExtensions
    {
        public static bool Implies(this bool a, bool b)
        {
            return !a | b; 
        }
    }
}
