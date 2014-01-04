using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourWalledCubicle.VizMap
{
    public struct SymbolInfo
    {
        public string Name { get; set; }

        public long StartAddress { get; set; }

        public uint Size { get; set; }

        public long EndAddress
        {
            get
            {
                return StartAddress + Size;
            }
        }
    }
}
