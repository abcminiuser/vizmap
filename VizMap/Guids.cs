// Guids.cs
// MUST match guids.h
using System;

namespace FourWalledCubicle.VizMap
{
    static class GuidList
    {
        public const string guidVizMapPkgString = "6430028e-720f-4380-806e-4ec431f28ef7";
        public const string guidVizMapCmdSetString = "0b03a848-8010-48ff-bfe3-fbc427df1e2a";
        public const string guidToolWindowPersistanceString = "875170c2-24b5-4280-af37-fe4a27f27f35";

        public static readonly Guid guidVizMapCmdSet = new Guid(guidVizMapCmdSetString);
    };
}