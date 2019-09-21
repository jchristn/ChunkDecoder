![alt tag](https://github.com/jchristn/chunkdecoder/blob/master/assets/icon.ico)

# ChunkDecoder 

[![][nuget-img]][nuget]

[nuget]:     https://www.nuget.org/packages/ChunkDecoder/
[nuget-img]: https://badge.fury.io/nu/Object.svg

ChunkDecoder is useful to help decode chunk-transfer-encoded data, particularly from HTTP.

## New in v1.0.4

- XML documentation

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
6<crlf>        // 6 bytes of data will follow.
Hello_<crlf>   // data.  The value is 'Hello_' where _ is actually a space.
5<crlf>        // 5 bytes of data will follow.
World<crlf>    // data.  The value is 'World'.
0<crlf><crlf>  // end of stream.
```
Results in:
```
Hello world
```

For more information, please see: 

- https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Transfer-Encoding
- https://en.wikipedia.org/wiki/Chunked_transfer_encoding

## Callbacks

In cases where the supplied data has signatures, i.e.
```
6;chunk-signature=foo<crlf>  // 6 bytes of data will follow, and the signature of the chunk is foo.
Hello_<crlf>                 // data.  The value is 'Hello ' and _ is actually a space.
5;chunk-signature:bar<crlf>  // 5 bytes of data will follow, and the signature of the chunk is bar.
World<crlf>                  // data.  The value is 'World'.
0;<crlf><crlf>               // end of stream.
```

You can assigned the ```Decoder.ProcessSignature``` callback, which has the following signature:
```
using System.Collections.Generic;
static bool ProcessSignature(KeyValuePair<string, string>, byte[] data)
{
	return true;
}
```

This method should return ```true```, otherwise ChunkDecoder will assume there is an error and terminate.

If you wish to process each chunk as it is read, set the ```Decoder.ProcessChunk``` callback, which has the followning signature:
```
static bool ProcessChunk(byte[] data)
{
	return true;
}
```

This method should also return ```true```, for the same reason.  ProcessChunk is always called after ProcessSignature.

## Version History

Refer to CHANGELOG.md
