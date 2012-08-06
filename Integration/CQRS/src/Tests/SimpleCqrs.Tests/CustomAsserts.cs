using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleCqrs.Core.Tests
{
    public static class CustomAsserts
    {
        [DebuggerStepThrough()]
        public static T Throws<T>(Action action) where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                throw new AssertFailedException(
                    string.Format("Expected exception was not thrown! Got other exception: '{0}'.", ex.GetType())
                    ,ex);
            }

            throw new AssertFailedException("Expected exception was not thrown! None was thrown.");
        }
    }
}