using System;
using MyMessages.Commands;
using MyMessages.Events;
using MyMessages.Requests;
using MyMessages.Responses;
using NServiceBus;

namespace HandlerEndpoint
{
    public class MyHandler : 
        IHandleMessages<MyRequest>,
        IHandleMessages<MyEvent>,
        IHandleMessages<MyCommand>
    {
        public IBus Bus { get; set; }

        public void Handle(MyRequest message)
        {
            Bus.Reply<MyResponse>(r => { });
        }

        public void Handle(MyEvent message)
        {
            Bus.Reply(new MyResponse());
        }

        public void Handle(MyCommand message)
        {
            var response = new MyResponse();

            Console.WriteLine("Responding");

            Bus.Reply(response);
        }
    }
}
