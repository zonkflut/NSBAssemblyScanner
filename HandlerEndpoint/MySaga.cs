using System;
using MyMessages.Commands;
using MyMessages.Events;
using MyMessages.Timeouts;
using NServiceBus;
using NServiceBus.Saga;

namespace HandlerEndpoint
{
    public class MySaga : Saga<MySagaData>,
        IAmStartedByMessages<MyCommand1>, // TODO: Not yet handled: appears as IHandleMessages
        IHandleMessages<MyEvent1>,
        IHandleTimeouts<MyTimeout1>,
        IHandleTimeouts<MyTimeout2>,
        IHandleTimeouts<MyTimeout3>
    {
        public void Handle(MyCommand1 message)
        {
            // TODO: Not yet handled
            RequestTimeout<MyTimeout1>(DateTime.Now.AddMinutes(1), t => { });
        }

        public void Handle(MyEvent1 message)
        {
            // TODO: Not yet handled
            RequestTimeout(DateTime.Now.AddMinutes(1), new MyTimeout2());
        }

        public void Timeout(MyTimeout1 state)
        {
            // TODO: Not yet handled
            var myTimeout = new MyTimeout3();

            Console.Write("Creating Timeout");

            RequestTimeout(DateTime.Now.AddMinutes(1), myTimeout);
        }

        public void Timeout(MyTimeout2 state)
        {
        }

        public void Timeout(MyTimeout3 state)
        {
        }
    }

    public class MySagaData : IContainSagaData
    {
        public Guid Id { get; set; }

        public string Originator { get; set; }

        public string OriginalMessageId { get; set; }
    }
}