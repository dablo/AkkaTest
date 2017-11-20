using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;

namespace akkatest.Actors
{
    class CommandActor : UntypedActor
    {
        private readonly IActorRef _vehicles;
        private readonly IActorRef _writer;
        private Cancelable _cancel;
        private Stopwatch _sw;

        public CommandActor(IActorRef vehicles, IActorRef writer)
        {
            _vehicles = vehicles;
            _writer = writer;
            _sw = Stopwatch.StartNew();
        }

        public static Props Props(IActorRef vehicles, IActorRef writer)
        {
            return Akka.Actor.Props.Create(() => new CommandActor(vehicles, writer));
        }

        protected override void OnReceive(object message)
        {
            if (message is string)
            {

                var line = message.ToString();
                var commands = line.Split(' ');
                switch (commands[0])
                {
                    case "print":
                        _writer.Tell("Welcome!");
                        _writer.Tell("Available commands are: start, exit, cs #, pos #");
                        break;

                    case "stat":
                        _vehicles.Ask<float>("stat").PipeTo(_writer);
                        break;

                    case "exit":
                        _writer.Tell("Shutting down...");
                        Context.System.Terminate();
                        return;

                    case "cs":
                        var count = int.Parse(commands[1]);
                        var batch = new List<Message<VehiclesActor.UpdateCurrentStatus>>();
                        for (int i = 0; i < count; i++)
                        {
                            batch.Add(new Message<VehiclesActor.UpdateCurrentStatus>(i,
                                new VehiclesActor.UpdateCurrentStatus(i, "vin-" + i, "Started"), _vehicles));
                        }
                        var batcher = Context.ActorOf(BatchActor<VehiclesActor.UpdateCurrentStatus>.Props(_writer));
                        Context.Watch(batcher);
                        batcher.Tell(new MessageBatch<VehiclesActor.UpdateCurrentStatus>(batch));
                        break;

                    case "pos":
                        var count2 = int.Parse(commands[1]);
                        var batch2 = new List<Message<VehiclesActor.UpdatePosition>>();
                        for (int i = 0; i < count2; i++)
                        {
                            batch2.Add(new Message<VehiclesActor.UpdatePosition>(i,new VehiclesActor.UpdatePosition(i, "vin-" + i, new Position(50, 51)), _vehicles));
                            batch2.Add(new Message<VehiclesActor.UpdatePosition>(i+count2,new VehiclesActor.UpdatePosition(i+count2, "vin-" + i, new Position(50, 51)), _vehicles));
                            batch2.Add(new Message<VehiclesActor.UpdatePosition>(i + count2 + count2, new VehiclesActor.UpdatePosition(i + count2 + count2, "vin-" + i, new Position(50, 51)), _vehicles));
                        };
                        var batcher2 = Context.ActorOf(BatchActor<VehiclesActor.UpdatePosition>.Props(_writer));
                        Context.Watch(batcher2);
                        batcher2.Tell(new MessageBatch<VehiclesActor.UpdatePosition>(batch2));
                        break;

                    case "timer":
                        var cmd = commands[1];
                        switch (cmd)
                        {
                            case "start":
                                var interval = int.Parse(commands[2]);
                                var batchSize = int.Parse(commands[3]);
                                _cancel = new Cancelable(Context.System.Scheduler, 30000);
                                _writer.Tell($"Starting timer for {batchSize} position updates every {interval}");
                                Context.System.Scheduler.ScheduleTellRepeatedly(interval, interval, Self,
                                    "pos " + batchSize, Self, _cancel);
                                break;

                            case "stop":
                                _writer.Tell($"Stopping timer");
                                _cancel.Cancel();
                                break;
                        }

                        break;

                    default:
                        _writer.Tell("Unhandled message!");
                        break;
                }
            }
            else
            {
                switch (message)
                {
                    case MessageDone msg when msg.Id < 0:
                        _writer.Tell("Stopping actor " + Sender);
                        Context.Stop(Sender);
                        break;

                    case Terminated t:
                        _writer.Tell($"Actor {t.ActorRef} stopped");

                        break;
                }
                return;
            }

            Context.ActorSelection("akka://theSystem/user/reader").Tell("read");

        }
    }
}