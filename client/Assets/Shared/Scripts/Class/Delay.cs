using System;

public partial class Delay {
	public double timeout;
	public double next_time;

	public Delay(double _timeout) {
		timeout = _timeout;
		reset();
	}

	public bool expired() {
		return this.next_time < (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
	}

	public void expire() {
		next_time = 0;
	}

	public void reset() {
		next_time = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + timeout;
	}
}