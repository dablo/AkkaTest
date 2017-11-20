using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;

namespace akkatest.Actors
{
    public class VehiclesActor : UntypedActor
    {
        private readonly Dictionary<string, IActorRef> _vehicleRefByVin = new Dictionary<string, IActorRef>();
        private readonly Dictionary<IActorRef, string> _vehicleVinByRef = new Dictionary<IActorRef, string>();

        public class UpdateCurrentStatus : Message
        {
            public string Vin { get; }
            public string Status { get; }

            public UpdateCurrentStatus(int id, string vin, string status) : base(id)
            {
                Vin = vin;
                Status = status;
            }
        }

        public class GetCurrentStatsus : Message
        {
            public string Vin { get; }

            public GetCurrentStatsus(int id, string vin) : base(id)
            {
                Vin = vin;
            }
        }

        public class UpdatePosition : Message
        {
            public string Vin { get; }
            public Position Position { get; }

            public UpdatePosition(int id, string vin, Position position) : base(id)
            {
                Vin = vin;
                Position = position;
            }
        }

        public class GetCurrentPosition : Message
        {
            public string Vin { get; }

            public GetCurrentPosition(int id, string vin) : base(id)
            {
                Vin = vin;
            }
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case UpdateCurrentStatus cs:
                    GetVehiceRef(cs.Vin).Forward(message);
                    break;
                case UpdatePosition pos:
                    GetVehiceRef(pos.Vin).Forward(message);
                    break;
                case GetCurrentStatsus vehicle:
                    GetVehiceRef(vehicle.Vin).Forward(message);
                    break;
                case GetCurrentPosition vehicle:
                    GetVehiceRef(vehicle.Vin).Forward(message);
                    break;
                case Terminated t:
                    if (_vehicleVinByRef.ContainsKey(t.ActorRef))
                    {
                        var vin = _vehicleVinByRef[t.ActorRef];
                        _vehicleRefByVin.Remove(vin);
                        _vehicleVinByRef.Remove(t.ActorRef);
                    }
                    break;
            }
        }

        private IActorRef GetVehiceRef(string vin)
        {
            if (_vehicleRefByVin.ContainsKey(vin))
            {
                return _vehicleRefByVin[vin];
            }
            var vehicle = Context.ActorOf(VehicleActor.Props(vin, "started"));
            Context.Watch(vehicle);
            _vehicleRefByVin.Add(vin, vehicle);
            _vehicleVinByRef.Add(vehicle, vin);
            return vehicle;
        }
    }
}