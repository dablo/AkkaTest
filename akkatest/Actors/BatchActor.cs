using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;

namespace akkatest.Actors
{
    public class BatchActor<T> : UntypedActor
    {
        private readonly IActorRef _writer;
        private List<int> _running;
        private int _count;
        private Stopwatch _sw;
        private IActorRef _starter;

        public BatchActor(IActorRef writer)
        {
            _writer = writer;
            _running = new List<int>();
        }

        public static Props Props(IActorRef writer)
        {
            return Akka.Actor.Props.Create(() => new BatchActor<T>(writer));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case MessageBatch<T> m:
                    _count = m.Messages.Count;
                    _starter = Sender;
                    _sw = Stopwatch.StartNew();
                    foreach (var mess in m.Messages)
                    {
                        _running.Add(mess.Id);
                        mess.Receiver.Tell(mess.Instruction);
                    }
                    _writer.Tell("Batch sent! -> " + m.Messages.Count);
                    break;
                case MessageDone m:
                    _running.Remove(m.Id);

                    if (_running.Count == 0)
                    {
                        var ms = (float)_sw.ElapsedMilliseconds;
                        _writer.Tell($"Done with batch of {_count} messages in {ms} ms -> {_count / ms * 1000} msg/sec");
                        _starter.Tell(new MessageDone(-1));
                    }
                    break;
            }
        }
    }
}