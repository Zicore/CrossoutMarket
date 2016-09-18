using System;
using System.Collections.Generic;

namespace ZicoreConnector.Zicore.Connector.Base
{
    public class QueryListResult : QueryResult
    {
        public QueryListResult(int rowCount, bool hasResult, bool hasError, Exception ex) : base(rowCount, hasResult, hasError, ex)
        {
        }

        public QueryListResult(int rowCount) : base(rowCount)
        {
        }

        public QueryListResult(Exception ex) : base(ex)
        {
        }

        public QueryListResult()
        {
            
        }

        public List<QueryResult> Items { get; set; } = new List<QueryResult>();

        public void Add(QueryResult result)
        {
            Items.Add(result);
        }
    }
}
