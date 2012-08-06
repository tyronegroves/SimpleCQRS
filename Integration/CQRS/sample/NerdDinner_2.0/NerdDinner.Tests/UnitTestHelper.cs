using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NerdDinner.Tests
{
    public static class UnitTestHelper
    {
        public static TException AssertThrows<TException>(Action action) where TException : Exception {
            try {
                action();
            }
            catch (TException e) {
                return e;
            }
            Assert.Fail();
            return null;
        }
    }
}
