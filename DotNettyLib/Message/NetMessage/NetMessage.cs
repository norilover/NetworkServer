using DotNetty.Transport.Channels;
using ProtoBuf;
using IChannel = System.ServiceModel.Channels.IChannel;

namespace DotNettyLib.Message.NetMessage
{
    public class NetMessage : Application.Message
    {
        public readonly IChannelId Id;
        
        public MessageCode MessageCode { get; }
        
        public NetMessage(IChannelId channelId, MessageCode messageCode)
        {
            Id = channelId;
            MessageCode = messageCode;
        }

    }
}