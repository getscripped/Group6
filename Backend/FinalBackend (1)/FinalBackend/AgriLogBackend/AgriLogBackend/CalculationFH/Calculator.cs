using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgriLogBackend.CalculationFH
{
    public class Calculator : CalcInterFace
    {
        public virtual int GetTotal(int farmID) {
            return 50;
        }

        public virtual int GetDenom(int farmID)
        {
            return 100;
        }

        public virtual int CalcScore(int farmID)
        {
            return 50;
        }

    }
}