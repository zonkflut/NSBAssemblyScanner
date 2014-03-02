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
            // TODO: Not yet handled
            Bus.Publish(new MyEvent1());
        }

        public void ExecuteGenericEvent()
        {
            // TODO: Not yet handled
            Bus.Publish<MyEvent2>(m => { });
        }

        public void ExecuteEventWithOtherStatmentsBetween()
        {
            // TODO: Not yet handled
            var myEvent = new MyEvent3();

            Console.WriteLine("Doing something in between");

            Bus.Publish(myEvent);
        }
    }
}
