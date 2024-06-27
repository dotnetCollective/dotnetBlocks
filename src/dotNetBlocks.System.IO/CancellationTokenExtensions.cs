using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Threading
{
    /// <summary>
    /// Adds functionality to cancelation tokens.
    /// Specifically helps for scenarios where you have the token but may not have access to the source.
    /// </summary>
    /// <remarks>
    /// Cancelation functions can either access the internal token source or use an intermediate source. Common practice uses a temporary linked <see cref=" CancellationTokenSource"/>. We use that method here.
    /// LLots of these methods only work because         disposing a token source doesn't break the token.
    /// </remarks>
    public static class CancellationTokenExtensions
    {

        #region Access internal source


        /// <summary>
        /// Gets the internal <see cref="CancellationTokenSource"/> from the <see cref="CancellationToken"/>
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><see cref="CancellationTokenSource"/> for the <see cref="CancellationToken"/></returns>
        /// <remarks> Read the documetation before using this very unsafe technique.</remarks>
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_source")]
        public extern static ref CancellationTokenSource GetSetCanInternalTokenSource(ref CancellationToken cancellationToken);

        #endregion

        #region Cancel methods 
        public static void Cancel(this CancellationToken token)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                cts.Cancel();
            }
        }

        public async static Task CancelAsync(this CancellationToken token)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                cts.Cancel();
                await cts.CancelAsync();
            }
        }


        /// <see cref="CancellationTokenSource.CancelAfter(int)"/>
        public static void CancelAfter(this CancellationToken token, int millisecondsDelay)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                cts.CancelAfter(millisecondsDelay);
            }
        }

        /// <see cref="CancellationTokenSource.CancellationTokenSource(TimeSpan)"/>
        public static void CancelAfter(this CancellationToken token, TimeSpan delay)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                cts.CancelAfter(delay);
            }
        }

        #endregion

        #region Linked Token sources

        /// <seealso cref="CancellationTokenSource.CreateLinkedTokenSource(CancellationToken[])"/>
        public static CancellationTokenSource CreateLinkedTokenSource(this IEnumerable<CancellationToken> tokens)
        {
            var cleanTokens = from t in tokens where t != default(CancellationToken) select t;
            return CancellationTokenSource.CreateLinkedTokenSource(cleanTokens.ToArray());
        }

        /// <seealso cref="cref="CancellationTokenSource.CreateLinkedTokenSource(CancellationToken[])"/>
        public static CancellationTokenSource CreateLinkedTokenSource(this CancellationToken cancellationToken, IEnumerable<CancellationToken> tokens)
            => tokens.Append(cancellationToken).CreateLinkedTokenSource();

        /// <seealso cref="CancellationTokenSource.CreateLinkedTokenSource(CancellationToken[])"/>
        public static CancellationTokenSource CreateLinkedTokenSource(this CancellationToken cancellationToken, params CancellationToken[] tokens)
            => tokens.Append(cancellationToken).CreateLinkedTokenSource();


        #endregion

        #region Linked Tokens

        /// <seealso cref="CancellationTokenSource.CreateLinkedTokenSource(CancellationToken[])"/>
        /// <remarks> This only works because disposing a token source doesn't break the token. </remarks>
        public static CancellationToken CreateLinkedToken(this IEnumerable<CancellationToken> tokens)
        {
            using var cts = tokens.CreateLinkedTokenSource(); return cts.Token;
        }
        

        /// <seealso cref="cref="CancellationTokenSource.CreateLinkedTokenSource(CancellationToken[])"/>
        public static CancellationToken CreateLinkedToken(this CancellationToken cancellationToken, IEnumerable<CancellationToken> tokens)
        {
            using var cts = tokens.CreateLinkedTokenSource(); return cts.Token;
        }

    /// <seealso cref="CancellationTokenSource.CreateLinkedTokenSource(CancellationToken[])"/>
    public static CancellationToken CreateLinkedToken(this CancellationToken cancellationToken, params CancellationToken[] tokens)
            => cancellationToken.CreateLinkedToken(tokens);

    #endregion

}
}