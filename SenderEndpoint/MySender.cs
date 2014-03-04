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
            Bus.Send(command);
        }

        public DateTime ExecuteCommandWithOtherNewedUpdStatementsBetween()
        {
            var myEvent = new MyCommand5();

            var otherObject = new DateTime();
            otherObject = otherObject.AddDays(10);

            Bus.Send(myEvent);
            
            return otherObject;
        }

        public void ExecuteCommandWithParameterFromMethod()
        {
            var command = GetCommand();
            Bus.Send(command);
        }

        private MyCommand6 GetCommand()
        {
            return new MyCommand6();
        }
    }
}