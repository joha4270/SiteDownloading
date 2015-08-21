using System;
using System.Threading;
using System.Threading.Tasks;

namespace web2Ebook.HttpTools
{
	//internal, public temp for testing
	public class RateLimiter
	{
		private float _rate = 1;
		private TimeSpan _duration = TimeSpan.FromSeconds(1);
		private DateTime _next = DateTime.MinValue;
		
		public RateLimiter()
		{
			
		}

		/// <summary>
		/// How many requests per seconds should be allowed
		/// </summary>
		public float Rate
		{
			get { return _rate; }
			set
			{
				_rate = value;
				_duration = TimeSpan.FromSeconds(1.0/value);
			}
		}

		public Task WaitForFree()
		{
			return WaitForFree(CancellationToken.None);
		} 

		public Task WaitForFree(CancellationToken token)
		{
			if (DateTime.UtcNow > _next)
			{
				_next = DateTime.UtcNow;
			}
			_next += _duration;
			return Task.Delay(_next - DateTime.UtcNow, token);
		} 
	}
}