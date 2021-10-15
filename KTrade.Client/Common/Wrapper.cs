namespace KTrade.Client.Common
{
    public class Wrapper<T>
    {
        public T Value { get; set; }
    }

    public class IntWrapper : Wrapper<int>
    {

    }
}