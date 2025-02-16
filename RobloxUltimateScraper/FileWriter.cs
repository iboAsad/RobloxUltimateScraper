﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RobloxUltimateScraper
{
    /// <summary>
    /// File utilities
    /// </summary>
    internal static class FileWriter
    {
        /// <summary>
        /// Constructs the output file name
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Output file name</returns>
        public static string BuildOutputFileName(string fileName, string? fileExtension)
        {
            return fileName + (!string.IsNullOrEmpty(fileExtension) ? $".{fileExtension}" : "");
        }

        /// <summary>
        /// Saves a stream with the given file path, last modified, and the configured compression type
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="stream">Stream</param>
        /// <param name="lastModified">Last modified</param>
        public static void Save(string filePath, Stream stream, int compressionLevel, DateTime? lastModified = null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                switch (Config.Default.CompressionType)
                {
                    case CompressionType.GZip:
                        ICSharpCode.SharpZipLib.GZip.GZip.Compress(stream, ms, false);
                        filePath += ".gz";
                        break;
                    case CompressionType.BZip2:
                        ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(stream, ms, false, compressionLevel);
                        filePath += ".bz2";
                        break;
                    case CompressionType.Zstd:
                        using (var compressor = new ZstdSharp.CompressionStream(ms, compressionLevel))
                            stream.CopyTo(compressor);
                        filePath += ".zst";
                        break;
                    default:
                        stream.CopyTo(ms);
                        break;
                }

                ms.Position = 0; // or else it wont write anything
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    ms.CopyTo(fileStream);

                if (lastModified.HasValue)
                    File.SetLastWriteTime(filePath, (DateTime)lastModified);
            }
        }
    }
}
