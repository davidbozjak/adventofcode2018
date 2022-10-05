using System.Collections.Generic;
using System.Linq;

namespace SantasToolbox
{
    public class EnumerableReader<T>
    {
        private readonly T[] dataSource;
        private IEnumerable<T> workingStream;

        public EnumerableReader(T[] dataSource)
        {
            this.dataSource = dataSource;
            this.Reset();
        }

        public T TakeFirst()
        {
            var first = workingStream.FirstOrDefault();
            workingStream = workingStream.Skip(1);
            return first;
        }

        public IEnumerable<T> TakeN(int n)
        {
            var range = workingStream.Take(n);
            workingStream = workingStream.Skip(n);
            return range;
        }

        public void Reset()
        {
            this.workingStream = dataSource;
        }
    }
}
