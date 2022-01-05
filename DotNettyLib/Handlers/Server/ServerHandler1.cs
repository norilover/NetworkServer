using System;
using System.IO;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNettyLib.Application;
using DotNettyLib.log;
using DotNettyLib.Message;
using DotNettyLib.Message.DataMessage;
using DotNettyLib.Message.NetMessage;
using DotNettyLib.Util;
using Microsoft.Extensions.ObjectPool;
using ProtoBuf;

namespace DotNettyLib.Handlers
{
    public class ServerHandler1 : ChannelHandlerAdapter
    {
        private readonly ObjectPool<MemoryStream> _memoryStreamPool =
            new DefaultObjectPool<MemoryStream>(new DefaultPooledObjectPolicy<MemoryStream>());

        private IConsumerProduct<Application.Message> _receiveMessageApplication;
        
        private readonly bool _isSupportConcurrent = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSupportConcurrent">是否支持并发</param>
        /// <param name="receiveMessageApplication">当支持并发时，确保这个集合可以处理多线程并发访问的情况</param>
        public ServerHandler1(bool isSupportConcurrent, IConsumerProduct<Application.Message> receiveMessageApplication)
        {
            _receiveMessageApplication = receiveMessageApplication;
            _isSupportConcurrent = isSupportConcurrent;
        }

        /// <summary>
        /// 允许创建多个handle并发访问
        /// </summary>
        public override bool IsSharable => _isSupportConcurrent;

        /// <summary>
        /// 初次建立连接时调用
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Log.Debug("Server ChannelActive");

            // Connected network message
            _receiveMessageApplication.TryAdd(new NetMessage(context.Channel.Id, MessageCode.Connect));

            base.ChannelActive(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Log.Debug("ChannelInActive");

            // Disconnected network message
            _receiveMessageApplication.TryAdd(new NetMessage(context.Channel.Id, MessageCode.Disconnect));

            base.ChannelInactive(context);
        }

        /// <summary>
        /// 发生在<see cref="ChannelReadComplete"/>被调用之前
        /// 调用顺序 ChannelRead() -> ChannelReadComplete() -> Read()
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Log.Debug("Server ChannelRead");

            TestMessage(message, context);

            base.ChannelRead(context, message);
        }

        private void TestMessage(object message, IChannelHandlerContext channelHandlerContext)
        {
            string str = string.Empty;
            if (message is IByteBuffer buffer)
            {
                // var seg = new ArraySegment<byte>(buffer.Array, buffer.ArrayOffset, buffer.ReadableBytes);

                MemoryStream memoryStream = _memoryStreamPool.Get();

                // Sign the start length
                memoryStream.SetLength(0);
                
                // memoryStream.WriteAsync(seg.Array, seg.Offset, seg.Count).Wait();
                memoryStream.WriteAsync(buffer.Array, buffer.ArrayOffset, buffer.ReadableBytes).Wait();
                memoryStream.Position = 0;

                ConsumeMessage(ref memoryStream);
                
                _memoryStreamPool.Return(memoryStream);
            }

            Log.Debug("Server Read content: " + str);

            // ThreadPool.QueueUserWorkItem((s) =>
            // {
            //     Thread.Sleep(1000000);
            //
            //     var memoryStream = _memoryStreamPool.Get();
            //     memoryStream.SetLength(0);
            //
            //     var message1 = new Message.DetailMessage.Message();
            //     message1.CommonMessage = str + ", ";
            //
            //
            //     // Header
            //     // Serializer.SerializeWithLengthPrefix(memoryStream, message1, PrefixStyle.Fixed32);
            //     
            //     // Body
            //     Serializer.SerializeWithLengthPrefix(memoryStream, message1, PrefixStyle.Fixed32);
            //     
            //     var buf = PooledByteBufferAllocator.Default.Buffer();
            //     // buf.WriteString(str + ",", Encoding.UTF8);
            //     
            //     buf.WriteBytesAsync(memoryStream, (int)memoryStream.Length, CancellationToken.None).Wait();
            //     buf.Retain();
            //
            //     channelHandlerContext.Channel.WriteAndFlushAsync(buf).ContinueWith(SendSucc, s).Wait();
            // });
        }

        private void ConsumeMessage(ref MemoryStream memoryStream)
        {
            // Get the type of message 
            var msg = NetUtil.GetDeserialize<Application.Message>(ref memoryStream);
            
            // Set the position to zero for deserializing the data
            memoryStream.Position = 0;

            Application.Message msg0 = null;
            
            // According to the type to dispatch
            switch (msg.MessageCode)
            {
                case MessageCode.CommonMessage:
                    Log.Debug("Server receive message");
                    var commonMessage = NetUtil.GetDeserialize<CommonMessage>(ref memoryStream);
                    
                    msg0 = commonMessage;
                    break;
                case MessageCode.Join:
                    Log.Debug("Server receive join message");
                    var joinMessage = NetUtil.GetDeserialize<JoinMessage>(ref memoryStream);
                    if (joinMessage != null)
                        Log.Debug("ID: " + joinMessage.Id);
                    
                    msg0 = joinMessage;
                    break;
                case MessageCode.Leave:
                    Log.Debug("Server receive leave message");

                    msg0 = NetUtil.GetDeserialize<LeaveMessage>(ref memoryStream);
                    break;
                case MessageCode.Unknown:
                    Log.Debug("Server receive unknown message");
                    break;
            }
            
            if(msg0 != null)
                _receiveMessageApplication.TryAdd(msg0);
        }

        private void SendSucc(Task arg1, object arg2)
        {
            if (!arg1.IsCompletedSuccessfully)
                Log.Warning("Server send info fail");
        }

        /// <summary>
        /// 发生在<see cref="Read"/>被调用之前,<see cref="ChannelRead"/>之后
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            Log.Debug("ChannelReadComplete");

            // TODO
            context.Flush();

            base.ChannelReadComplete(context);
        }

        /// <summary>
        /// 当收到某个连接发来的消息时调用
        /// 当<see cref="ChannelReadComplete"/>被调用后执行
        /// </summary>
        /// <param name="context"></param>
        public override void Read(IChannelHandlerContext context)
        {
            Log.Debug("Read");
            // Log.Debug("Receive content: ");
            base.Read(context);
        }

        /// <summary>
        /// 当添加<see cref="IdleStateHandler"/>时会调用该函数
        /// </summary>
        /// <param name="context">触发条件的连接</param>
        /// <param name="evt">事件类型</param>
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            Log.Debug("UserEventTriggered");

            if (evt is IdleStateEvent)
            {
                // 如果是 IdleStateEvent 类型
                var e = (IdleStateEvent)evt;

                // 判断
                switch (e.State)
                {
                    case IdleState.ReaderIdle:
                        Log.Debug("IdleState.ReaderIdle");
                        break;
                    case IdleState.WriterIdle:
                        Log.Debug("IdleState.WriteIdle");
                        break;
                    case IdleState.AllIdle:
                        Log.Debug("IdleState.AllIdle");
                        break;
                }
            }

            base.UserEventTriggered(context, evt);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Log.Debug("ExceptionCaught");

            Log.Error(exception.Message);
            base.ExceptionCaught(context, exception);
        }
    }
}