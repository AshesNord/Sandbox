using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest.Results {
    public class AllResultsWrapper {

        public IEnumerable<FirstResultWrapper> FirstResult { get; set; }
        public IEnumerable<SecondResultWrapper> SecondResult { get; set; }
    }
}
