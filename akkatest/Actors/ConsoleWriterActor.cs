using System;
using Akka.Actor;

namespace akkatest.Actors
{
    public class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            Console.WriteLine(message);
        }
    }
}