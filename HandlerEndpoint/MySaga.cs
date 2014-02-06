using System;
using MyMessages.Commands;
using MyMessages.Events;
using MyMessages.Timeouts;
using NServiceBus;
using NServiceBus.Saga;

namespace HandlerEndpoint
{
    public class MySaga : Saga<MySagaData>,
        IAmStartedByMessages<MyCommand>,
        IHandleMessages<MyEvent>,
        IHandleTimeouts<MyTimeout>
    {
        public void Handle(MyCommand message)
        {
            RequestTimeout<MyTimeout>(DateTime.Now.AddMinutes(1), t => { });
        }

        public void Handle(MyEvent message)
        {
            RequestTimeout(DateTime.Now.AddMinutes(1), new MyTimeout());
        }

        public void Timeout(MyTimeout state)
        {
            var myTimeout = new MyTimeout();

            Console.Write("Creating Timeout");

            RequestTimeout(DateTime.Now.AddMinutes(1), myTimeout);
        }
    }

    public class MySagaData : IContainSagaData
    {
        public Guid Id { get; set; }

        public string Originator { get; set; }

        public string OriginalMessageId { get; set; }
    }
}