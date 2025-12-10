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
    

    //Factory methods
    public static RespValue SimpleString(string value) => new(RespType.SimpleString, stringValue: value);
    public static RespValue Error(string message) => new(RespType.Error, stringValue: message);
    public static RespValue Integer(long value) => new(RespType.Integer, integerValue: value);
    public static RespValue BulkString(string? value) => value == null ? new(RespType.BulkString, isNull: true) : new(RespType.BulkString, stringValue: value);
    public static RespValue Array(RespValue[]? value) => value == null ? new(RespType.Array, isNull: true) : new(RespType.BulkString, arrayValue: value);

    
}
