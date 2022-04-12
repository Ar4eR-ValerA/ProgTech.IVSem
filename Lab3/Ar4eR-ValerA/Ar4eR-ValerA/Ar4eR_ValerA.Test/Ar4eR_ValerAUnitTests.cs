using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Ar4eR_ValerA.Test.CSharpCodeFixVerifier<
    Ar4eR_ValerA.Ar4eR_ValerAAnalyzer,
    Ar4eR_ValerA.Ar4eR_ValerACodeFixProvider>;

namespace Ar4eR_ValerA.Test
{
    [TestClass]
    public class Ar4eR_ValerAUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class t
        {
            public string TryParse()
            {
                return string.Empty;
            }   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";

            var expected = VerifyCS
                .Diagnostic("Ar4eR_ValerA")
                .WithSpan(13, 13, 16, 14)
                .WithArguments("TryParse");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
            //await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}