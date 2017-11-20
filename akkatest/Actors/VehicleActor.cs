using System.Threading.Tasks;
using Akka.Actor;

namespace akkatest.Actors
{
    public class VehicleActor : UntypedActor
    {
        public string Vin { get; }
        public string Status { get; set; }
        public Position Position { get; set; }

        public VehicleActor(string vin, string status)
        {
            Vin = vin;
            Status = status;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case VehiclesActor.UpdateCurrentStatus cs:
                    Status = cs.Status;
                    Sender.Tell(new MessageDone(cs.Id));
                    //DelayedAnswer(new MessageDone(cs.Id), 3).PipeTo(Sender);
                    break;
                case VehiclesActor.UpdatePosition pos:
                    Position = pos.Position;
                    Sender.Tell(new MessageDone(pos.Id));             
                    //DelayedAnswer(new MessageDone(pos.Id), 3).PipeTo(Sender);
                    break;
                case VehiclesActor.GetCurrentStatsus _:
                    Sender.Tell(Status);
                    break;
                case VehiclesActor.GetCurrentPosition _:
                    Sender.Tell(Position);
                    break;
            }
        }

        private async Task<MessageDone> DelayedAnswer(MessageDone done, int afterMs)
        {
            await Task.Delay(afterMs);
            return done;
        }

        public static Props Props(string vin, string status)
        {
            return Akka.Actor.Props.Create(() => new VehicleActor(vin, status));
        }
    }
}