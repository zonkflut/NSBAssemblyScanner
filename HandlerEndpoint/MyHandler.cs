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
        IHandleMessages<MyEvent1>,
        IHandleMessages<MyCommand1>
    {
        public IBus Bus { get; set; }

        public void Handle(MyRequest message)
        {
            Bus.Reply<MyResponse1>(r => { });
        }

        public void Handle(MyEvent1 message)
        {
            Bus.Reply(new MyResponse2());
        }

        public void Handle(MyCommand1 message)
        {
            var response = new MyResponse3();

            Console.WriteLine("Responding");

            Bus.Reply(response);
        }
    }
}
