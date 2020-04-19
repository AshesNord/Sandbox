using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using ExpressionTest.Results;

namespace ExpressionTest.Translator {
    public class ResultTranslator<TResult> where TResult : new() {

        private List<MemberAssignment> _memberBindingExpressions;
        private HashSet<string> _memberNames;

        public static ResultTranslator<TResult> GetTranslator() {
            return new ResultTranslator<TResult> {
                _memberBindingExpressions = new List<MemberAssignment>(),
                _memberNames = new HashSet<string>()
            };
        }


        private ResultTranslator() {}

        public ResultTranslator<TResult> Translate<T>(Expression<Func<TResult, IEnumerable<T>>> propertyGetter) {

            if (propertyGetter.Body is MemberExpression member) {
                AddResultGetter(member, typeof(Repository), nameof(Repository.Translate), new Type[] { typeof(T) });
            } else {
                throw new ArgumentException($"Должна быть функция, возвращающая публичное свойство объекта");
            }

            return this;
        }


        private void AddResultGetter(MemberExpression member, Type repoType, string repoMethodName, Type[] repoTypeArguments, params Expression[] repoParams) {
            
            if (_memberNames.Contains(member.Member.Name)) {
                throw new InvalidOperationException($"Выражение для записи в поле {member.Member.Name} уже было!");
            } else {
                _memberNames.Add(member.Member.Name);
            }

            var getResultExpression = Expression.Call(repoType, repoMethodName, repoTypeArguments, repoParams);
            var memberBindingExpression = Expression.Bind(member.Member, getResultExpression);

            _memberBindingExpressions.Add(memberBindingExpression);
        }

        private Func<TResult> BuildExpression() {
            var newExpression = Expression.New(typeof(TResult));

            var init = Expression.MemberInit(newExpression, _memberBindingExpressions);
            var lambda = Expression.Lambda<Func<TResult>>(init);
            return lambda.Compile();
        }

        public TResult Run() {
            var func = BuildExpression();
            
            return func();
        }

    }
}
