using DotNettyLib.Application;
using ProtoBuf;

namespace DotNettyLib.Message.DetailMessage
{
    [ProtoContract]
    public class CommonMessage : Application.Message
    {
        [ProtoMember(2)] public object ObjectMessage;
        public MessageCode MessageCode => MessageCode.Message;
    }
}