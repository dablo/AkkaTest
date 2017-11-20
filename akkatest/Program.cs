using System.Threading.Tasks;
using akkatest.Actors;
using Akka.Actor;

namespace akkatest
{
    internal class Program
    {
        private static ActorSystem _sysactor;

        private static async Task Main()
        {
            _sysactor = ActorSystem.Create("theSystem");
            var vehicles = _sysactor.ActorOf<VehiclesActor>("vehicles");
            var writer = _sysactor.ActorOf<ConsoleWriterActor>("writer");
            var commander = _sysactor.ActorOf(CommandActor.Props(vehicles, writer), "commander");
             _sysactor.ActorOf(ConsoleReaderActor.Props(commander),"reader");
            commander.Tell("print");
            await _sysactor.WhenTerminated;

        }
    }
}