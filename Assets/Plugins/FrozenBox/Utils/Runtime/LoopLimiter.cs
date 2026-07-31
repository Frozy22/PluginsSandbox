using UnityEngine.Assertions;

namespace FrozenBox.Utils
{
    public struct LoopLimiter
    {
        public const int DEFAULT_LOOP_LIMIT = 9999;

        private int _currentIndex;
        private readonly int _limit;

        public LoopLimiter(int limit = DEFAULT_LOOP_LIMIT)
        {
            _currentIndex = 0;
            this._limit = limit;
        }
        
        public static LoopLimiter New(int limit = DEFAULT_LOOP_LIMIT) => new(limit);
        
        public bool Next()
        {
            Assert.AreNotEqual(0, _limit, "Loop limiter has not been initialized");
            Assert.IsTrue(_currentIndex < _limit, "Loop limit exceeded. Possible infinite loop");
            return _currentIndex++ < _limit;
        }
    }
}