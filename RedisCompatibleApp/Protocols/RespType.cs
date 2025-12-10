using System;

namespace RedisCompatibleApp.Protocols;

/// <summary>
/// Define the 5 RESP data types, 
/// When you read first byte of TCP, we can cast it as (RespType)data[0]
/// </summary>
public enum RespType
{
    SimpleString = '+', // OK\r\n
    Error = '-', // -Err message\r\n
    Integer = ':', // :1000\r\n
    BulkString = '$', // $6\r\nfoobar\r\n
    Array = '*' // *2\r\n...
}
