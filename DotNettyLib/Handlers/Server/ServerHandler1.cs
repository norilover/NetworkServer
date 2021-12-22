using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNettyLib.Application;
using DotNettyLib.log;
using DotNettyLib.Message;
using DotNettyLib.Message.DetailMessage;
using Microsoft.Extensions.ObjectPool;
using ProtoBuf;

namespace DotNettyLib.Handlers
{
    public class ServerHandler1<T> : ChannelHandlerAdapter
    {
        private readonly ObjectPool<MemoryStream> _memoryStreamPool =
            new DefaultObjectPool<MemoryStream>(new DefaultPooledObjectPolicy<MemoryStream>());

        private IConsumerProduct<T> _receiveMessageApplication;

        public ServerHandler1(IConsumerProduct<T> receiveMessageApplication)
        {
            _receiveMessageApplication = receiveMessageApplication;
        }

        public override bool IsSharable => true;

        /// <summary>
        /// 初次建立连接时调用
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Log.Debug("Server ChannelActive");
            base.ChannelActive(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Log.Debug("ChannelInActive");
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

                memoryStream.SetLength(0);
                // memoryStream.WriteAsync(seg.Array, seg.Offset, seg.Count).Wait();
                memoryStream.WriteAsync(buffer.Array, buffer.ArrayOffset, buffer.ReadableBytes).Wait();
                memoryStream.Position = 0;

                var msg = Serializer.DeserializeWithLengthPrefix<Application.Message>(memoryStream,
                    PrefixStyle.Fixed32);
                ConsumeMessage(msg, memoryStream);
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

        private void ConsumeMessage(Application.Message msg, MemoryStream memoryStream)
        {
            memoryStream.Position = 0;
            
            switch (msg.MessageCode)
            {
                case MessageCode.Message:
                    Log.Debug("Server receive message");
                    break;
                case MessageCode.Join:
                    Log.Debug("Server receive join message");
                    JoinMessage joinMessage =
                        Serializer.DeserializeWithLengthPrefix<JoinMessage>(memoryStream, PrefixStyle.Fixed32);
                    if(joinMessage != null)
                        Log.Debug("ID: " + joinMessage.Id);
                    break;
                case MessageCode.Leave:
                    Log.Debug("Server receive leave message");
                    break;
                case MessageCode.Unknown:
                    Log.Debug("Server receive unknown message");
                    break;
            }
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