using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Schema;

namespace AgriLogBackend.CalculationFH
{
    public interface CalcInterFace
    { 
         int GetTotal(int farmID);
         int GetDenom(int farmID);
         int CalcScore(int farmID);
    }
}