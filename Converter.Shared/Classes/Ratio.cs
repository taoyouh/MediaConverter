using System;
using System.Collections.Generic;
using System.Text;

namespace Converter.Classes
{
    public class Ratio
    {
        public Ratio(uint denominator, uint numerator)
        {
            Denominator = denominator;
            Numerator = numerator;
        }

        public uint Denominator { get; set; }

        public uint Numerator { get; set; }
    }
}
