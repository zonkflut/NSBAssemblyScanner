using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NSBEndpointAndMessageDetection;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace ScannerTests
{
    [TestFixture]
    public class NSBAssemblyScannerTests
    {
        [Test]
        public void MyTest()
        {
            var scanner = new NSBAssemblyScanner();
            var results = scanner.Scan(@"D:\Projects\ChildPool\src\ChildPool.Handlers\bin\Debug");
        }

        [Test]
        public void UsingCecil()
        {
            
        }
    }
}
// ReSharper restore InconsistentNaming
