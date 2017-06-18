using System;

namespace ZicoreConnector.Zicore.Connector.Base
{
    public class QueryResult
    {
        public QueryResult() : this(0)
        {

        }

        public QueryResult(int rowCount, bool hasResult, bool hasError, Exception ex)
        {
            this.RowCount = rowCount;
            this.HasResult = hasResult;
            this.Exception = ex;
        }

        public QueryResult(int rowCount)
        {
            this.RowCount = rowCount;
            this.HasResult = false;
            this.Exception = null;

            if (rowCount != -1)
            {
                this.HasResult = true;
            }
        }

        public QueryResult(Exception ex)
        {
            this.RowCount = -1;
            this.HasResult = false;
            this.Exception = ex;
        }

        public static QueryResult Default { get; } = new QueryResult(0, false, false, null);

        public Boolean HasResultAndNoError
        {
            get
            {
                return HasResult && !HasError;
            }
        }

        public int RowCount { get; set; } = 0;

        public bool HasResult { get; set; } = false;

        public bool HasError
        {
            get { return Exception != null; }
        }

        public Exception Exception { get; set; } = null;

        public object Data { get; set; } = null;
        public string Query { get; set; }
        /// <summary>
        /// Is set whenever a broader exception interrupted a loop to commit multiple rows
        /// </summary>
        public bool BulkError { get; set; } = false;

        public long LastInsertedId { get; set; }
    }
}
