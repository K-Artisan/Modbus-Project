using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using NCS.Infrastructure.Querying;
using NCS.Repository.ADO.DataMapper;

namespace NCS.Repository.ADO.DataSession
{
    public static class QueryTranslator
    {
        public static string TranslateIntoSqlString<T>(this Query query, string baseSelectQuery, List<MySqlParameter> parameters)
        {
            string resulitSqlString = string.Empty;

            if (query.IsNamedQuery())
            {

            }
            else
            {
                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append(baseSelectQuery);
                bool isNotfirstFilterClause = false;

                if (query.Criterias.Count() > 0)
                    sqlQuery.Append("where ");   

                foreach (Criterion criterion in query.Criterias)
                {
                    if (isNotfirstFilterClause)
                        sqlQuery.Append(GetQueryOperator(query));

                    //"@" + criterion.PropertyName + criterion.Value.ToString(),是为了保证参数的唯一性
                    string sqlParementName = "@" + criterion.PropertyName + parameters.Count.ToString();

                    sqlQuery.Append(AddFilterClauseFrom<T>(criterion, sqlParementName));
                    parameters.Add(new MySqlParameter(sqlParementName, criterion.Value));

                    isNotfirstFilterClause = true;
                }

                if (null != query.OrderByProperty)
                {
                    sqlQuery.Append(GenerateOrderByClauseFrom<T>(query.OrderByProperty));
                }
                resulitSqlString = sqlQuery.ToString();
            }

            return resulitSqlString;
        }

        private static string GenerateOrderByClauseFrom<T>(OrderByClause orderByClause)
        {
            if (null == orderByClause)
            {
                return " ";
            }

            return String.Format("order by {0} {1}",
                FindTableColumnFor<T>(orderByClause.PropertyName), orderByClause.Descending ? "desc" : "asc");
        }

        private static string GetQueryOperator(Query query)
        {
            if (query.QueryOperator == QueryOperator.And)
                return "and ";
            else
                return "or ";
        }

        private static string AddFilterClauseFrom<T>(Criterion criterion, string sqlParementName)
        {
            return string.Format("{0} {1} {2} ", 
                FindTableColumnFor<T>(criterion.PropertyName), 
                FindSqlOperatorFor(criterion.CriteriaOperator),
                sqlParementName);
        }

        private static string FindSqlOperatorFor(CriteriaOperator criteriaOperator)
        {
            switch (criteriaOperator)
            {
                case CriteriaOperator.Equal:
                    return "=";
                case CriteriaOperator.NotApplicable:
                    return "<>";
                case CriteriaOperator.LessThan:
                    return "<";
                case CriteriaOperator.LesserThanOrEqual:
                    return "<=";
                case CriteriaOperator.GreaterThan:
                    return ">";
                case CriteriaOperator.GreaterThanOrEqual:
                    return ">=";
                default:
                    throw new ApplicationException("No operator defined.");
            }
        }

        private static string FindTableColumnFor<T>(string propertyName)
        {
            string tableColumName = string.Empty;

            Dictionary<string, string> propertyMapToTableColumn = 
                DataMapperFactory.GetDataMapper<T>().PropertyMapToTableColumn;

            if (propertyMapToTableColumn.ContainsKey(propertyName))
            {
                tableColumName = propertyMapToTableColumn[propertyName];
            }
            else
            {
                throw new Exception("属性:" + propertyName + "在数据库表中不存在对用的列");
            }

            return tableColumName;

        }
    }
}
