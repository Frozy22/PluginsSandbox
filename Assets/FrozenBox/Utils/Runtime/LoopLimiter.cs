using UnityEngine.Assertions;

namespace FrozenBox.Utils
{
    public struct LoopLimiter
    {
        public const int DEFAULT_LOOP_LIMIT = 9999;

        public int CurrentIndex;
        public readonly int Limit;

        public LoopLimiter(int limit = DEFAULT_LOOP_LIMIT)
        {
            CurrentIndex = 0;
            Limit = limit;
        }
        
        public static LoopLimiter New(int limit = DEFAULT_LOOP_LIMIT) => new(limit);
        
        public bool Next()
        {
            Assert.AreNotEqual(0, Limit, "Loop limiter has not been initialized");
            Assert.IsTrue(CurrentIndex < Limit, "Loop limit exceeded. Possible infinite loop");
            return CurrentIndex++ < Limit;
        }
    }
}