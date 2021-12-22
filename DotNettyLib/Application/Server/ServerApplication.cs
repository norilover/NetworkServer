using System;
using System.Net;
using System.Net.Sockets;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNettyLib.Config;
using DotNettyLib.Handlers;
using DotNettyLib.log;

namespace DotNettyLib.Application
{
    public class ServerApplication : IApplication
    {
        private IChannel _channel;
        private IConsumerProduct<object> _receiveMessageApplication = new ReceiveApplication<object>();

        public void Start()
        {
            var serverBootstrap = new ServerBootstrap();
            
            serverBootstrap
                .Group(new SingleThreadEventLoop())
                .Channel<TcpServerSocketChannel>()
                // 等待队列中可以保留的连接数
                .Option(ChannelOption.SoBacklog, 1 << 9)
                .Option(ChannelOption.SoSndbuf, 1 << 18)
                .Option(ChannelOption.SoRcvbuf, 1 << 18)
                .Option(ChannelOption.SoKeepalive, true)
                .Option(ChannelOption.ConnectTimeout, new TimeSpan(0, 1, 0))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>((socketChannel) =>
                {
                    socketChannel.Pipeline
                        .AddLast("enc", new LengthFieldPrepender(4))
                        .AddLast("dec", new LengthFieldBasedFrameDecoder(1 << 22, 0, 4, 0, 4))
                        // .AddLast("idle handler", new IdleStateHandler(60, 0, 0))
                        .AddLast("server handler", new ServerHandler1<object>(_receiveMessageApplication));
                }));

            var channel = serverBootstrap.BindAsync(IPAddress.Any, ServerConfig.Port).Result;
            
            // 绑定地址失败
            if(!channel.Open)
                Log.Error("Can't open the channel when bind the ip and port");
            
            Log.Info("Bind ip:port " + channel.LocalAddress);
            
            // 记录channel
            _channel = channel;
        }

        public void End()
        {
            if(_channel != null)
                _channel.CloseAsync();
        }
    }
}