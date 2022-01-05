using ProtoBuf;

namespace DotNettyLib.Message.DataMessage
{
    [ProtoContract]
    public class LeaveMessage : Application.Message
    {
        [ProtoMember(1)] public string Id;
        public MessageCode MessageCode => MessageCode.Leave;
    }
}