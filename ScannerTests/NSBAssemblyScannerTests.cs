using System;
using System.Collections.Generic;
using System.Linq;
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
            var results = new List<IInstance>();
            var scanner = new NSBAssemblyScanner();
            results.AddRange(scanner.Scan(@"C:\Projects\NSBAssemblyScanner\SenderEndpoint\bin\Debug"));
            results.AddRange(scanner.Scan(@"C:\Projects\NSBAssemblyScanner\HandlerEndpoint\bin\Debug"));

            Console.WriteLine("\r\n\r\n\r\n\r\n\r\n\r\n{0}", string.Join("\r\n\r\n", results.Select(r => string.Format("Assembly Name:{0}\r\nName: {1}\r\nMessages:\r\n{2}", r.AssemblyName, r.Name, string.Join("\r\n", r.Messages.Select(m => string.Format("Name: {0} Operation: {1}", m.Name, m.Operation)))))));
        }
    }
}
// ReSharper restore InconsistentNaming
