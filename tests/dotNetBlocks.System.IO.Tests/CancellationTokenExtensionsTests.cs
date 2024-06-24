using System;
using System.Threading;

namespace CancellationTokenExtensionsTests
{

	[TestClass]
	public class CancellationTokenExtensionsTests
	{
		[TestMethod]
		public void does_dispose_cancel_the_token()
		{
			using (var cts = new CancellationTokenSource())
			{
				var token = cts.Token;
			}
		}
	}
}