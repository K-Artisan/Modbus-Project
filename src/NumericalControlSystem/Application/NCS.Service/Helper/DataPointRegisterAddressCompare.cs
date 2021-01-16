using NCS.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NCS.Service.Helper
{
    public class DataPointRegisterAddressCompare : IComparer<DataPoint>
    {
        public int Compare(DataPoint x, DataPoint y)
        {
            return x.StartRegisterAddress.CompareTo(y.StartRegisterAddress);
        }
    }
}
