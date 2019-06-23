![alt tag](https://github.com/jchristn/chunkdecoder/blob/master/assets/icon.ico)

# ChunkDecoder 

ChunkDecoder is useful to help decode chunk-transfer-encoded data, particularly from HTTP.

## New in v1.0.x

- Initial release
- Does not support signature verification of chunk data

## Usage
```
using ChunkDecoder;

public Decoder decoder = new Decoder();

// process a byte array
if (decoder.Decode(data, out outData))
{
  // success
}

// process a stream
long outLength = 0;
MemoryStream outStream = null;
if (decoder.Decode(stream, out outLength, out outStream))
{
  // success
}
```

## Functionality

Chunk-transfer-encoded data is sent in segments, where each segment is comprised of a line containing the length (in hexadecimal) and the subsequent line containing the payload.  Each line ends with a carriage return and line feed <crlf>.

The final line ends with ```0<crlf><crlf>``` indicating the end of the stream.

For instance:
```
6<crlf>        // 6 bytes of data will follow
Hello_<crlf>   // data.  The value is 'Hello_' where _ is actually a space
5<crlf>        // 5 bytes of data will follow
World<crlf>    // data.  The value is 'World'
0<crlf><crlf>  // end of stream
```
Results in:
```
Hello world
```

For more information, please see: 

- https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Transfer-Encoding
- https://en.wikipedia.org/wiki/Chunked_transfer_encoding

## Limitations

Chunk transfer encoding commonly includes key-value pairs in the length line, where the length and the key-value pairs are all separated by semicolons.  These key-value pairs can be used for additional processing including validation of the included segment of data.

For instance:
```
6;chunk-signature=A917FE91...;anotherkey=anothervalue<crlf>
```

The current release of ChunkDecoder does NOT support signature validation or processing of additional key-value pairs in the length line.

## Version History

v1.0.x
- Initial release
