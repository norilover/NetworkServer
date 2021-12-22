using DotNettyLib.Application;
using ProtoBuf;

namespace DotNettyLib.Message.DetailMessage
{
    [ProtoContract]
    public class LeaveMessage : Application.Message
    {
        [ProtoMember(2)] public string Id;
        public MessageCode MessageCode => MessageCode.Leave;
    }
}