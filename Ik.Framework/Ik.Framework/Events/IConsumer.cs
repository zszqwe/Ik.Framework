
namespace Ik.Framework.Events
{
    public interface IConsumer<T>
    {
        void HandleEvent(string lable, T eventMessage);
    }
}
