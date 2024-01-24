using Homework.OldCustomerService.Services.Interfaces;

namespace Homework.OldCustomerService.Services
{
    public class RequestCounter : IRequestCounter
    {
        private int _currentCount = 0;
        private readonly int _maxCount = 5;

        public void Increment()
        {
            if (Interlocked.Increment(ref _currentCount) > _maxCount)
            {
                throw new InvalidOperationException("Překročen limit současně zpracovávaných požadavků.");
            }
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _currentCount);
        }
    }
}
