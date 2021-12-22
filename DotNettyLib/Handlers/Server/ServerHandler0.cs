using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using DotNettyLib.log;

namespace DotNettyLib.Handlers
{
    public class ServerHandler : IChannelHandler
    {
        public void ChannelRegistered(IChannelHandlerContext context)
        {
            Log.Debug("ChannelRegistered");
        }

        public void ChannelUnregistered(IChannelHandlerContext context)
        {
            Log.Debug("ChannelUnregistered");
        }

        public void ChannelActive(IChannelHandlerContext context)
        {
            Log.Debug("ChannelActive");
        }

        public void ChannelInactive(IChannelHandlerContext context)
        {
            Log.Debug("ChannelInactive");
        }

        public void ChannelRead(IChannelHandlerContext context, object message)
        {
            Log.Debug("ChannelRead");
        }

        public void ChannelReadComplete(IChannelHandlerContext context)
        {
            Log.Debug("ChannelReadComplete");
        }

        public void ChannelWritabilityChanged(IChannelHandlerContext context)
        {
            Log.Debug("ChannelWritabilityChanged");
        }

        public void HandlerAdded(IChannelHandlerContext context)
        {
            Log.Debug("HandlerAdded");
        }

        public void HandlerRemoved(IChannelHandlerContext context)
        {
            Log.Debug("HandlerRemoved");
        }

        public Task WriteAsync(IChannelHandlerContext context, object message)
        {
            Log.Debug("WriteAsync");

            return context.WriteAsync(message);
        }

        public void Flush(IChannelHandlerContext context)
        {
            Log.Debug("Flush");
        }

        public Task BindAsync(IChannelHandlerContext context, EndPoint localAddress)
        {
            Log.Debug("BindAsync");

            return context.BindAsync(localAddress);
        }

        public Task ConnectAsync(IChannelHandlerContext context, EndPoint remoteAddress, EndPoint localAddress)
        {
            Log.Debug("ConnectAsync");

            return context.ConnectAsync(localAddress);
        }

        public Task DisconnectAsync(IChannelHandlerContext context)
        {
            Log.Debug("DisconnectAsync");

            return context.DisconnectAsync();
        }

        public Task CloseAsync(IChannelHandlerContext context)
        {
            Log.Debug("CloseAsync");

            return context.CloseAsync();
        }

        public void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Log.Debug("ExceptionCaught");
        }

        public Task DeregisterAsync(IChannelHandlerContext context)
        {
            Log.Debug("DeregisterAsync");

            return context.DeregisterAsync();
        }

        public void Read(IChannelHandlerContext context)
        {
            Log.Debug("Read");
        }

        public void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            Log.Debug("UserEventTriggered");
        }
    }
}