using ProtoBuf;

namespace DotNettyLib.Message.DataMessage
{
    [ProtoContract]
    public class JoinMessage : Application.Message
    {
        [ProtoMember(1)] public string Id;
        public MessageCode MessageCode => MessageCode.Join;
    }
}