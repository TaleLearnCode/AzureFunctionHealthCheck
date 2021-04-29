using System;
using System.Diagnostics;

namespace TaleLearnCode.AzureFunctionHealthCheck
{

	public struct ValueStopwatch
	{

		private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
		private long value;

		/// <summary>
		/// Creates and starts a new stopwatch instance.
		/// </summary>
		/// <returns>A new stopwatch which has been started.</returns>
		public static ValueStopwatch StartNew() => new ValueStopwatch(Stopwatch.GetTimestamp());

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueStopwatch"/> struct.
		/// </summary>
		/// <param name="timestamp">The timestamp.</param>
		private ValueStopwatch(long timestamp)
		{
			value = timestamp;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is running.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
		/// </value>
		public bool IsRunning => value > 0;

		/// <summary>
		/// Gets the elapsed ticks.
		/// </summary>
		/// <value>
		/// The elapsed ticks.
		/// </value>
		public long ElapsedTicks
		{
			get
			{
				long timestamp = value;

				long delta;
				if (IsRunning)
				{
					long start = timestamp;
					long end = Stopwatch.GetTimestamp();
					delta = end - start;
				}
				else
				{
					delta = -timestamp;
				}
				return (long)(delta * TimestampToTicks);
			}

		}

		public TimeSpan Elapsed => TimeSpan.FromTicks(ElapsedTicks);

		/// <summary>
		/// Gets the raw timestamp value.
		/// </summary>
		/// <returns>A <c>long</c> representing the value of the raw timestamp.</returns>
		public long GetRawTimestamp() => value;

		/// <summary>
		/// Starts this stopwatch.
		/// </summary>
		public void Start()
		{
			long timestamp = value;

			if (IsRunning) return;  // Already stated; nothing to do

			long newValue = Stopwatch.GetTimestamp() + timestamp;
			if (newValue == 0) newValue = 1;
			value = newValue;
		}

		/// <summary>
		/// Restarts the stopwatch.
		/// </summary>
		public void Restart() => value = Stopwatch.GetTimestamp();

		/// <summary>
		/// Stops this stopwatch.
		/// </summary>
		public void Stop()
		{
			long timestamp = value;
			if (!IsRunning) return;

			long end = Stopwatch.GetTimestamp();
			long delta = end - timestamp;

			value = -delta;
		}

	}

}