using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Querying;
using NHibernate;
using NHibernate.Criterion;

namespace NCS.Repository.NHibernate.Repositories
{
    public static class QueryTranslator
    {
        public static ICriteria TranslateIntoNHQuery<T>(this Query query,
                                                        ICriteria criteria)
        {
            BuildQueryFrom(query, criteria);

            if (query.OrderByProperty != null)
                criteria.AddOrder(new Order(query.OrderByProperty.PropertyName,
                                                  !query.OrderByProperty.Descending));

            return criteria;
        }

        private static void BuildQueryFrom(Query query, ICriteria criteria)
        {
            IList<ICriterion> critrions = new List<ICriterion>();

            if (query.Criterias != null)
            {
                foreach (Criterion c in query.Criterias)
                {
                    ICriterion criterion;

                    switch (c.CriteriaOperator)
                    {
                        case CriteriaOperator.Equal: //==
                            criterion = Expression.Eq(c.PropertyName, c.Value);
                            break;
                        case CriteriaOperator.LesserThanOrEqual: //<=
                            criterion = Expression.Le(c.PropertyName, c.Value);
                            break;
                        case CriteriaOperator.LessThan://<
                            criterion = Expression.Lt(c.PropertyName, c.Value);
                            break;
                        case CriteriaOperator.GreaterThan: //>
                            criterion = Expression.Ge(c.PropertyName, c.Value);
                            break;
                        case CriteriaOperator.GreaterThanOrEqual: //>=
                            criterion = Expression.Gt(c.PropertyName, c.Value);
                            break;
                        case CriteriaOperator.Like:
                            criterion = Expression.Like(c.PropertyName, c.Value);

                            break;
                        default:
                            throw new ApplicationException("No operator defined");
                    }

                    critrions.Add(criterion);
                }

                if (query.QueryOperator == QueryOperator.And)
                {
                    Conjunction andSubQuery = Expression.Conjunction();
                    foreach (ICriterion criterion in critrions)
                    {
                        andSubQuery.Add(criterion);
                    }

                    criteria.Add(andSubQuery);
                }
                else
                {
                    Disjunction orSubQuery = Expression.Disjunction();
                    foreach (ICriterion criterion in critrions)
                    {
                        orSubQuery.Add(criterion);
                    }
                    criteria.Add(orSubQuery);
                }

                foreach (Query sub in query.SubQueries)
                {
                    BuildQueryFrom(sub, criteria);
                }
            }

        }
    }
}
