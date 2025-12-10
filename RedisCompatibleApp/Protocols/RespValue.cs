using System;

namespace RedisCompatibleApp.Protocols;

public class RespValue(RespType type, string? stringValue = null, long? integerValue = null, RespValue[]? arrayValue = null,
bool isNull = false)
{
    public RespType Type { get; set; } = type; // what type is this
    public string? StringValue { get; set; } = stringValue; // for Simple string, Error, Bulk string
    public long? IntegerValue { get; set; } = integerValue;
    public RespValue[]? ArrayValue { get; set; } = arrayValue;
    public bool IsNull { get; } = isNull;// Is this $-1 or *-1?
    
    //Factory methods - doubt 1
    public static RespValue SimpleString(string value) => new(RespType.SimpleString, stringValue: value);
    public static RespValue Error(string message) => new(RespType.Error, stringValue: message);
    public static RespValue Integer(long value) => new(RespType.Integer, integerValue: value);
    public static RespValue BulkString(string? value) => value == null ? new(RespType.BulkString, isNull: true) : new(RespType.BulkString, stringValue: value);
    public static RespValue Array(RespValue[]? value) => value == null ? new(RespType.Array, isNull: true) : new(RespType.BulkString, arrayValue: value);

    //doubt 2
    public static RespValue Null => new(RespType.BulkString, isNull: true);
    public static RespValue NullArray => new(RespType.Array, isNull: true);
    public static RespValue OK => SimpleString("OK");
    public static RespValue Pong => SimpleString("PONG");
    public static RespValue Zero => Integer(0);
    public static RespValue One => Integer(1);

    // dount 3
    public string? AsString() => Type switch // Type is property of this class
    {
        RespType.SimpleString => StringValue,
        RespType.Error => StringValue,
        RespType.BulkString => StringValue,
        _ => null
    };

    // Conceptually when is &&
    public long? AsInteger() => Type switch
    {
        RespType.Integer => IntegerValue,
        RespType.SimpleString when long.TryParse(StringValue, out var val) => val,
        RespType.BulkString when long.TryParse(StringValue, out var val) => val,
        _ => null
    };

    public override string ToString() => Type switch
    {
        RespType.SimpleString => $"SimpleString{StringValue}",
        RespType.Error => $"Error{StringValue}",
        RespType.Integer => $"Integer{IntegerValue}",
        RespType.BulkString when IsNull => "BulkString(null)",
        RespType.BulkString => $"BulkString{StringValue}",
        RespType.Array when IsNull => "Array(null)",
        RespType.Array => $"Array({ArrayValue?.Length} items)",
        _ => "Unknown",
    };
}
