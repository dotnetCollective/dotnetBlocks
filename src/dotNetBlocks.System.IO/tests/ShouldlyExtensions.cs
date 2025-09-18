using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace dotNetBlocks.System.IO.Tests
{
    public class ShouldNotCompleteInException : ShouldlyTimeoutException // Need to do this to not break existing API
    {
        public ShouldNotCompleteInException(string? message, ShouldlyTimeoutException? inner) : base(message, inner)
        {
        }

    }

    public static  class ShouldlyExtensions
    {

        public static void ShouldNotCompleteIn(this Task actual, TimeSpan timeout, string? custommessage = null)
        {
            try
            {
                Should.CompleteIn(actual, timeout, custommessage); // This should timeout and throw ShouldNotCompleteInException
                throw new ShouldNotCompleteInException(custommessage, null); // Timeout should jump over this.

            }
            catch (ShouldlyTimeoutException e) when (e is not ShouldNotCompleteInException) { } // Swallow the ShouldNotCompleteInException
            catch (Exception) { throw; } // Rethrow all other exceptions.
        }

        public static void ShouldNotCompleteIn<T>(this Task<T> actual, TimeSpan timeout, string? custommessage = null)
        {
            try
            {
                Should.CompleteIn(actual, timeout, custommessage); // This should timeout and throw ShouldNotCompleteInException
                throw new ShouldNotCompleteInException(custommessage, null); // Timeout should jump over this.

            }
            catch (ShouldlyTimeoutException e) when (e is not ShouldNotCompleteInException) { } // Swallow the ShouldNotCompleteInException
            catch (Exception) { throw; } // Rethrow all other exceptions.
        }


        public static void ShouldNotCompleteIn(this Func<Task> function, TimeSpan timeout, string? custommessage = null)
        {
            try
            {
                Should.CompleteIn(function, timeout, custommessage); // This should timeout and throw ShouldNotCompleteInException
                throw new ShouldNotCompleteInException(custommessage, null); // Timeout should jump over this.

            }
            catch (ShouldlyTimeoutException e) when (e is not ShouldNotCompleteInException){ } // Swallow the ShouldNotCompleteInException
            catch (Exception) { throw; } // Rethrow all other exceptions.
        }

        public static void ShouldNotCompleteIn<T>(this Func<Task<T>> actual, TimeSpan timeout, string? custommessage = null)
        {
            try
            {
                Should.CompleteIn(actual, timeout, custommessage); // This should timeout and throw ShouldNotCompleteInException
                throw new ShouldNotCompleteInException(custommessage, null); // Timeout should jump over this.

            }
            catch (ShouldlyTimeoutException e) when (e is not ShouldNotCompleteInException) { } // Swallow the ShouldNotCompleteInException
            catch (Exception) { throw; } // Rethrow all other exceptions.
        }


        public static void ShouldNotCompleteIn<T>(this Func<T> function, TimeSpan timeout, string? custommessage = null)
        {
            try
            {
                Should.CompleteIn(function, timeout, custommessage); // This should timeout and throw ShouldNotCompleteInException
                throw new ShouldNotCompleteInException(custommessage, null); // Timeout should jump over this.

            }
            catch (ShouldlyTimeoutException e) when (e is not ShouldNotCompleteInException) { } // Swallow the ShouldNotCompleteInException
            catch (Exception) { throw; } // Rethrow all other exceptions.
        }



        public static void ShouldNotCompleteIn(this Action action, TimeSpan timeout, string? custommessage = null)
        {
            try
            {
                Should.CompleteIn(action, timeout, custommessage); // This should timeout and throw ShouldNotCompleteInException
                throw new ShouldNotCompleteInException(custommessage, null); // Timeout should jump over this.

            }
            catch (ShouldlyTimeoutException e) when (e is not ShouldNotCompleteInException) { } // Swallow the ShouldNotCompleteInException
            catch (Exception) { throw; } // Rethrow all other exceptions.
        }

    }
}
