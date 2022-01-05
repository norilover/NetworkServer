using System.IO;
using DotNettyLib.Message.DataMessage;
using ProtoBuf;

namespace DotNettyLib.Util
{
    public class NetUtil
    {
        public static T GetDeserialize<T>(ref MemoryStream ms) => Serializer.DeserializeWithLengthPrefix<T>(ms, PrefixStyle.Fixed32);
    }
}