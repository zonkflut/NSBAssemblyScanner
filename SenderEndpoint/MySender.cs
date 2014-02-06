using System;
using MyMessages.Commands;
using NServiceBus;

namespace SenderEndpoint
{
    public class MySender
    {
        public IBus Bus { get; set; }

        public void ExecuteCommand()
        {
            Bus.Send(new MyCommand());
        }

        public void ExecuteGenericCommand()
        {
            Bus.Send<MyCommand>(c => { });
        }

        public void ExecuteEventWithOtherStatmentsBetween()
        {
            var myEvent = new MyCommand();

            Console.WriteLine("Doing something in between");

            Bus.Send(myEvent);
        }
    }
}