using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionTest.Results;

namespace ExpressionTest.Translator {
    public class ResultTranslator<TResult> where TResult : new() {

        private HashSet<string> _bindedPropsNames;
        private List<BindingData> _bindingDataCollection;

        public static ResultTranslator<TResult> GetTranslator() {
            return new ResultTranslator<TResult> {
                _bindedPropsNames = new HashSet<string>(),
                _bindingDataCollection = new List<BindingData>()
            };
        }


        private ResultTranslator() {}

        public ResultTranslator<TResult> Translate<T>(
            Expression<Func<TResult, IEnumerable<T>>> propertyGetter) where T : new() {

            if (propertyGetter.Body is MemberExpression member && 
                member.Member.MemberType == MemberTypes.Property) {
                
                AddBindingData(member.Member.Name, member.Type, nameof(Context.Translate), typeof(T));
            } else {
                throw new ArgumentException($"Должна быть функция, возвращающая публичное свойство объекта");
            }

            return this;
        }


        private void AddBindingData(string propertyName, 
            Type propertyType, string resultReceivingMethodName, Type resultItemType) {

            RegisterPropertyName(propertyName);

            _bindingDataCollection.Add(new BindingData() {
                ResultWrapperPropertyName = propertyName,
                ResultWrapperPropertyType = propertyType,
                ResultSetReceivingMethodName = resultReceivingMethodName,
                ResultSetElementType = resultItemType
            });
        }
        
        private void RegisterPropertyName (string propertyName) {
            if (!_bindedPropsNames.Add(propertyName)) {
                throw new InvalidOperationException($"Недопустимая попытка дважды присвоить значение свойству {propertyName}.");
            } 
        }

        private Func<Context, Reader, TResult> BuildExpression() {

            var contextParam = Expression.Parameter(typeof(Context), "context");
            var readerParam  = Expression.Parameter(typeof(Reader), "reader");
            var resultVar    = Expression.Parameter(typeof(TResult), "result");
            
            var returnLabel = Expression.Label(typeof(TResult));

            var bodyExpressions = new List<Expression> {
                Expression.Assign(resultVar,
                    Expression.New(typeof(TResult)))
            };

            for (var i = 0; i < _bindingDataCollection.Count; i++) {
                var bind = _bindingDataCollection[i];
                bodyExpressions.Add(
                    Expression.Assign(
                        Expression.Property(resultVar, bind.ResultWrapperPropertyName),
                        Expression.Call(contextParam, 
                            bind.ResultSetReceivingMethodName, 
                            new[] { bind.ResultSetElementType }, 
                            readerParam)));
                
                if (i != _bindingDataCollection.Count - 1) {
                    bodyExpressions.Add(Expression.Call(readerParam, typeof(Reader).GetMethod(nameof(Reader.NextResult))));
                }
            }

            bodyExpressions.Add(Expression.Label(returnLabel, resultVar));

            var lambdaBody = Expression.Block(
                    new [] { resultVar },
                    bodyExpressions);

            var lambda = Expression.Lambda<Func<Context, Reader, TResult>>(lambdaBody, contextParam, readerParam);


            return lambda.Compile();
        }

        public TResult Run() {
            var context = new Context();
            var reader = new Reader("_RDR_");

            var func = BuildExpression();
            var result = func(context, reader);
            return result;
        }


        private class BindingData {
            public string ResultWrapperPropertyName    { get; set; }
            public Type   ResultWrapperPropertyType    { get; set; }
            public string ResultSetReceivingMethodName { get; set; }
            public Type   ResultSetElementType         { get; set; }
        }
    }
}
