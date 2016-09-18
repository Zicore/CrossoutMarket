using System;

namespace ZicoreConnector.Zicore.Connector.Base
{
    public class Parameter
    {
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
