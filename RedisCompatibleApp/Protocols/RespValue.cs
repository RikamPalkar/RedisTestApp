using System;

namespace RedisCompatibleApp.Protocols;

public class RespValue
{
    // Lines 8-12: Properties (read-only, no setters) doubt 0
    public RespType Type { get; } // what type is this
    public string? StringValue { get; } // for Simple string, Error, Bulk string
    public long? IntegerValue { get; }
    public RespValue[]? ArrayValue { get; }
    public bool IsNull { get; }// Is this $-1 or *-1?
    
    // Lines 14-22: Private constructor (only factory methods can create)
    private RespValue(RespType type, string? stringValue = null, 
        long? integerValue = null, RespValue[]? arrayValue = null, bool isNull = false)
    {
        Type = type;
        StringValue = stringValue;
        IntegerValue = integerValue;
        ArrayValue = arrayValue;
        IsNull = isNull;
    }

    // Lines 25-42: Factory methods (the ONLY way to create RespValue) - doubt 1
    public static RespValue SimpleString(string value) => new(RespType.SimpleString, stringValue: value);
    public static RespValue Error(string message) => new(RespType.Error, stringValue: message);
    public static RespValue Integer(long value) => new(RespType.Integer, integerValue: value);
    public static RespValue BulkString(string? value) => value == null ? new(RespType.BulkString, isNull: true) : new(RespType.BulkString, stringValue: value);
    public static RespValue Array(RespValue[]? value) => value == null ? new(RespType.Array, isNull: true) : new(RespType.BulkString, arrayValue: value);

    // Lines 44-49: Pre-built common responses (reusable) - doubt 2
    public static RespValue Null => new(RespType.BulkString, isNull: true); // $-1\r\n (key not found)
    public static RespValue NullArray => new(RespType.Array, isNull: true); // *-1\r\n
    public static RespValue OK => SimpleString("OK"); // +OK\r\n
    public static RespValue Pong => SimpleString("PONG"); // +PONG\r\n
    public static RespValue Zero => Integer(0); // :0\r\n
    public static RespValue One => Integer(1); // :1\r\n

    // Lines 56-62: Convert any type to string - doubt 3
    public string? AsString() => Type switch // Type is property of this class
    {
        RespType.SimpleString => StringValue,
        RespType.Error => StringValue,
        RespType.BulkString => StringValue,
        _ => null
    };

    // Lines 67-73: Convert any type to number - Conceptually when is &&
    public long? AsInteger() => Type switch
    {
        RespType.Integer => IntegerValue,
        RespType.SimpleString when long.TryParse(StringValue, out var val) => val,
        RespType.BulkString when long.TryParse(StringValue, out var val) => val,
        _ => null
    };

    //doubt 4
    public override string ToString() => Type switch
    {
        // +OK\r\n → "SimpleString(OK)"
        RespType.SimpleString => $"SimpleString({StringValue})",
        
        // -ERR unknown\r\n → "Error(ERR unknown)"
        RespType.Error => $"Error({StringValue})",
        
        // :42\r\n → "Integer(42)"
        RespType.Integer => $"Integer({IntegerValue})",
        
        // $-1\r\n → "BulkString(null)"
        RespType.BulkString when IsNull => "BulkString(null)",
        
        // $5\r\nhello\r\n → "BulkString(hello)"
        RespType.BulkString => $"BulkString({StringValue})",
        
        // *-1\r\n → "Array(null)"
        RespType.Array when IsNull => "Array(null)",
        
        // *3\r\n... → "Array(3 items)"
        RespType.Array => $"Array({ArrayValue?.Length} items)",
        
        // Fallback (should never happen)
        _ => "Unknown"
    };
}
