using ProtoBuf;

namespace DotNettyLib.Message.DataMessage
{
    [ProtoContract]
    public class CommonMessage : Application.Message
    {
        [ProtoMember(1)]
        public object Message;
        public MessageCode MessageCode => MessageCode.CommonMessage;
    }
}