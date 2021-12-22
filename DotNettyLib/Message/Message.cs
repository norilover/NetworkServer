using DotNettyLib.Message;
using ProtoBuf;

namespace DotNettyLib.Application
{
    [ProtoContract]
    public class Message
    {
        [ProtoMember(1)]
        public MessageCode MessageCode { get; }
    }
}