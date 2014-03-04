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
            var scanner = new NsbAssemblyScanner();
            results.AddRange(scanner.Scan(@"D:\Projects\NSBAssemblyScanner\SenderEndpoint\bin\Debug"));
            results.AddRange(scanner.Scan(@"D:\Projects\NSBAssemblyScanner\HandlerEndpoint\bin\Debug"));

            Console.WriteLine("\r\n\r\n\r\n\r\n\r\n\r\n{0}", string.Join("\r\n\r\n", results.Select(r => string.Format("Assembly Name:{0}\r\nName: {1}\r\nMessages:\r\n{2}", r.AssemblyName, r.Name, string.Join("\r\n", r.Messages.Select(m => string.Format("Name: {0} Operation: {1}", m.Name, m.Operation)))))));
        }
    }

    [TestFixture]
    public class NsbUsageIlRendererTests
    {
        [Test]
        public void TryRender()
        {
            var renderUsages = new NsbUsageIlRenderer()
                .RenderUsages(@"D:\Projects\NSBAssemblyScanner\SenderEndpoint\bin\Debug\SenderEndpoint.dll");

            Console.WriteLine(renderUsages);
        }
    }
}
// ReSharper restore InconsistentNaming
