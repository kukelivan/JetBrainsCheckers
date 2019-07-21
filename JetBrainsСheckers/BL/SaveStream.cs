using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace JetBrainsСheckers.BL
{
	public class SaveStream : IDisposable
	{
		private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();
		private readonly ConcurrentQueue<string> _jsonCollection = new ConcurrentQueue<string>();
		private readonly Thread _thread;
		private readonly Thread _mainThread;
		private readonly AutoResetEvent _writeFlag = new AutoResetEvent(false);


		public SaveStream()
		{
			_mainThread = Thread.CurrentThread;
			_thread = new Thread(Work);
			_thread.Start();
		}

		public void Dispose()
		{
			_cancelToken?.Cancel();
			_writeFlag?.Set();
			_thread?.Join();
			_writeFlag?.Dispose();
			_cancelToken?.Dispose();
		}


		public void Save(string json)
		{
			_jsonCollection.Enqueue(json);
			_writeFlag.Set();
		}

		private void Work()
		{
			var cancelTokenToken = _cancelToken.Token;
			while (_mainThread.IsAlive && !cancelTokenToken.IsCancellationRequested)
			{
				_writeFlag.WaitOne(1000);
				if (_jsonCollection.TryDequeue(out var output))
					using (var sw = new StreamWriter("save.json", false, Encoding.Default))
					{
						sw.WriteLine(output);
					}
			}
		}
	}
}