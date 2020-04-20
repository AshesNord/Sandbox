using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest.Results {
    public class Repository {

        private static readonly IEnumerable<FirstResultWrapper> firstResult =
            new List<FirstResultWrapper> {
                new FirstResultWrapper { Name = "Weight", Value = 92 },
                new FirstResultWrapper { Name = "Height", Value = 180 }
            };

        private static readonly IEnumerable<SecondResultWrapper> secondResult =
            new List<SecondResultWrapper> {
                new SecondResultWrapper { City = "Norilsk", Country = "Russia" },
                new SecondResultWrapper { City = "Lugansk", Country = "Ukraine" },
                new SecondResultWrapper { City = "New York", Country = "USA" },
                new SecondResultWrapper { City = "London", Country = "UK" }
            };

        public static IEnumerable<TResult> Translate<TResult>() {
            var resultType = typeof(TResult);

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
