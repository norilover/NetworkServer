using DotNettyLib.Application;
using ProtoBuf;

namespace DotNettyLib.Message.DetailMessage
{
    [ProtoContract]
    public class JoinMessage : Application.Message
    {
        [ProtoMember(2)] public string Id;
        public MessageCode MessageCode => MessageCode.Join;
    }
}