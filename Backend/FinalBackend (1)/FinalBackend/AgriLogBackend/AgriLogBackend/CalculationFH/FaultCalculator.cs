using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgriLogBackend.CalculationFH
{
    public class FaultCalculator : Calculator
    {
        public override int GetTotal(int farm_ID)
        {
            return 50;
        }

        public override int GetDenom(int farm_ID)
        {
            return 100;
        }

        public override int CalcScore(int farmID)
        {
            return 50;
        }

    }
}