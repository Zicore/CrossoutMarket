using System;

namespace ZicoreConnector.Zicore.Connector.Base
{
    public class Parameter
    {
        public Parameter()
        {
            
        }

        public Parameter(string identifier, object value)
        {
            this.Identifier = identifier;
            this.Value = value;
        }

        private String _identifier;
        private object _value;

        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
