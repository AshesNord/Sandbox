using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest.Results {
    public class Reader {

        private string _name;

        public Reader(string name) {
            _name = name;
        }

        public void NextResult() {
            Console.WriteLine($"READER {_name}: NextResult() invokation.");
        }

        public void TranslateCall(string typeName) {
            Console.WriteLine($"READER {_name}: TranslateCall({typeName}) invokation.");
        }
    }
}
