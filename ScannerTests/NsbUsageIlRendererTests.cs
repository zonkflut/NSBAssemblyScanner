using System;
using NSBEndpointAndMessageDetection;
using NUnit.Framework;

namespace ScannerTests
{
    [TestFixture]
    public class NsbUsageIlRendererTests
    {
        [Test]
        public void TryRender()
        {
            var renderUsages = new NsbUsageIlRenderer()
                .RenderUsages(@"C:\Projects\NSBAssemblyScanner\SenderEndpoint\bin\Debug\SenderEndpoint.dll");

            Console.WriteLine(renderUsages);
        }
    }
}