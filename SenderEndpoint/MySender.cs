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
            Bus.Send(new MyCommand1());
        }

        public void ExecuteGenericCommand()
        {
            // TODO: Not yet handled
            Bus.Send<MyCommand2>(c => { });
        }

        public void ExecuteEventWithOtherStatmentsBetween()
        {
            var myEvent = new MyCommand3();

            Console.WriteLine("Doing something in between");

            Bus.Send(myEvent);
        }

        public void ExecuteWithParameter(MyCommand4 command)
        {
            // TODO: Not yet handled
            Bus.Send(command);
        }
    }
}