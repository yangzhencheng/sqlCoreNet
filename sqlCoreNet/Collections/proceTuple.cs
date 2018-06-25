using System;

namespace sqlCoreNet.Collections
{
    [Serializable]
    public class proceTuple
    {
        private string _paramName;
        private string _typeName;
        private int _typeLength;
        private object _value = null;
        public string ParamName
        {
            get
            {
                return this._paramName;
            }
            set
            {
                this._paramName = value;
            }
        }
        public string TypeName
        {
            get
            {
                return this._typeName;
            }
            set
            {
                this._typeName = value;
            }
        }
        public int TypeLegnth
        {
            get
            {
                return this._typeLength;
            }
            set
            {
                this._typeLength = value;
            }
        }
        public object inputData
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }
        public proceTuple()
        {
        }
        public proceTuple(string paramName, string typeName, int typeLength = 0)
        {
            this._paramName = paramName;
            this._typeName = typeName;
            this._typeLength = typeLength;
        }
        public proceTuple(string paramName, string typeName, int typeLength = 0, object inputData = null)
        {
            this._paramName = paramName;
            this._typeName = typeName;
            this._typeLength = typeLength;
            this._value = inputData;
        }
    }
}
