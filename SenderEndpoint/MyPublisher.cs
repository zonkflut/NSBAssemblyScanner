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
            Bus.Publish(new MyEvent());
        }

        public void ExecuteGenericEvent()
        {
            Bus.Publish<MyEvent>(m => { });
        }

        public void ExecuteEventWithOtherStatmentsBetween()
        {
            var myEvent = new MyEvent();

            Console.WriteLine("Doing something in between");

            Bus.Publish(myEvent);
        }
    }
}
