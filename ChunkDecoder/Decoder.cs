using System;
using System.IO;

namespace ChunkDecoder
{
    /// <summary>
    /// Decode chunk-transfer-encoded streams or byte arrays, particularly from HTTP data.
    /// </summary>
    public class Decoder
    {
        #region Public-Members

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public Decoder()
        {

        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Decode a chunk-transfer-encoded byte array to a non-chunked byte array.
        /// </summary>
        /// <param name="data">Original byte array with chunk-transfer-encoding.</param>
        /// <param name="outData">Data without encoding.</param>
        /// <returns>True if successful.</returns>
        public bool Decode(byte[] data, out byte[] outData)
        {
            outData = null;
            if (data == null || data.Length < 1) throw new ArgumentNullException(nameof(data));

            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);

            long contentLength = 0;
            MemoryStream outStream = null;

            if (Decode(ms, out contentLength, out outStream))
            {
                if (contentLength > 0)
                {
                    outData = new byte[contentLength];
                    outData = outStream.ToArray();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Decode a chunk-transfer-encoded stream to a non-chunked stream.
        /// </summary>
        /// <param name="stream">Original stream with chunk-transfer-encoding.</param>
        /// <param name="contentLength">Content length of the data in the output stream.</param>
        /// <param name="outStream">Output stream containing data without encoding.</param>
        /// <returns>True if successful.</returns>
        public bool Decode(Stream stream, out long contentLength, out MemoryStream outStream)
        {
            contentLength = 0;
            outStream = new MemoryStream();

            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException("Cannot read from supplied stream.");
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

            int bytesRead = 0;
            long segmentLength = 0;
            byte[] headerBuffer = new byte[1];
            string header = "";
            byte[] dataBuffer = null;
            long bytesRemaining = 0;

            while (true)
            {
                #region Read-Chunk-Length

                headerBuffer = new byte[1];
                header = "";

                while (true)
                {
                    bytesRead = stream.Read(headerBuffer, 0, headerBuffer.Length);
                    if (bytesRead > 0)
                    {
                        header += Convert.ToChar(headerBuffer[0]);
                        if ((int)headerBuffer[0] == 10)
                        {
                            // end of header
                            break;
                        }
                    }
                }

                #endregion

                #region Check-for-End

                header = header.Trim();
                if (!String.IsNullOrEmpty(header)) segmentLength = Convert.ToInt64(header, 16);
                if (segmentLength < 1)  // Segment length of 0 indicates end of message
                {
                    // Read out the final \r\n
                    headerBuffer = new byte[2];
                    bytesRead = stream.Read(headerBuffer, 0, headerBuffer.Length);
                    break;  // end of stream
                }

                #endregion

                #region Read-Data

                dataBuffer = new byte[segmentLength];
                bytesRemaining = segmentLength;

                while (bytesRemaining > 0)
                {
                    bytesRead = stream.Read(dataBuffer, 0, dataBuffer.Length);
                    if (bytesRead > 0)
                    {
                        outStream.Write(dataBuffer, 0, bytesRead);
                        bytesRemaining -= bytesRead;
                        contentLength += bytesRead;
                    }
                }

                #endregion

                #region Read-CRLF-After-Data

                // Read out the final \r\n
                byte[] newlineBuffer = new byte[1];
                while (true)
                {
                    bytesRead = stream.Read(newlineBuffer, 0, newlineBuffer.Length);
                    if (bytesRead > 0)
                    {
                        if ((int)newlineBuffer[0] == 10) break;
                    }
                }

                #endregion
            }

            outStream.Seek(0, SeekOrigin.Begin);
            return true;
        }

        #endregion

        #region Private-Methods

        #endregion
    }
}
