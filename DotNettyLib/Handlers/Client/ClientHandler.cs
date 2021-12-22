using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNettyLib.Application;
using DotNettyLib.log;
using DotNettyLib.Message.DetailMessage;
using Microsoft.Extensions.ObjectPool;
using ProtoBuf;

namespace DotNettyLib.Handlers.Client
{
    public class ClientHandler : ChannelHandlerAdapter
    {
        private MemoryStream _memoryStream = new MemoryStream();

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Log.Info("Client ChannelActive");

            // JoinMessage
            JoinMessage joinMessage = new JoinMessage();
            joinMessage.Id = 111111111 + "";

            // 将序列化数据填入_memoryStream
            _memoryStream.Position = 0;
            _memoryStream.SetLength(0);
            Serializer.SerializeWithLengthPrefix<JoinMessage>(_memoryStream, joinMessage, PrefixStyle.Fixed32);

            // 将_memoryStream 中的数据放入 IByteBuffer
            var buf = PooledByteBufferAllocator.Default.Buffer();
            buf.WriteBytes(_memoryStream.ToArray(), 0, (int)_memoryStream.Length);
            buf.Retain();

            // 这里只接收IByteBuffer的实现类
            context.Channel.WriteAndFlushAsync(buf).ContinueWith(SendSucc, buf).Wait();

            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Log.Info("ChannelInactive");
            
            // LeaveMessage
            LeaveMessage leaveMessage = new LeaveMessage();

            // 将序列化数据填入_memoryStream
            _memoryStream.Position = 0;
            _memoryStream.SetLength(0);
            Serializer.SerializeWithLengthPrefix<LeaveMessage>(_memoryStream, leaveMessage, PrefixStyle.Fixed32);

            // 将_memoryStream 中的数据放入 IByteBuffer
            var buf = PooledByteBufferAllocator.Default.Buffer();
            buf.WriteBytes(_memoryStream.ToArray(), 0, (int)_memoryStream.Length);
            buf.Retain();

            // 这里只接收IByteBuffer的实现类
            context.Channel.WriteAndFlushAsync(buf).ContinueWith(SendSucc, buf).Wait();

            base.ChannelInactive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Log.Info("Client ChannelRead");

            TestMessage(message, context);

            base.ChannelRead(context, message);
        }

        private void TestMessage(object message, IChannelHandlerContext channelHandlerContext)
        {
            string str = string.Empty;
            if (message is IByteBuffer buffer)
            {
                // 接收普通的字符串
                // str = buffer.ReadString(buffer.ReadableBytes, Encoding.Default);

                // 使用ArraySegment进行解析
                // var arraySegment = new ArraySegment<byte>(buffer.Array, buffer.ArrayOffset, buffer.ReadableBytes);
                // _memoryStream.SetLength(0);
                // _memoryStream.WriteAsync(arraySegment.Array, (int)arraySegment.Offset, (int)arraySegment.Count);

                // 直接解析
                _memoryStream.SetLength(0);
                _memoryStream.WriteAsync(buffer.Array, buffer.ArrayOffset, buffer.ReadableBytes);
                _memoryStream.Position = 0;

                // 反序列化成对象
                var deserialize = Serializer.DeserializeWithLengthPrefix<Application.Message>(_memoryStream, PrefixStyle.Fixed32);

                // 消费消息
                ConsumeMessage(deserialize);
            }

            Log.Debug("Client Read content: " + str);

            // ThreadPool.QueueUserWorkItem((s) =>
            // {
            //     var buf = PooledByteBufferAllocator.Default.Buffer();
            //     buf.WriteString(str + ",", Encoding.UTF8);
            //     buf.WriteBytes(buf, buf.ReadableBytes);
            //     buf.Retain();
            //
            //     channelHandlerContext.Channel.WriteAndFlushAsync(buf).ContinueWith(SendSucc, s).Wait();
            //
            //     Thread.Sleep(1000);
            // });
        }

        private void ConsumeMessage(Application.Message msg)
        {
            switch (msg)
            {
                case Message.DetailMessage.CommonMessage commonMessage:
                    Log.Debug("Client receive message");
                    break;
                case JoinMessage joinMessage:
                    Log.Debug("Client receive join message");
                    break;
                case LeaveMessage leaveMessage:
                    Log.Debug("Client receive leave message");
                    break;
            }
        }

        private void SendSucc(Task arg1, object arg2)
        {
            if (arg2 is IByteBuffer buf)
                buf.Release();

            if (!arg1.IsCompletedSuccessfully)
                Log.Warning("Client send info fail");
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            Log.Info("ChannelReadComplete");
            base.ChannelReadComplete(context);
        }

        public override bool IsSharable => false;
    }
}