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
        private const string HandlerEndpointAssemblyDirectory = @"C:\Projects\NSBAssemblyScanner\HandlerEndpoint\bin\Debug";
        private const string SenderEndpointAssemblyDirectory = @"C:\Projects\NSBAssemblyScanner\SenderEndpoint\bin\Debug";

        [Test]
        public void RenderAllResults()
        {
            var results = new List<IInstance>();
            var scanner = new NsbAssemblyScanner();
            results.AddRange(scanner.Scan(SenderEndpointAssemblyDirectory));
            results.AddRange(scanner.Scan(HandlerEndpointAssemblyDirectory));

            Console.WriteLine("\r\n\r\n\r\n\r\n\r\n\r\n{0}", string.Join("\r\n\r\n", results.Select(r => string.Format("Assembly Name:{0}\r\nName: {1}\r\nMessages:\r\n{2}", r.AssemblyName, r.Name, string.Join("\r\n", r.Messages.Select(m => string.Format("Name: {0} Operation: {1}", m.Name, m.Operation)))))));
        }

        [Test]
        public void MySender_ExecuteCommand()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MySender", "MyMessages.Commands.MyCommand1", "Send").Any());
        }
        
        [Test]
        public void MySender_ExecuteGenericCommand()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MySender", "MyMessages.Commands.MyCommand2", "Send").Any());
        }

        [Test]
        public void MySender_ExecuteEventWithOtherStatmentsBetween()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MySender", "MyMessages.Commands.MyCommand3", "Send").Any());
        }

        [Test]
        public void MySender_ExecuteWithParameter()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MySender", "MyMessages.Commands.MyCommand4", "Send").Any());
        }

        [Test]
        public void MySender_ExecuteCommandWithOtherNewedUpdStatementsBetween()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MySender", "MyMessages.Commands.MyCommand5", "Send").Any());
        }

        [Test]
        public void MySender_ExecuteCommandWithParameterFromMethod()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MySender", "MyMessages.Commands.MyCommand6", "Send").Any());
        }

        [Test]
        public void MySender_ExecuteCommandWithDestinationAddress()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MySender", "MyMessages.Commands.MyCommand7", "Send").Any());
        }

        [Test]
        public void MySender_ExecuteSendLocalCommand()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MySender", "MyMessages.Commands.MyCommand8", "SendLocal").Any());
        }

        [Test]
        public void MyPublisher_ExecuteEvent()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MyPublisher", "MyMessages.Events.MyEvent1", "Publish").Any());
        }

        [Test]
        public void MyPublisher_ExecuteGenericEvent()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MyPublisher", "MyMessages.Events.MyEvent2", "Publish").Any());
        }

        [Test]
        public void MyPublisher_ExecuteEventWithOtherStatmentsBetween()
        {
            var results = new NsbAssemblyScanner().Scan(SenderEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("SenderEndpoint", "SenderEndpoint.MyPublisher", "MyMessages.Events.MyEvent3", "Publish").Any());
        }

        [Test]
        public void MySaga_HandlesIAmStartedByMessages()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MySaga", "MyMessages.Commands.MyCommand1", "IAmStartedByMessages").Any());
        }

        [Test]
        public void MySaga_HandlesIHandleMessages()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MySaga", "MyMessages.Events.MyEvent1", "IHandleMessages").Any());
        }

        [Test]
        public void MySaga_HandlesIHandleTimeouts()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MySaga", "MyMessages.Timeouts.MyTimeout1", "IHandleTimeouts").Any());
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MySaga", "MyMessages.Timeouts.MyTimeout2", "IHandleTimeouts").Any());
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MySaga", "MyMessages.Timeouts.MyTimeout3", "IHandleTimeouts").Any());
        }

        [Test]
        public void MySaga_RequestGenericTimeout()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MySaga", "MyMessages.Timeouts.MyTimeout1", "RequestTimeout").Any());
        }

        [Test]
        public void MySaga_RequestTimeout()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MySaga", "MyMessages.Timeouts.MyTimeout2", "RequestTimeout").Any());
        }

        [Test]
        public void MySaga_RequestTimeoutWithCommandsBetween()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MySaga", "MyMessages.Timeouts.MyTimeout3", "RequestTimeout").Any());
        }

        [Test]
        public void MyHandler_HandlesRequests()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MyHandler", "MyMessages.Requests.MyRequest", "IHandleMessages").Any());
        }

        [Test]
        public void MyHandler_HandlesEvents()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MyHandler", "MyMessages.Events.MyEvent1", "IHandleMessages").Any());
        }

        [Test]
        public void MyHandler_HandlesCommands()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MyHandler", "MyMessages.Commands.MyCommand1", "IHandleMessages").Any());
        }

        [Test]
        public void MyHandler_GenericReply()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MyHandler", "MyMessages.Responses.MyResponse1", "Reply").Any());
        }

        [Test]
        public void MyHandler_Reply()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MyHandler", "MyMessages.Responses.MyResponse2", "Reply").Any());
        }

        [Test]
        public void MyHandler_ReplyWithCommandsBetween()
        {
            var results = new NsbAssemblyScanner().Scan(HandlerEndpointAssemblyDirectory);
            Assert.IsTrue(results.GetMessages("HandlerEndpoint", "HandlerEndpoint.MyHandler", "MyMessages.Responses.MyResponse3", "Reply").Any());
        }
    }
}
// ReSharper restore InconsistentNaming
