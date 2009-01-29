﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using NHibernate.Criterion;

namespace Atlanta.Application.Domain.DomainBase
{

    /// <summary>
    /// Mapper class to convert ClientQuery to NHibernate DetachedCriteria
    /// </summary>
    public static class ClientQueryConverter
    {

        private readonly static IDictionary<ExpressionType, Func<string, object, ICriterion>> _converters = null;

        static ClientQueryConverter()
        {
            _converters = new Dictionary<ExpressionType, Func<string, object, ICriterion>>();

            _converters[ExpressionType.Equal] = NHibernate.Criterion.Expression.Eq;
        }

        /// <summary>
        /// Conversion entry point
        /// </summary>
        public static DetachedCriteria ToDetachedCriteria(ClientQuery query)
        {
            Type criteriaType = GetType(query.ForClass);
            DetachedCriteria criteria = DetachedCriteria.For(criteriaType);

            foreach (ClientQueryExpression expression in query.Expressions)
            {
                criteria.Add(Convert(expression));
            }

            return criteria;
        }

        private static ICriterion Convert(ClientQueryExpression expression)
        {
            Func<string, object, ICriterion> converter = _converters[expression.Operator];
            return converter(expression.Property, expression.Operand);
        }

        private static Type GetType(string type)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type targetType = assembly.GetType(type);
                if (targetType != null)
                {
                    return targetType;
                }
            }
            throw new Exception("Type not found: " + type);
        }

    }

}
