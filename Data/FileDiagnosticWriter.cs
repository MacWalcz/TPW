using System.Collections.Concurrent;
using System.Text;

namespace TP.ConcurrentProgramming.Data.Diagnostics
{
    internal class FileDiagnosticWriter : IDisposable
    {
        private readonly BlockingCollection<string> _queue;
        private readonly Thread _worker;
        private readonly StreamWriter _sw;
        private readonly CancellationTokenSource _cts = new();

        public FileDiagnosticWriter(string path, int maxBuffer = 10000)
        {
            _queue = new BlockingCollection<string>(maxBuffer);
            _sw = new StreamWriter(path, append: true, encoding: Encoding.ASCII)
            { AutoFlush = true };

            _worker = new Thread(() => Consume(_cts.Token))
            {
                IsBackground = true
            };
            _worker.Start();
        }

        public bool Enqueue(string line)
            => _queue.TryAdd(line);

        private void Consume(CancellationToken ct)
        {
            try {
                foreach (var line in _queue.GetConsumingEnumerable(ct))
                {
                    bool written = false;
                    int backoff = 50;
                    while (!written && !ct.IsCancellationRequested)
                    {
                        try
                        {
                            _sw.WriteLine(line);
                            written = true;
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(backoff);
                            backoff = Math.Min(backoff * 2, 2000);
                        }
                    }
                }
            } catch(OperationCanceledException)
            { }

            finally
            {
                _sw.Flush();
            }
        }

        public void Dispose()
        {
            _queue.CompleteAdding();
            _cts.Cancel();
            _worker.Join();
            _sw.Dispose();
            _cts.Dispose();
        }
    }
}