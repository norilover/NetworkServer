using DotNettyLib.Message;
using ProtoBuf;

namespace DotNettyLib.Application
{
    [ProtoContract]
    public interface Message
    {
        MessageCode MessageCode { get; }
    }
}