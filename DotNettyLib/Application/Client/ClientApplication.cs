﻿using System;
using System.Net;
using System.Threading;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNettyLib.Config;
using DotNettyLib.Handlers;
using DotNettyLib.Handlers.Client;
using DotNettyLib.log;

namespace DotNettyLib.Application
{
    public class ClientApplication : IApplication
    {
        private IChannel _channel;
        private Bootstrap _clientBootstrap;

        public void Start()
        {
            if (_channel != null)
            {
                _channel.CloseAsync().Wait();
            }

            if (_clientBootstrap == null)
            {
                _clientBootstrap = new Bootstrap();

                _clientBootstrap
                    .Group(new SingleThreadEventLoop())
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Option(ChannelOption.SoReuseaddr, true)
                    .Option(ChannelOption.SoReuseport, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>((socketChannel) =>
                    {
                        socketChannel.Pipeline
                            .AddLast("enc", new LengthFieldPrepender(4))
                            .AddLast("dec", new LengthFieldBasedFrameDecoder(1 << 22, 0, 4, 0, 4))
                            .AddLast("server handler", new ClientHandler());
                    }));
            }

            var channelTask = _clientBootstrap.ConnectAsync(ServerConfig.Ip, ServerConfig.Port);

            // 记录channel
            _channel = channelTask.Result;

            // SocketAddress socketAddress = _channel.LocalAddress.Serialize();

            // 连接失败
            // if (!channelTask.Wait(1))
            // {
            //     Log.Error("Can't connect the server based on ip: " + ServerConfig.Ip + " port: " + ServerConfig.Port);
            // }

            Thread.Sleep(1000);
            Start();


            Log.Info("Connect ip:port " + _channel.LocalAddress);
        }

        public void End()
        {
            if (_channel != null)
                _channel.CloseAsync();
        }
    }
}