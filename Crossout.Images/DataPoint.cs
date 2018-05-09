using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineChart
{
    public class DataPoint
    {
        public DataPoint()
        {

        }

        public DataPoint(double xValue, double yValue)
        {
            XValue = xValue;
            YValue = yValue;
        }
        

        //public DataPoint(double xValue, double[] yValues);

        public double XValue { get; set; }

        public double YValue { get; set; }
        
        public bool IsEmpty { get; set; }
        
        //public double GetValueByName(string valueName);
        
        //public void SetValueXY(object xValue, params object[] yValue);
        
        //public void SetValueY(params object[] yValue);
    }
}
