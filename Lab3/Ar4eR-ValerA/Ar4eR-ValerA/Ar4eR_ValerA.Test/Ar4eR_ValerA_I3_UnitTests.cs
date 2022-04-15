using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Ar4eR_ValerA.Test.CSharpCodeFixVerifier<
    Ar4eR_ValerA.Ar4eR_ValerA_I3_Analyzer,
    Ar4eR_ValerA.Ar4eR_ValerA_I3_CodeFixProvider>;

namespace Ar4eR_ValerA.Test
{
    [TestClass]
    public class Ar4eR_ValerA_I3_UnitTests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

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
            public void Test()
            {
                Console.WriteLine(1);
                Console.WriteLine(1);
                Console.WriteLine(5);
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
        class t
        {
            public void Test()
            {
            const int magicNumber1 = 1;
            Console.WriteLine(magicNumber1);
            const int magicNumber2 = 1;
            Console.WriteLine(magicNumber2);
            const int magicNumber3 = 5;
            Console.WriteLine(magicNumber3);
            }   
        }
    }";

            await VerifyCS.VerifyCodeFixAsync(test, new[]
            {
                VerifyCS
                    .Diagnostic("Ar4eR_ValerA")
                    .WithSpan(15, 35, 15, 36)
                    .WithArguments("1"),
                VerifyCS
                    .Diagnostic("Ar4eR_ValerA")
                    .WithSpan(16, 35, 16, 36)
                    .WithArguments("1"),
                VerifyCS
                    .Diagnostic("Ar4eR_ValerA")
                    .WithSpan(17, 35, 17, 36)
                    .WithArguments("5")
            }, fixtest);
        }


        [TestMethod]
        public async Task TestMethod3()
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
            public void Test()
            {
                const int magicNumber3 = 5;
                Console.WriteLine(magicNumber3);
            }   
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
        
        [TestMethod]
        public async Task TestMethod4()
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
            public void Test1()
            {
                Console.WriteLine(1);
            }

            public void Test2()
            {
                Console.WriteLine(6);
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
        class t
        {
            public void Test1()
            {
            const int magicNumber1 = 1;
            Console.WriteLine(magicNumber1);
            }

            public void Test2()
            {
            const int magicNumber1 = 6;
            Console.WriteLine(magicNumber1);
            }
        }
    }";

            await VerifyCS.VerifyCodeFixAsync(test, new[]
            {
                VerifyCS
                    .Diagnostic("Ar4eR_ValerA")
                    .WithSpan(15, 35, 15, 36)
                    .WithArguments("1"),
                VerifyCS
                    .Diagnostic("Ar4eR_ValerA")
                    .WithSpan(20, 35, 20, 36)
                    .WithArguments("6")
            }, fixtest);
        }
    }
}