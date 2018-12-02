using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SantasToolbox
{
    public class InputProvider<T> : IEnumerator<T>
    {
        public delegate bool StringToTConverter(string input, out T result);
        
        private readonly StringToTConverter converter;
        private readonly Cached<StreamReader> fileStream;
        
        public InputProvider(string filePath, StringToTConverter converter)
        {
            this.converter = converter;

            this.fileStream = new Cached<StreamReader>(() => new StreamReader(filePath));
        }

        public T Current { get; private set; }

        object IEnumerator.Current => this.Current;

        public void Dispose()
        {
            this.fileStream?.Dispose();
        }

        public bool MoveNext()
        {
            var line = this.fileStream.Value.ReadLine();

            if (line?.Length <= 0)
            {
                return false;
            }

            if (this.converter(line, out T result))
            {
                this.Current = result;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            this.fileStream.Reset();
        }
    }
}
