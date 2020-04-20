using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest.Results {
    public class Context {

        private readonly IEnumerable<FirstResultWrapper> firstResult =
            new List<FirstResultWrapper> {
                new FirstResultWrapper { Name = "FirstName", Value = 10 },
                new FirstResultWrapper { Name = "SecondName", Value = 20 },
                new FirstResultWrapper { Name = "ThirdName", Value = 30 }
            };

        private readonly IEnumerable<SecondResultWrapper> secondResult =
            new List<SecondResultWrapper> {
                new SecondResultWrapper { City = "Paris", Country = "France" },
                new SecondResultWrapper { City = "Tokyo", Country = "Japan" },
                new SecondResultWrapper { City = "New York", Country = "USA" },
                new SecondResultWrapper { City = "London", Country = "UK" }
            };

        public IEnumerable<TResult> Translate<TResult>(Reader reader) {
            var resultType = typeof(TResult);

            // Исключительно для отладки.
            reader.TranslateCall(resultType.Name);

            if (resultType == typeof(FirstResultWrapper)) {
                return (IEnumerable<TResult>)firstResult;
            }

            if (resultType == typeof(SecondResultWrapper)) {
                return (IEnumerable<TResult>)secondResult;
            }

            throw new ArgumentException($"Cannot translate result of type {resultType.Name}");
        }

        

    }
}
