using System;

namespace ZicoreConnector.Zicore.Connector.Base
{
    public class ConnectorDescription
    {
        String _host;
        String _database;
        String _username;
        String _password;

        int _port;

        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public String CreateConnectionString(ConnectionType type)
        {
            switch (type)
            {
                case ConnectionType.MySql:
                    return $"SERVER={Host};DATABASE={Database}; UID={Username};PASSWORD={Password};Port = {Port};";
                case ConnectionType.MicrosoftSql:
                    return $"server={Host};database={Database};user id={Username};PASSWORD={Password}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
