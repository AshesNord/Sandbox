using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ExpressionTest.Results;
using ExpressionTest.Translator;

namespace ExpressionTest {
    class Program {
        
        static void Main(string[] args) {


            var xxx = ResultTranslator<AllResultsWrapper>
                .GetTranslator()
                .Translate(r => r.FirstResult)
                .Translate(r => r.SecondResult)
                .Run();




            Console.ReadLine();
        }
    }
}
