using System;
using MyMessages.Events;
using NServiceBus;

namespace SenderEndpoint
{
    public class MyPublisher
    {
        public IBus Bus { get; set; }

        public void ExecuteEvent()
        {
            Bus.Publish(new MyEvent1());
        }

        public void ExecuteGenericEvent()
        {
            Bus.Publish<MyEvent2>(m => { });
        }

        public void ExecuteEventWithOtherStatmentsBetween()
        {
            var myEvent = new MyEvent3();

            Console.WriteLine("Doing something in between");

            Bus.Publish(myEvent);
        }
    }
}
