using System;
using System.Threading;

namespace CancellationTokenExtensionsTests
{

	[TestClass]
	public class CancellationTokenExtensionsTests
	{
		[TestMethod]
		public void token_source_dispose_does_not_cancel_the_token()
		{
			using (var cts = new CancellationTokenSource())
			{
				var token = cts.Token;

				// Check token status.
				Assert.IsTrue(token.CanBeCanceled);
				Assert.IsFalse(token.IsCancellationRequested);

				cts.Dispose();

				// Check status of token after dispose;
                Assert.IsTrue(token.CanBeCanceled);
                Assert.IsFalse(token.IsCancellationRequested);
            }
        }

		[TestMethod]
		public void Can_we_access_the_internal_token_Source()
		{
			using (var cts = new CancellationTokenSource())
			{
				var t = cts.Token;
				Assert.AreSame(cts, CancellationTokenExtensions.GetSetCanInternalTokenSource(ref t));
			}
		}
	}
}