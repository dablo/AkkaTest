using System;
using Akka.Actor;

namespace akkatest.Actors
{
    public  class ConsoleReaderActor:UntypedActor
    {
        private readonly IActorRef _commander;

        public ConsoleReaderActor(IActorRef commander)
        {
            _commander = commander;
        }

        public static Props Props(IActorRef commander) { return Akka.Actor.Props.Create(() => new ConsoleReaderActor(commander)); }

        protected override void OnReceive(object message)
        {

            Console.WriteLine("Enter command:");

            var cmd = Console.ReadLine();

            //see if the user typed "exit"
            if (!string.IsNullOrEmpty(cmd) &&
                cmd.ToLowerInvariant().Equals("exit"))
            {
                Console.WriteLine("Exiting!");
                // shut down the entire actor system via the ActorContext
                // causes MyActorSystem.AwaitTermination(); to stop blocking the current thread
                // and allows the application to exit.
                Context.System.Terminate();
                return;
            }
            _commander.Tell(cmd);
        }
    }
}