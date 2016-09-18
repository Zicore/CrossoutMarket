using System;

namespace ZicoreConnector.Zicore.Connector.Base
{
    public class SqlString
    {
        public SqlString(String cmd)
        {
            this.Command = cmd;
        }

        private String _command = "";
        public String Command
        {
            get { return _command; }
            set { _command = value; }
        }


        public override string ToString()
        {
            return Command;
        }
    }
}
