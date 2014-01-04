using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace FourWalledCubicle.VizMap
{
    public class SymbolParser
    {
        private ObservableCollection<SymbolInfo> _symbolSizes = new ObservableCollection<SymbolInfo>();

        private static readonly Regex _symbolParserRegex = new Regex(
                @"^" + //                   Start of line
                @"(?<Address>[^\s]*)" + //  Match/capture symbol address
                @"\s*" + //                 Whitespace seperator
                @"(?<Size>[^\s]*)" + //     Match/capture symbol size
                @"\s*" + //                 Whitespace seperator
                @"(?<Storage>[^\s])" + //   Match/capture symbol storage
                @"\s*" + //                 Whitespace seperator
                @"(?<Name>[^\t]*)" + //     Match/capture symbol name
                @"\t" + //                  Tab seperator
                @"(?<Location>.*)" + //     Match/capture symbol location
                @"$", //                    End of line
                RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public ObservableCollection<SymbolInfo> SymbolSizes
        {
            get { return _symbolSizes; }
        }

        public void ClearSymbols()
        {
            _symbolSizes.Clear();
        }

        public void ReloadSymbols(string elfPath, string toolchainNMPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            Process p = new Process();

            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.Arguments = String.Format(@"--print-size --demangle --line-numbers --radix=d ""{0}""", elfPath);
            startInfo.FileName = toolchainNMPath;

            List<String> symbolOutput = new List<String>();

            p.StartInfo = startInfo;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += (sender, args) => symbolOutput.Add(args.Data);
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();

            _symbolSizes.Clear();

            foreach (string s in symbolOutput)
            {
                if (s == null)
                    continue;

                Match itemData = _symbolParserRegex.Match(s);

                if (!itemData.Groups["Name"].Success || !itemData.Groups["Size"].Success || !itemData.Groups["Address"].Success)
                    continue;

                if (!"tTwW".Contains((char)itemData.Groups["Storage"].Value[0]))
                    continue;

                _symbolSizes.Add(new SymbolInfo()
                {                    
                    Size = UInt32.Parse(itemData.Groups["Size"].Value),
                    StartAddress = UInt32.Parse(itemData.Groups["Address"].Value),
                    Name = itemData.Groups["Name"].Value,
                });
            }
        }
    }
}
