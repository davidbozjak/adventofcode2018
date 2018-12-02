using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SantasToolbox
{
    public class InputProvider<T> : IEnumerator<T>
    {
        public delegate bool StringToTConverter(string input, out T result);

        private readonly string filePath;
        private StreamReader fileStream;
        private StringToTConverter converter;

        public InputProvider(string filePath, StringToTConverter converter)
        {
            this.filePath = filePath;
            this.converter = converter;

            this.fileStream = new StreamReader(this.filePath);
        }

        public T Current { get; private set; }

        object IEnumerator.Current => this.Current;

        public void Dispose()
        {
            this.fileStream?.Dispose();
        }

        public bool MoveNext()
        {
            var line = this.fileStream.ReadLine();

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
            this.fileStream = new StreamReader(filePath);
        }
    }
}
