using System;
using System.Data.Common;

namespace ZicoreConnector.Zicore.Connector.Extensions
{
    public static class ConnectorExtensions
    {
        public static void AddParameterWithValue(this DbCommand command, string parameterName, object parameterValue)
        {
            if (command.Parameters.Contains(parameterName))
            {
                var parameter = command.Parameters[parameterName];
                if (parameterValue != null)
                {
                    parameter.Value = parameterValue;
                }
                else
                {
                    parameter.Value = DBNull.Value;
                }
            }
            else
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;

                if (parameterValue != null)
                {
                    parameter.Value = parameterValue;
                }
                else
                {
                    parameter.Value = DBNull.Value;
                }

                command.Parameters.Add(parameter);
            }
        }
    }
}
