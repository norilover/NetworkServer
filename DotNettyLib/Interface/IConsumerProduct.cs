namespace DotNettyLib.Application
{
    public interface IConsumerProduct<T>
    {
        bool TryTake(out T msg); 
        
        bool TryAdd(T msg);

        bool IsEmpty();

        int Size();
    }
}