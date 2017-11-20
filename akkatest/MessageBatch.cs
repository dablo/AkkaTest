using System.Collections.Generic;
using Akka.Actor;

namespace akkatest
{
    public class MessageBatch<T>
    {
        public List<Message<T>> Messages { get; }

        public MessageBatch(List<Message<T>> messages)
        {
            Messages = messages;
        }
    }

    public class Message<T>
    {
        public int Id { get; }
        public IActorRef Receiver { get;  }
        public T Instruction { get;  }

        public Message(int id, T instruction,IActorRef receiver)
        {
            Id = id;
            Instruction = instruction;
            Receiver = receiver;
        }
    }

    internal class MessageDone
    {
        public MessageDone(int id)
        {
            Id = id;
        }

        public int Id { get;  }
    }

    public class Message
    {
        public Message(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }

    public class Position

    {
        public Position(decimal longitude, decimal latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public decimal Longitude { get;  }
        public decimal Latitude { get; }
    }
}