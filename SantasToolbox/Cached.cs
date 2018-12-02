using System;

namespace SantasToolbox
{
    public class Cached<T> : IDisposable
    {
        private readonly Func<T> initializer;
        private Lazy<T> value;

        public Cached(Func<T> initializer)
        {
            this.initializer = initializer;
            this.Reset();
        }

        public void Reset()
        {
            this.value = new Lazy<T>(this.initializer);
        }

        public void Dispose()
        {
            if (this.value.IsValueCreated && this.value.Value is IDisposable disposable)
            {
                disposable.Dispose();
                this.Reset();
            }
        }

        public T Value => this.value.Value;

        public bool IsValueCreated => this.value.IsValueCreated;
    }
}
