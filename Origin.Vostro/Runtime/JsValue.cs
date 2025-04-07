using System;

namespace Origin.Vostro.Runtime
{
    public struct JsValue
    {
        private readonly ValueType _type;
        private readonly object _value;

        public static readonly JsValue Undefined = new(ValueType.Undefined, null);
        public static readonly JsValue Null = new(ValueType.Null, null);

        private JsValue(ValueType type, object value)
        {
            _type = type;
            _value = value;
        }

        public static JsValue CreateNumber(double value) => new(ValueType.Number, value);
        public static JsValue CreateBoolean(bool value) => new(ValueType.Boolean, value);
        public static JsValue CreateString(string value) => new(ValueType.String, value);
        public static JsValue CreateObject(object obj) => new(ValueType.Object, obj);

        public bool IsUndefined => _type == ValueType.Undefined;
        public bool IsNull => _type == ValueType.Null;
        public bool IsNumber => _type == ValueType.Number;
        public bool IsBoolean => _type == ValueType.Boolean;
        public bool IsString => _type == ValueType.String;
        public bool IsObject => _type == ValueType.Object;

        public double AsNumber() => (double)_value;
        public bool AsBoolean() => (bool)_value;
        public string AsString() => (string)_value;
        public object AsObject() => _value;

        private enum ValueType
        {
            Undefined,
            Null,
            Boolean,
            Number,
            String,
            Object
        }
    }
}
