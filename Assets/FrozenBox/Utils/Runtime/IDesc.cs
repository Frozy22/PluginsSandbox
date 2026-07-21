namespace FrozenBox.Utils
{
    public interface IDesc<out T>
    {
        public T Create();
    }
    
    public interface IDesc<out TOut, in TIn>
    {
        public TOut Create(TIn value);
    }
    
    public interface IDesc<out TOut, in TIn1, in TIn2>
    {
        public TOut Create(TIn1 value1, TIn2 value2);
    }
}