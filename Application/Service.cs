using System;
using Polly;

namespace Application
{
    public class Service
    {
        private readonly Random _rnd = new Random((int)DateTime.UtcNow.Ticks);

        public int Add(int x, int y) => x + y;

        public int Return3()
        {
            return Policy
                .Handle<InvalidOperationException>()
                .Retry(3)
                .Execute(() =>
                {
                    var rndValue = _rnd.Next(0, 3);
                    if (rndValue == 0)
                        throw new InvalidOperationException();
                    return 3;
                });
        }
    }
}