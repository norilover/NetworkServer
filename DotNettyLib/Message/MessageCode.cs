namespace DotNettyLib.Message
{
    public enum MessageCode : byte
    {
        // Network Message
        Connect,
        Disconnect,

        // Data Message 
        Unknown,
        Join,
        Leave,
        CommonMessage,
    }
}